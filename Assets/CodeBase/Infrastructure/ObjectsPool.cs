using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace CodeBase.Infrastructure
{
    public class ObjectsPool<T> where T : class
    {
        private List<T> _objects;
        private readonly Func<T> _onCreate;
        private readonly Action<T> _onGet;
        private readonly Action<T> _onRelease;

        public ObjectsPool(Func<T> onCreate, Action<T> onGet, Action<T> onRelease)
        {
            _onCreate = onCreate;
            _onGet = onGet;
            _onRelease = onRelease;
        }

        public T TryGet([CanBeNull] Predicate<T> predicate)
        {
            int index = _objects.FindIndex(obj => predicate?.Invoke(obj) == true);

            T obj = index == -1 ? _onCreate() : _objects[index];

            if(index != -1)
                _objects.RemoveAt(index);
            
            _onGet(obj);
            return obj;
        }
        
        public void Release(T element) 
        {
            _onRelease.Invoke(element);
            _objects.Add(element);
        }
    }
}