using UnityEngine;

namespace CrimsonBoard
{
    public class WeaponView : MonoBehaviour
    {
        [SerializeField] private Transform _playerAttachPoint;
        [SerializeField] private Transform _muzzlePoint;
        [SerializeField] private Collider _pickupCollider;
        [SerializeField] private Transform _rotationPoint;
        [SerializeField] private float _hoverHeight = 1;

        public Transform PlayerAttachPoint => _playerAttachPoint;
        public Transform MuzzlePoint => _muzzlePoint;
        public int WeaponId { get; private set; }
        public Transform RotationPoint => _rotationPoint;
        public float HoverHeight => _hoverHeight;
        public Vector2Int CurrentCell { get; set; }

        public System.Action<WeaponView, Collider> TriggerEntered;

        public void SetWeaponId(int id) => WeaponId = id;

        public void SetDroppedMode(Vector2Int cell, BoardConfig config)
        {
            CurrentCell = cell;
            var worldPos = ChunkCoordConverter.TileToWorld(cell, config);
            transform.position = worldPos + Vector3.up * _hoverHeight;
            if (_pickupCollider != null)
            {
                _pickupCollider.isTrigger = true;
                _pickupCollider.enabled = false;
            }
        }

        public void SetEquippedMode()
        {
            if (_pickupCollider != null)
                _pickupCollider.enabled = false;
            TriggerEntered = null;
        }

        private void OnTriggerEnter(Collider other) => TriggerEntered?.Invoke(this, other);
    }
}
