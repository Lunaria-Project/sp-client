#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class SheetInfo
{
    public string FileName;
    public string SheetName;
    public string[] ColumnNames;
    public string[] ColumnTypes;
    public List<object[]> Rows;
}

public static class JsonDataLoader
{
    public static List<SheetInfo> LoadAllSheets()
    {
        var list = new List<SheetInfo>();
        
        var repoPath = Path.Combine(JsonDataRepositorySetting.GetRepoPath(), "data");
        if (string.IsNullOrEmpty(repoPath) || !Directory.Exists(repoPath))
        {
            LogManager.LogError($"JSON repo not found: {repoPath}");
            return list;
        }
        
        foreach (var file in Directory.GetFiles(repoPath, "*.json", SearchOption.AllDirectories))
        {
            var text = File.ReadAllText(file);
            var root = JObject.Parse(text);

            var typesObj = (JObject)root["types"];
            var rowsArr = (JArray)root["rows"];

            if (typesObj == null || rowsArr == null) continue;

            var colNames = new List<string>();
            var colTypes = new List<string>();
            foreach (var prop in typesObj.Properties())
            {
                colNames.Add(prop.Name);
                colTypes.Add(prop.Value.ToString());
            }

            var rows = new List<object[]>();
            foreach (var rowToken in rowsArr)
            {
                var arr = ((JArray)rowToken).ToObject<object[]>();
                rows.Add(arr);
            }

            var relPath = Path.GetRelativePath(repoPath, file);
            var firstSep = relPath.IndexOf(Path.DirectorySeparatorChar);
            var parentName= relPath.Substring(0, firstSep);
            var sheetName = Path.GetFileNameWithoutExtension(file);
            list.Add(new SheetInfo
            {
                FileName = parentName,
                SheetName = sheetName,
                ColumnNames = colNames.ToArray(),
                ColumnTypes = colTypes.ToArray(),
                Rows = rows
            });
        }

        return list;
    }
}
#endif