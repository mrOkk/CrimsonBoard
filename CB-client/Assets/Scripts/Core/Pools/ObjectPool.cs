using System.Collections.Generic;
using UnityEngine;

namespace CrimsonBoard
{
    public class ObjectPool<T> where T : MonoBehaviour
    {
        private readonly T _prefab;
        private readonly Transform _container;
        private readonly Queue<T> _available = new Queue<T>();

        public ObjectPool(T prefab, int prewarmCount)
        {
            _prefab = prefab;
            _container = new GameObject($"[Pool] {typeof(T).Name}").transform;
            Prewarm(prewarmCount);
        }

        private T Create()
        {
            var instance = Object.Instantiate(_prefab, _container);
            instance.gameObject.SetActive(false);
            return instance;
        }

        private void Prewarm(int count)
        {
            for (int i = 0; i < count; i++)
                _available.Enqueue(Create());
        }

        public T Get()
        {
            var obj = _available.Count > 0 ? _available.Dequeue() : Create();
            obj.gameObject.SetActive(true);
            return obj;
        }

        public void Return(T obj)
        {
            obj.gameObject.SetActive(false);
            obj.transform.SetParent(_container);
            _available.Enqueue(obj);
        }
    }
}
