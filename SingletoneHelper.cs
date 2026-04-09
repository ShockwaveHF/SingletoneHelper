public class SingletonHelper
{
    private static readonly object _lock = new object();
    private static SingletonInstance _instance;

    // Property for accessing the manager (Thread-safe)
    public static SingletonInstance Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance = new SingletonInstance();
                }
            }
            return _instance;
        }
    }
}

public class SingletonInstance
{
    // Factory Storage: (Type, ID) -> Object
    private readonly ConcurrentDictionary<(Type type, int id), object> _factory = new ConcurrentDictionary<(Type, int), object>();

    // Singles Storage: Type -> Object
    private readonly ConcurrentDictionary<Type, object> _singletons = new ConcurrentDictionary<Type, object>();

    internal SingletonInstance()
    { }

    public event global::System.Action<object> OnInstanceGet;
    public event global::System.Action<object> OnObjectCreated;
    public event global::System.Action<object> OnObjectRemoved;

    public void ClearAll()
    {
        _singletons.Clear();
        _factory.Clear();
    }

    /// <summary>
    /// Creates or returns an instance of type T given a specific ID.
    /// </summary>
    public T GetFromFactory<T>(int id) where T : class, new()
    {
        (Type, int id) key = (typeof(T), id);

        T instance = (T)_factory.GetOrAdd(key, k =>
        {
            T newObj = new T();
            OnObjectCreated?.Invoke(newObj);
            return newObj;
        });

        OnInstanceGet?.Invoke(instance);
        return instance;
    }

    /// <summary>
    /// Creates or returns a single instance of type T
    /// </summary>
    public T GetSingleton<T>() where T : class, new()
    {
        Type type = typeof(T);

        T instance = (T)_singletons.GetOrAdd(type, t =>
        {
            T newObj = new T();
            OnObjectCreated?.Invoke(newObj);
            return newObj;
        });

        OnInstanceGet?.Invoke(instance);
        return instance;
    }

    public void RemoveFromFactory<T>(int id) where T : class
    {
        (Type, int id) key = (typeof(T), id);
        if (_factory.TryRemove(key, out object removed))
        {
            OnObjectRemoved?.Invoke(removed);
        }
        else
        {
            Console.WriteLine($"[Factory] Object {typeof(T).Name} with ID {id} not found.");
        }
    }

    public void RemoveSingleton<T>() where T : class
    {
        if (_singletons.TryRemove(typeof(T), out object removed))
        {
            OnObjectRemoved?.Invoke(removed);
        }
    }
}
