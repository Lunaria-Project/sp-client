using UnityEditor;

public static class DebugToolMenu
{
    private const string ToggleVisualizeColliderMenuName = "Lunaria/Debug/Visualize Collider";

    [MenuItem(ToggleVisualizeColliderMenuName)]
    private static void ToggleVisualizeCollider()
    {
        var current = PlayerPrefsManager.GetBool(PrefKey.ColliderVisualize);
        PlayerPrefsManager.SetBool(PrefKey.ColliderVisualize, !current);
        LogManager.Log($"Collider Visualize set to {!current}");
    }

    [MenuItem(ToggleVisualizeColliderMenuName, true)]
    private static bool ToggleVisualizeColliderValidate()
    {
        var enabled = PlayerPrefsManager.GetBool(PrefKey.ColliderVisualize);
        Menu.SetChecked(ToggleVisualizeColliderMenuName, enabled);
        return true;
    }
}