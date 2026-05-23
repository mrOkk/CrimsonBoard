using UnityEngine;

namespace CrimsonBoard
{
    public class PlayerView : EntityView
    {
        [SerializeField] private Transform _weaponLocator;

        public Transform WeaponLocator => _weaponLocator;
    }
}
