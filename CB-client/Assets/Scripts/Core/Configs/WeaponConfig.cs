using UnityEngine;

namespace CrimsonBoard
{
    [System.Serializable]
    public class WeaponConfig
    {
        public Mesh[] meshVariants;
        public GameObject prefab;
        public int damage;
        public float shotsPerBeat;
        public float spread;
        public float rotationSpeed;
        public float moveSpeedCoefficient;
    }
}
