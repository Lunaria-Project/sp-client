#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public static partial class DataCodeGenerator
{
    private const string EnumDataPath = "Assets/1_Scripts/Generated/GeneratedEnumData.cs";
    private const string EnumDataFileName = "EnumData";

    public static void GenerateEnumDataCode(List<SheetInfo> sheets)
    {
        try
        {
            var enumCode = GenerateEnumCode(sheets);
            WriteFile(EnumDataPath, enumCode);

            LogManager.Log($"[GameDataCodeGenerator] Generated: {EnumDataPath}");
        }
        catch (Exception e)
        {
            LogManager.LogException(e);
            EditorUtility.DisplayDialog("GameData Generation Error", e.Message, "OK");
        }
    }

    private static string GenerateEnumCode(List<SheetInfo> sheets)
    {
        var enumSheets = sheets
            .Where(s => string.Equals(s.FileName, "EnumData", StringComparison.OrdinalIgnoreCase))
            .ToList();

        var byEnum = new Dictionary<string, List<(string Name, string DisplayName, string ResourceKey)>>(StringComparer.Ordinal);

        foreach (var sheet in enumSheets)
        {
            // 필수/선택 컬럼 인덱스
            var idxEnumName = Array.FindIndex(sheet.ColumnNames, c => string.Equals(c, "EnumName", StringComparison.OrdinalIgnoreCase));
            var idxValue = Array.FindIndex(sheet.ColumnNames, c => string.Equals(c, "Value", StringComparison.OrdinalIgnoreCase));
            var idxDisp = Array.FindIndex(sheet.ColumnNames, c => string.Equals(c, "DisplayName", StringComparison.OrdinalIgnoreCase));
            var idxResKey = Array.FindIndex(sheet.ColumnNames, c => string.Equals(c, "ResourceKey", StringComparison.OrdinalIgnoreCase));

            if (idxEnumName < 0 || idxValue < 0)
            {
                LogManager.LogWarning($"[EnumGen] Sheet '{sheet.SheetName}' is missing EnumName/Value columns. Skipped.");
                continue;
            }

            foreach (var row in sheet.Rows)
            {
                if (row == null) continue;
                if (idxEnumName >= row.Length || idxValue >= row.Length) continue;

                var enumName = row[idxEnumName]?.ToString();
                var value = row[idxValue]?.ToString();
                if (string.IsNullOrWhiteSpace(enumName) || string.IsNullOrWhiteSpace(value)) continue;

                var disp = (idxDisp >= 0 && idxDisp < row.Length) ? row[idxDisp]?.ToString() : null;
                var rkey = (idxResKey >= 0 && idxResKey < row.Length) ? row[idxResKey]?.ToString() : null;

                if (!byEnum.TryGetValue(enumName, out var list))
                    byEnum[enumName] = list = new List<(string, string, string)>();

                list.Add((value.Trim(), string.IsNullOrWhiteSpace(disp) ? null : disp.Trim(),
                    string.IsNullOrWhiteSpace(rkey) ? null : rkey.Trim()));
            }
        }

        var sb = new StringBuilder();
        sb.AppendLine($"namespace {OutputNamespace}");
        sb.AppendLine("{");

        foreach (var (enumName, itemsRaw) in byEnum.OrderBy(k => k.Key, StringComparer.Ordinal))
        {
            // 결정적 순서: 멤버 이름 Ordinal 정렬(원하면 원본 순서 유지로 바꿔도 됨)
            var items = itemsRaw
                .GroupBy(x => x.Name, StringComparer.Ordinal) // 중복행 방지
                .Select(g =>
                (
                    Name: g.Key,
                    DisplayName: g.Select(x => x.DisplayName).FirstOrDefault(v => !string.IsNullOrEmpty(v)),
                    ResourceKey: g.Select(x => x.ResourceKey).FirstOrDefault(v => !string.IsNullOrEmpty(v))
                ))
                .OrderBy(x => x.Name, StringComparer.Ordinal)
                .ToList();

            sb.AppendIndentedLine($"[SerializeEnum]", 1);
            sb.AppendIndentedLine($"public enum {enumName}", 1);
            sb.AppendIndentedLine("{", 1);
            sb.AppendIndentedLine("None = 0,", 2);
            var next = 1;
            foreach (var it in items)
            {
                sb.AppendIndentedLine($"{it.Name} = {next},", 2);
                next++;
            }

            sb.AppendIndentedLine("}", 1);
            sb.AppendLine();

            // DisplayName 값이 실제로 하나라도 채워져 있어야 true
            var hasDisplay = items.Any(i => !string.IsNullOrWhiteSpace(i.DisplayName));
            // ResourceKey 값이 실제로 하나라도 채워져 있어야 true
            var hasResKey = items.Any(i => !string.IsNullOrWhiteSpace(i.ResourceKey));


            if (hasDisplay || hasResKey)
            {
                sb.AppendIndentedLine($"public static class {enumName}Extensions", 1);
                sb.AppendIndentedLine("{", 1);

                if (hasDisplay)
                {
                    sb.AppendIndentedLine($"public static string GetDisplayName(this {enumName} value)", 2);
                    sb.AppendIndentedLine("{", 2);
                    sb.AppendIndentedLine("switch (value)", 3);
                    sb.AppendIndentedLine("{", 3);
                    foreach (var it in items)
                    {
                        var text = it.DisplayName ?? it.Name;
                        sb.AppendIndentedLine($"case {enumName}.{it.Name}: return \"{Escape(text)}\";", 4);
                    }

                    sb.AppendIndentedLine("default: return value.ToString();", 4);
                    sb.AppendIndentedLine("}", 3);
                    sb.AppendIndentedLine("}", 2);
                    sb.AppendLine();
                }

                if (hasResKey)
                {
                    sb.AppendIndentedLine($"public static string GetResourceKey(this {enumName} value)", 2);
                    sb.AppendIndentedLine("{", 2);
                    sb.AppendIndentedLine("switch (value)", 3);
                    sb.AppendIndentedLine("{", 3);
                    foreach (var it in items)
                    {
                        var text = it.ResourceKey ?? string.Empty;
                        sb.AppendIndentedLine($"case {enumName}.{it.Name}: return \"{Escape(text)}\";", 4);
                    }

                    sb.AppendIndentedLine("default: return string.Empty;", 4);
                    sb.AppendIndentedLine("}", 3);
                    sb.AppendIndentedLine("}", 2);
                    sb.AppendLine();
                }

                sb.AppendIndentedLine("}", 1);
                sb.AppendLine();
            }
        }

        sb.AppendLine("}");
        return sb.ToString();

        // 문자열에 \, "가 들어있으면 안 됨
        static string Escape(string s) => s.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }
}
#endif