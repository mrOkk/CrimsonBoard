using System;
using System.Collections.Generic;
using UnityEngine;

namespace CrimsonBoard
{
    public class UiRoot : MonoBehaviour
    {
        private readonly Dictionary<Type, BaseView> _views = new();

        public void Init()
        {
            _views.Clear();
            foreach (var view in GetComponentsInChildren<BaseView>(true))
                _views[view.GetType()] = view;
        }

        public void Show<T>() where T : BaseView
        {
            if (_views.TryGetValue(typeof(T), out var view))
                view.Show();
            else
                Debug.LogWarning($"[UiRoot] View of type {typeof(T).Name} is not registered.");
        }

        public void Hide<T>() where T : BaseView
        {
            if (_views.TryGetValue(typeof(T), out var view))
                view.Hide();
            else
                Debug.LogWarning($"[UiRoot] View of type {typeof(T).Name} is not registered.");
        }

        public T GetView<T>() where T : BaseView
        {
            if (_views.TryGetValue(typeof(T), out var view))
                return (T)view;

            Debug.LogWarning($"[UiRoot] View of type {typeof(T).Name} is not registered.");
            return null;
        }

        public void Tick(float deltaTime)
        {
            foreach (var view in _views.Values)
                view.Tick(deltaTime);
        }
    }
}
