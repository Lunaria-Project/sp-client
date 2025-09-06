using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
{
    public static T Instance => GetInstance();
    public static bool HasInstance => _instance && !_isDestroyed;

    private static T _instance;
    private static readonly object _lock = new();
    private static bool _isDestroyed;

    private static T GetInstance()
    {
        if (_isDestroyed)
        {
            Debug.Log($"Singleton {typeof(T)} is Destroyed");
            return null;
        }

        lock (_lock)
        {
            if (!_instance)
            {
                var singletonObject = new GameObject(typeof(T).Name);
                _instance = singletonObject.AddComponent<T>();
                DontDestroyOnLoad(singletonObject);
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (!_instance)
        {
            _instance = (T)this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Debug.LogErrorFormat("Instance already exists {0}", this);
            Destroy(gameObject);
        }
    }

    private void OnApplicationQuit()
    {
        _isDestroyed = true;
    }

    private void OnDestroy()
    {
        _instance = null;
    }
}