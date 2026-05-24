using UnityEngine;

namespace CrimsonBoard
{
    public class PlayerView : EntityView
    {
        [SerializeField] private Transform _weaponLocator;
        [SerializeField] private HealthComponent _health;
        [SerializeField] private Transform _directionIndicator;

        public Transform WeaponLocator => _weaponLocator;
        public HealthComponent Health => _health;
        public Transform DirectionIndicator => _directionIndicator;
    }
}
