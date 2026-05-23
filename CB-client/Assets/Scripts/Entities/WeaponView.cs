using UnityEngine;

namespace CrimsonBoard
{
    public class WeaponView : MonoBehaviour
    {
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private Transform _playerAttachPoint;
        [SerializeField] private Transform _muzzlePoint;

        public MeshFilter MeshFilter => _meshFilter;
        public Transform PlayerAttachPoint => _playerAttachPoint;
        public Transform MuzzlePoint => _muzzlePoint;
    }
}
