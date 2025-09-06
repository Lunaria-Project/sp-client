#if UNITY_EDITOR
using System.IO;
using UnityEngine;

public static class JsonDataRepositorySetting
{
    public static string GetRepoPath()
    {
        var projectRoot = Directory.GetParent(Application.dataPath)?.FullName ?? string.Empty;
        return Path.Combine(projectRoot, "lunaria-data-json");
    }
}
#endif