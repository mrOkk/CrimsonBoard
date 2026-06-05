using UnityEngine;

namespace CrimsonBoard
{
    public class HitEmitter : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _particleSystem;

        public void Emit(Vector3 position)
        {
            if (_particleSystem == null) return;
            _particleSystem.transform.position = position;
            _particleSystem.Emit(1);
        }
    }
}
