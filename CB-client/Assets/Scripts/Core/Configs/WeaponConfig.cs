using UnityEngine;

namespace CrimsonBoard
{
    [System.Serializable]
    public class WeaponConfig
    {
        public int id;
        public Mesh[] meshVariants;
        public int damage;
        public float shotsPerBeat;
        public float spread;
        public float rotationSpeed;
        public float moveSpeedCoefficient;
    }
}
