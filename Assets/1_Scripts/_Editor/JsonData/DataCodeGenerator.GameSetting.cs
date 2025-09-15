using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

public static partial class DataCodeGenerator
{
    private const string GameSettingDataPath = "Assets/1_Scripts/Generated/GeneratedGameSettingData.cs";
    private const string GameSettingDataFileName = "GameSettingData";

    public static void GenerateGameSettingCode(List<SheetInfo> sheets)
    {
        try
        {
            foreach (var sheet in sheets)
            {
                if (!string.Equals(sheet.SheetName, GameSettingDataFileName, StringComparison.OrdinalIgnoreCase)) continue;
                var dataCode = GenerateSettingCode(sheet.Rows);
                WriteFile(GameSettingDataPath, dataCode);
                LogManager.Log($"[GameDataCodeGenerator] Generated: {GameSettingDataPath}");
                return;
            }

            LogManager.LogError("[GameDataCodeGenerator] Failed: GameSettingData 파일을 찾을 수 없습니다.");
        }
        catch (Exception e)
        {
            LogManager.LogException(e);
            EditorUtility.DisplayDialog("GameData Generation Error", e.Message, "OK");
        }
    }

    public static string GenerateSettingCode(List<object[]> rows)
    {
        var sb = new StringBuilder();

        sb.AppendLine("using System;");
        sb.AppendLine();
        sb.AppendLine("public class GameSetting : Singleton<GameSetting>");
        sb.AppendLine("{");

        foreach (var row in rows)
        {
            var csType = row[0]?.ToString() ?? "string";
            var name = row[1]?.ToString() ?? "Field";
            sb.AppendIndentedLine($"public {csType} {name} {{ get; private set; }}", 1);
        }

        sb.AppendLine();
        sb.AppendIndentedLine("public void InvokeLoadForSheet(SheetInfo sheetInfo)", 1);
        sb.AppendIndentedLine("{", 1);
        sb.AppendIndentedLine("foreach (var row in sheetInfo.Rows)", 2);
        sb.AppendIndentedLine("{", 2);
        sb.AppendIndentedLine("var name = (string)row[1];", 3);
        sb.AppendIndentedLine("switch (name)", 3);
        sb.AppendIndentedLine("{", 3);

        foreach (var row in rows)
        {
            var csType = row[0]?.ToString() ?? "string";
            var name = row[1]?.ToString() ?? "Field";
            var cast = CastExpr(csType, "row[2]");

            sb.AppendIndentedLine($"case \"{name}\": {name} = {cast}; break;", 4);
        }

        sb.AppendIndentedLine("}", 3);
        sb.AppendIndentedLine("}", 2);
        sb.AppendIndentedLine("}", 1);
        sb.AppendLine("}");

        return sb.ToString();
    }
}