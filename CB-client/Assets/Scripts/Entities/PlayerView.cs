using UnityEngine;

namespace CrimsonBoard
{
    public class PlayerView : EntityView
    {
        [SerializeField] private Transform _weaponLocator;
        [SerializeField] private HealthComponent _health;

        public Transform WeaponLocator => _weaponLocator;
        public HealthComponent Health => _health;
    }
}
