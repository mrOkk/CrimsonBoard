using System;
using System.Collections.Generic;
using UnityEngine;

namespace CrimsonBoard
{
    public class UiRoot : MonoBehaviour
    {
        private readonly Dictionary<Type, BaseView> _views = new();
        private readonly List<BaseView> _openViews = new();

        public void Init()
        {
            _views.Clear();
            _openViews.Clear();
            foreach (var view in GetComponentsInChildren<BaseView>(true))
            {
                _views[view.GetType()] = view;
                view.Hide();
            }
        }

        public void Show<T>() where T : BaseView
        {
            if (_views.TryGetValue(typeof(T), out var view))
            {
                if (!_openViews.Contains(view))
                    _openViews.Add(view);
                view.Show();
            }
            else
                Debug.LogWarning($"[UiRoot] View of type {typeof(T).Name} is not registered.");
        }

        public void Hide<T>() where T : BaseView
        {
            if (_views.TryGetValue(typeof(T), out var view))
            {
                if (_openViews.Remove(view))
                    view.Hide();
            }
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
            foreach (var view in _openViews)
                view.Tick(deltaTime);
        }
    }
}
