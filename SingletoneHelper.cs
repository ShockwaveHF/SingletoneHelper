using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SingletoneHelper
    {
        private static SingletoneInstance singletoneHelper;

        public static SingletoneInstance Initialize
        {
            get
            {
                if (singletoneHelper == null)
                {
                    singletoneHelper = new SingletoneInstance();
                }
                return singletoneHelper;
            }
        }

        public class SingletoneInstance
        {
            private Dictionary<string, Dictionary<int, object>> _instancesFactory = new Dictionary<string, Dictionary<int, object>>();

            /// <summary>
            /// A singleton doesn't have a specific ID, so use it only if you're sure you won't need other instances.
            /// </summary>
            private Dictionary<Type, object> _instancesSingletone = new Dictionary<Type, object>();

            public delegate void OnInstanceGet<T>(T obj);

            public delegate void OnObjectCreate<T>(T obj);

            public delegate void OnObjectRemoved<T>(T obj);

            public event OnInstanceGet<Type> OnGetInstanceEvent;

            public event OnObjectCreate<Type> OnObjectCreateEvent;

            public event OnObjectRemoved<Type> OnObjectRemovedEvent;

            private void ClearSingletone()
            {
                _instancesSingletone.Clear();
            }

            public void ClearFactory()
            {
                _instancesFactory.Clear();
            }

            public T CreateInstance<T>() where T : new()
            {
                Type type = typeof(T);

                if (_instancesSingletone.ContainsKey(type) == false)
                {
                    ClassMaker<T> maker1 = new ClassMaker<T>();
                    _instancesSingletone[type] = maker1;
                    OnObjectCreateEvent?.Invoke(type);
                }
                OnGetInstanceEvent?.Invoke(type);

                ClassMaker<T> maker = (ClassMaker<T>)_instancesSingletone[type];
                T instance = maker.GetInstance();
                return instance;
            }

            public T CreateInstance<T>(int id) where T : new()
            {
                string typeName = typeof(T).FullName;

                if (!_instancesFactory.ContainsKey(typeName))
                {
                    _instancesFactory[typeName] = new Dictionary<int, object>();
                }

                if (_instancesFactory[typeName].ContainsKey(id))
                {
                    if (_instancesFactory[typeName][id] is T existingInstance)
                    {
                        OnGetInstanceEvent?.Invoke(typeof(T));
                        return existingInstance;
                    }
                }

                T instance = new T();
                _instancesFactory[typeName][id] = instance;
                OnObjectCreateEvent?.Invoke(typeof(T));
                return instance;
            }

            public void RemoveInstance<T>() where T : new()
            {
                Type type = typeof(T);
                if (_instancesSingletone.ContainsKey(type) == true)
                {
                    _instancesSingletone.Remove(type);
                    OnObjectRemovedEvent?.Invoke(type);
                }
            }

            public void RemoveInstance<T>(int id)
            {
                string typeName = typeof(T).FullName;

                if (_instancesFactory.ContainsKey(typeName))
                {
                    if (_instancesFactory[typeName].ContainsKey(id))
                    {
                        Type type = _instancesFactory[typeName][id].GetType();
                        _instancesFactory[typeName].Remove(id);
                        OnObjectRemovedEvent?.Invoke(type);
                    }
                    else
                    {
                        Console.WriteLine($"Object {typeName} | {id} not found.");
                    }
                }
                else
                {
                    Console.WriteLine($"Object {typeName} | {id} not found.");
                }
            }

            private class ClassMaker<T> where T : new()
            {
                private T _instance;

                public T GetInstance()
                {
                    if (_instance == null)
                    {
                        _instance = new T();
                    }

                    return _instance;
                }
            }
        }
    }
}
