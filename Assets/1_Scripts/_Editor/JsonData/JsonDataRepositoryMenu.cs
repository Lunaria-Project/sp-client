#if UNITY_EDITOR
using UnityEditor;

public class JsonDataRepositoryMenu
{
    [MenuItem("Lunaria/Json Data/[Generate Data from JSON Repository]", priority = 10)]
    public static void TestLoad()
    {
        var sheets = JsonDataLoader.LoadAllSheets();
        DataCodeGenerator.GenerateGameDataCode(sheets);
        DataCodeGenerator.GenerateEnumDataCode(sheets);
        DataCodeGenerator.GenerateGameSettingCode(sheets);
    }
}
#endif