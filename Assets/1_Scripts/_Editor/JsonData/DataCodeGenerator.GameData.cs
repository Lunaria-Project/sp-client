#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public static partial class DataCodeGenerator
{
    private const string OutputNamespace = "Generated";
    private const string GameDataPath = "Assets/1_Scripts/Generated/GeneratedGameData.cs";
    private const string GameGetterDataPath = "Assets/1_Scripts/Generated/GameData.GeneratedClass.cs";
    private const string DataLoaderPath = "Assets/1_Scripts/Generated/GameData.GeneratedLoader.cs";
    private const string KeyColumn = ";key";
    private const string IdColumn = ";id";

    private static readonly Dictionary<string, string> TypeMap = new(StringComparer.OrdinalIgnoreCase)
    {
        { "int", "int" },
        { "float", "float" },
        { "double", "double" },
        { "long", "long" },
        { "bool", "bool" },
        { "string", "string" }
        // "enum" 은 별도 처리
    };

    public static void GenerateGameDataCode(List<SheetInfo> sheets)
    {
        try
        {
            var dataCode = GenerateDataCode(sheets);
            WriteFile(GameDataPath, dataCode);

            LogManager.Log($"[GameDataCodeGenerator] Generated: {GameDataPath}");

            var dataGetterCode = GenerateDataGetterCode(sheets);
            WriteFile(GameGetterDataPath, dataGetterCode);

            var dataLoaderCode = GenerateDataLoaderCode(sheets);
            WriteFile(DataLoaderPath, dataLoaderCode);

            LogManager.Log($"[GameDataCodeGenerator] Generated: {GameGetterDataPath}");
        }
        catch (Exception e)
        {
            LogManager.LogException(e);
            EditorUtility.DisplayDialog("GameData Generation Error", e.Message, "OK");
        }
    }

    private static string GenerateDataCode(List<SheetInfo> sheets)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"namespace {OutputNamespace}");
        sb.AppendLine("{");

        for (var i = 0; i < sheets.Count; i++)
        {
            var sheet = sheets[i];

            var className = sheet.SheetName;
            if (!CanGenerateCode(className)) continue;

            sb.AppendIndentedLine($"public partial class {className}", 1);
            sb.AppendIndentedLine("{", 1);

            for (var j = 0; j < sheet.ColumnNames.Length; j++)
            {
                var colName = sheet.ColumnNames[j];
                var columnType = GetColumnType(sheet.ColumnTypes[j]);
                var isEnum = string.Equals(columnType, "enum", StringComparison.OrdinalIgnoreCase);

                var csType = isEnum ? sheet.ColumnNames[j] : (TypeMap.GetValueOrDefault(columnType ?? "", "string"));

                sb.AppendIndentedLine($"public {csType} {colName} {{ get; private set; }}", 2);
            }

            sb.AppendLine();

            var paramList = MakeConstructorParams(sheet);
            sb.AppendIndentedLine($"public {className}({paramList})", 2);
            sb.AppendIndentedLine("{", 2);
            foreach (var raw in sheet.ColumnNames)
            {
                sb.AppendIndentedLine($"{raw} = {ToCamelCase(raw)};", 3);
            }

            sb.AppendIndentedLine("}", 2);
            sb.AppendIndentedLine("}", 1);

            if (i != sheets.Count - 1)
            {
                sb.AppendLine();
            }
        }

        sb.AppendLine("}");
        return sb.ToString();
    }

    private static string GenerateDataGetterCode(List<SheetInfo> sheets)
    {
        var sb = new StringBuilder();

        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("using Generated;");
        sb.AppendLine();
        sb.AppendLine("public partial class GameData");
        sb.AppendLine("{");

        for (var i = 0; i < sheets.Count; i++)
        {
            var sheet = sheets[i];
            var className = sheet.SheetName;
            if (!CanGenerateCode(className)) continue;

            var keyIndex = FindKeyIndex(sheet);
            var (HasKeyColumn, KeyColumnName) = (keyIndex >= 0, keyIndex >= 0 ? sheet.ColumnNames[keyIndex] : string.Empty);
            if (HasKeyColumn)
            {
                sb.AppendIndentedLine($"// {sheet.SheetName} - {className}, key: {KeyColumnName}", 1);
                sb.AppendIndentedLine($"public IReadOnlyDictionary<int, {className}> DT{className} => _dt{className};", 1);
                sb.AppendIndentedLine($"public bool TryGet{className}(int key, out {className} result) => DT{className}.TryGetValue(key, out result);", 1);
                sb.AppendIndentedLine($"public bool Contains{className}(int key) => DT{className}.ContainsKey(key);", 1);
                sb.AppendIndentedLine($"private readonly Dictionary<int, {className}> _dt{className} = new();", 1);
            }
            else
            {
                sb.AppendIndentedLine($"// {sheet.SheetName} - {className}", 1);
                sb.AppendIndentedLine($"public IReadOnlyList<{className}> DT{className} => _dt{className};", 1);
                sb.AppendIndentedLine($"private List<{className}> _dt{className} = new();", 1);
            }

            if (i != sheets.Count - 1)
            {
                sb.AppendLine();
            }
        }

        sb.AppendLine("}");

        return sb.ToString();
    }

    private static string GenerateDataLoaderCode(List<SheetInfo> sheets)
    {
        var sb = new StringBuilder();

        sb.AppendLine("using System;");
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("using Generated;");
        sb.AppendLine();
        sb.AppendLine("public partial class GameData");
        sb.AppendLine("{");

        foreach (var sheet in sheets)
        {
            var className = sheet.SheetName;
            var fieldName = "_dt" + className;

            if (!CanGenerateCode(className)) continue;

            var keyIndex = FindKeyIndex(sheet);
            var (HasKeyColumn, KeyColumnName) = (keyIndex >= 0, keyIndex >= 0 ? sheet.ColumnNames[keyIndex] : string.Empty);

            var args = new List<string>(sheet.ColumnNames.Length);
            for (var i = 0; i < sheet.ColumnNames.Length; i++)
            {
                var columnType = GetColumnType(sheet.ColumnTypes[i]);
                var isEnum = string.Equals(columnType, "enum", StringComparison.OrdinalIgnoreCase);
                if (isEnum)
                {
                    var columnName = sheet.ColumnNames[i];
                    args.Add($"({columnName})Enum.Parse(typeof({columnName}), (string)row[{i}], true)");
                }
                else
                {
                    var csType = TypeMap.GetValueOrDefault(columnType ?? "", "string");
                    var arg = CastExpr(csType, $"row[{i}]");
                    args.Add(arg);
                }
            }

            sb.AppendIndentedLine($"private void Load{className}(List<object[]> rows)", 1);
            sb.AppendIndentedLine("{", 1);
            sb.AppendIndentedLine("if (rows.IsNullOrEmpty()) return;", 2);
            sb.AppendIndentedLine("foreach (var row in rows)", 2);
            sb.AppendIndentedLine("{", 2);

            sb.AppendIndentedLine($"var newData = new {className}({string.Join(", ", args)});", 3);

            if (HasKeyColumn)
            {
                sb.AppendIndentedLine($"{fieldName}.Add(newData.{KeyColumnName}, newData);", 3);
            }
            else
            {
                sb.AppendIndentedLine($"{fieldName}.Add(newData);", 3);
            }

            sb.AppendIndentedLine("}", 2);
            sb.AppendIndentedLine("}", 1);
            sb.AppendLine();
        }

        sb.AppendIndentedLine("private void InvokeLoadForSheet(string sheetName, List<object[]> rows)", 1);
        sb.AppendIndentedLine("{", 1);
        sb.AppendIndentedLine("switch (sheetName)", 2);
        sb.AppendIndentedLine("{", 2);
        foreach (var sheet in sheets.Select(s => s.SheetName).Distinct())
        {
            if (!CanGenerateCode(sheet)) continue;
            sb.AppendIndentedLine($"case \"{sheet}\": Load{sheet}(rows); break;", 3);
        }

        sb.AppendIndentedLine("}", 2);
        sb.AppendIndentedLine("}", 1);

        sb.AppendLine("}");

        return sb.ToString();
    }

    private static string MakeConstructorParams(SheetInfo sheet)
    {
        var parts = new List<string>();
        for (var i = 0; i < sheet.ColumnNames.Length; i++)
        {
            var rawName = sheet.ColumnNames[i];
            var columnType = GetColumnType(sheet.ColumnTypes[i]);
            var isEnum = string.Equals(columnType, "enum", StringComparison.OrdinalIgnoreCase);

            var csType = isEnum ? rawName : (TypeMap.GetValueOrDefault(columnType ?? "", "string"));
            parts.Add($"{csType} {ToCamelCase(rawName)}");
        }

        return string.Join(", ", parts);
    }

    private static bool CanGenerateCode(string sheetName)
    {
        var isEnumData = string.Equals(sheetName, EnumDataFileName, StringComparison.OrdinalIgnoreCase);
        var isGameSettingData = string.Equals(sheetName, GameSettingDataFileName, StringComparison.OrdinalIgnoreCase);
        return !isEnumData && !isGameSettingData;
    }

    private static string ToCamelCase(string name)
    {
        if (string.IsNullOrEmpty(name)) return name;
        if (name.Length == 1) return name.ToLowerInvariant();
        return char.ToLowerInvariant(name[0]) + name.Substring(1);
    }

    private static string GetColumnType(string rawType)
    {
        rawType = rawType.Trim();
        rawType = rawType?.Replace(KeyColumn, string.Empty);
        rawType = rawType?.Replace(IdColumn, string.Empty);
        return rawType;
    }

    private static int FindKeyIndex(SheetInfo sheet)
    {
        for (var j = 0; j < sheet.ColumnNames.Length; j++)
        {
            if (sheet.ColumnTypes[j] != null && sheet.ColumnTypes[j].Contains(KeyColumn))
            {
                return j;
            }
        }

        return -1;
    }

    private static string CastExpr(string csType, string srcExpr)
    {
        return csType switch
        {
            "int" => $"Convert.ToInt32({srcExpr})",
            "long" => $"Convert.ToInt64({srcExpr})",
            "float" => $"Convert.ToSingle({srcExpr})",
            "double" => $"Convert.ToDouble({srcExpr})",
            "bool" => $"Convert.ToBoolean({srcExpr})",
            "string" => $"({srcExpr} as string) ?? string.Empty",
            _ => $"{srcExpr}"
        };
    }

    private static void WriteFile(string path, string content)
    {
        var dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        File.WriteAllText(path, content, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
        AssetDatabase.Refresh();
    }
}
#endif