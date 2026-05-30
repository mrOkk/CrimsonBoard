using UnityEngine;

namespace CrimsonBoard
{
    public abstract class BaseView : MonoBehaviour
    {
        public virtual void Show() => gameObject.SetActive(true);
        public virtual void Hide() => gameObject.SetActive(false);
        public virtual void Tick(float deltaTime) { }
    }
}
