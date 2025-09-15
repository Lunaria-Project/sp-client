using System.Diagnostics;
using Debug = UnityEngine.Debug;

public static class LogManager
{
    [Conditional("UNITY_EDITOR")]
    public static void Log(string message)
    {
        Debug.Log(message);
    }

    [Conditional("UNITY_EDITOR")]
    public static void LogWarning(string message)
    {
        Debug.LogWarning(message);
    }
    
    [Conditional("UNITY_EDITOR")]
    public static void Assert(bool condition, string message = null)
    {
        Debug.Assert(condition, message);
    }

    [Conditional("UNITY_EDITOR")]
    public static void LogError(string message)
    {
        Debug.LogError(message);
    }

    [Conditional("UNITY_EDITOR")]
    public static void LogErrorFormat(string format, params object[] args)
    {
        Debug.LogErrorFormat(format, args);
    }

    [Conditional("UNITY_EDITOR")]
    public static void LogException(System.Exception exception)
    {
        Debug.LogException(exception);
    }
}