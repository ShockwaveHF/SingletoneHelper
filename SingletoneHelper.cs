using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

 public class SingletonHelper
    {
        private static SingletonInstance _instance;
        private static readonly object _lock = new object();

        // Свойство для доступа к менеджеру (Thread-safe)
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

        public class SingletonInstance
        {

            // Хранилище для фабрики: (Тип, ID) -> Объект
            private readonly ConcurrentDictionary<(Type type, int id), object> _factory = new ConcurrentDictionary<(Type, int), object>();
            // Хранилище для одиночек: Тип -> Объект
            private readonly ConcurrentDictionary<Type, object> _singletons = new ConcurrentDictionary<Type, object>();

            internal SingletonInstance() { }

            public event Action<object> OnInstanceGet;

            // Делегаты и события (теперь передаем экземпляр T)
            public event Action<object> OnObjectCreated;
            public event Action<object> OnObjectRemoved;

            public void ClearAll()
            {
                _singletons.Clear();
                _factory.Clear();
            }

            /// <summary>
            /// Создает или возвращает экземпляр типа T по конкретному ID
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
            /// Создает или возвращает единственный экземпляр типа T
            /// </summary>
            public T GetSingleton<T>() where T : class, new()
            {
                Type type = typeof(T);

                // GetOrAdd атомарно проверяет наличие или создает новый объект
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
    }
    }
}
