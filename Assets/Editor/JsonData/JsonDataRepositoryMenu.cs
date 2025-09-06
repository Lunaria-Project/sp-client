#if UNITY_EDITOR
using UnityEditor;

public class JsonDataRepositoryMenu
{
    [MenuItem("Lunaria/Json Data/[Generate Data from JSON Repository]", priority = 10)]
    public static void TestLoad()
    {
        DataCodeGenerator.GenerateGameDataCode();
        DataCodeGenerator.GenerateEnumDataCode();
    }
}
#endif