using UnityEngine;

namespace CrimsonBoard
{
    public class EntityView : MonoBehaviour
    {
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Collider _collider;

        public MeshFilter MeshFilter => _meshFilter;
        public Rigidbody Rigidbody => _rigidbody;
        public Collider Collider => _collider;
    }
}
