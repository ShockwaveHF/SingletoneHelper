using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class SingletoneHelper
    {
        private Dictionary<Type, object> _classMakers = new Dictionary<Type, object>();

        public delegate void OnInstanceGet<T>(T obj);

        public delegate void OnObjectCreate<T>(T obj);

        public delegate void OnObjectRemoved<T>(T obj);

        public event OnInstanceGet<Type> OnGetInstanceEvent;

        public event OnObjectCreate<Type> OnObjectCreateEvent;

        public event OnObjectRemoved<Type> OnObjectRemovedEvent;

        public ClassMaker<T> GetClassMaker<T>() where T : new()
        {
            Type type = typeof(T);

            if (_classMakers.ContainsKey(type) == false)
            {
                ClassMaker<T> maker = new ClassMaker<T>();
                _classMakers[type] = maker;
                OnObjectCreateEvent?.Invoke(type);
            }
            OnGetInstanceEvent?.Invoke(type);
            return (ClassMaker<T>)_classMakers[type];
        }

        public T GetInstance<T>() where T : new()
        {
            ClassMaker<T> maker = GetClassMaker<T>();
            T instance = maker.GetInstance();
            return instance;
        }

        public void RemoveClass<T>() where T : new()
        {
            Type type = typeof(T);
            if (_classMakers.ContainsKey(type) == true)
            {
                _classMakers.Remove(type);
                OnObjectRemovedEvent?.Invoke(type);
            }
        }

        public class ClassMaker<T> where T : new()
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
