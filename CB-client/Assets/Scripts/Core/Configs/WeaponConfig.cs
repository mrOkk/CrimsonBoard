using UnityEngine;

namespace CrimsonBoard
{
    [System.Serializable]
    public class WeaponConfig
    {
        public int id;
        public string name;
        public int damage;
        public float shotsPerBeat;
        public float spread;
        public float rotationSpeed;
        public bool infiniteAmmo;
        public int ammoOnPickup;
        [Range(0f, 1f)] public float dropChance;
        public float range = 10f;
        public float holsterTime = 0.3f;
        public float drawTime = 0.3f;
        public int maxTargetsPerBullet = 1;
    }
}
