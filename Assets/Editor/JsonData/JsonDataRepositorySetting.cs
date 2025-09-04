#if UNITY_EDITOR
using UnityEditor;

public static class JsonDataRepositorySetting
{
    private const string EditorPrefsKey = "JSON_REPO_PATH";

    public static string GetRepoPath()
    {
        return EditorPrefs.GetString(EditorPrefsKey, string.Empty);
    }

    public static void SetRepoPath(string path)
    {
        if (string.IsNullOrEmpty(path)) return;
        EditorPrefs.SetString(EditorPrefsKey, path);
    }
}
#endif
