#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public static partial class DataCodeGenerator
{
    private const string OutputNamespace = "Generated";
    private const string GameDataPath = "Assets/1_Scripts/Generated/GeneratedGameData.cs";

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

    public static void GenerateGameDataCode()
    {
        try
        {
            var sheets = JsonDataLoader.LoadAllSheets();

            var dataCode = GenerateDataCode(sheets);
            WriteFile(GameDataPath, dataCode);

            Debug.Log($"[GameDataCodeGenerator] Generated: {GameDataPath}");
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            EditorUtility.DisplayDialog("GameData Generation Error", e.Message, "OK");
        }
    }

    private static string GenerateDataCode(List<SheetInfo> sheets)
    {
        var sb = new StringBuilder();

        sb.AppendLine("using System;");
        sb.AppendLine();
        sb.AppendLine($"namespace {OutputNamespace}");
        sb.AppendLine("{");

        foreach (var sheet in sheets)
        {
            if (sheet.FileName == EnumDataFileName) continue;
            
            var className = sheet.SheetName;

            sb.AppendIndentedLine($"public class {className}", 1);
            sb.AppendIndentedLine("{", 1);

            for (var i = 0; i < sheet.ColumnNames.Length; i++)
            {
                var colName = sheet.ColumnNames[i];
                var rawType = sheet.ColumnTypes[i]?.Trim();
                var isEnum = string.Equals(rawType, "enum", StringComparison.OrdinalIgnoreCase);

                var csType = isEnum ? sheet.ColumnNames[i] : (TypeMap.GetValueOrDefault(rawType ?? "", "string"));

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
            sb.AppendLine();
        }

        sb.AppendLine("}");
        return sb.ToString();
    }

    private static string MakeConstructorParams(SheetInfo sheet)
    {
        var parts = new List<string>();
        for (var i = 0; i < sheet.ColumnNames.Length; i++)
        {
            var rawName = sheet.ColumnNames[i];
            var rawType = sheet.ColumnTypes[i]?.Trim();
            var isEnum = string.Equals(rawType, "enum", StringComparison.OrdinalIgnoreCase);

            var csType = isEnum ? rawName : (TypeMap.GetValueOrDefault(rawType ?? "", "string"));

            parts.Add($"{csType} {ToCamelCase(rawName)}");
        }

        return string.Join(", ", parts);
    }

    private static string ToCamelCase(string name)
    {
        if (string.IsNullOrEmpty(name)) return name;
        if (name.Length == 1) return name.ToLowerInvariant();
        return char.ToLowerInvariant(name[0]) + name.Substring(1);
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