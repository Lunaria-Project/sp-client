public class Singleton<T> where T : Singleton<T>, new()
{
    public static T Instance { get; private set; } = GetInstance();
    public static bool HasInstance => _instance != null;
    private static T _instance;

    private static T GetInstance()
    {
        if (!HasInstance)
        {
            _instance = new T();
            _instance.OnInit();
        }

        return _instance;
    }

    protected virtual void OnInit()
    {
    }

    protected virtual void OnDestroy()
    {
        _instance = null;
    }
}