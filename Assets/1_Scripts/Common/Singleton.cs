public class Singleton<T> where T : Singleton<T>, new()
{
    public static T Instance => GetInstance();
    public static bool HasInstance => _instance != null;

    private static T _instance;
    private static readonly object _lock = new();

    private static T GetInstance()
    {
        lock (_lock)
        {
            if (!HasInstance)
            {
                _instance = new T();
                _instance.OnInit();
            }

            return _instance;
        }
    }

    protected virtual void OnInit()
    {
    }

    protected virtual void OnDestroy()
    {
        _instance = null;
    }
}