using UnityEditor;

public static class DebugToolMenu
{
    private const string MenuPath = "Lunaria/Debug/Visualize Collider";

    [MenuItem(MenuPath)]
    private static void ToggleVisualizeCollider()
    {
        var current = PlayerPrefsManager.GetBool(PrefKey.ColliderVisualize);
        PlayerPrefsManager.SetBool(PrefKey.ColliderVisualize, !current);
        LogManager.Log($"Collider Visualize set to {!current}");
    }

    [MenuItem(MenuPath, true)]
    private static bool ToggleVisualizeColliderValidate()
    {
        var enabled = PlayerPrefsManager.GetBool(PrefKey.ColliderVisualize);
        Menu.SetChecked(MenuPath, enabled);
        return true;
    }
}
