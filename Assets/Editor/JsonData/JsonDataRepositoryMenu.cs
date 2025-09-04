#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

public class JsonDataRepositoryMenu
{
    [MenuItem("Tools/Data Import/Set JSON Repository Folder", priority = 0)]
    public static void SetJsonRepoFolder()
    {
        var current = JsonDataRepositorySetting.GetRepoPath();
        var start = string.IsNullOrEmpty(current) ? Application.dataPath : current;

        var picked = EditorUtility.OpenFolderPanel("Select JSON Repository Folder", start, "");
        if (!string.IsNullOrEmpty(picked))
        {
            if (!Directory.Exists(picked))
            {
                EditorUtility.DisplayDialog("Folder Error", "선택한 폴더가 존재하지 않습니다.", "OK");
                return;
            }

            JsonDataRepositorySetting.SetRepoPath(picked);
            EditorUtility.DisplayDialog("JSON Repo", $"경로가 저장되었습니다:\n{picked}", "OK");
        }
    }

    [MenuItem("Tools/Data Import/Generate Data from JSON Repository", priority = 10)]
    public static void TestLoad()
    {
        DataCodeGenerator.GenerateGameDataCode();
        DataCodeGenerator.GenerateEnumDataCode();
    }
}
#endif