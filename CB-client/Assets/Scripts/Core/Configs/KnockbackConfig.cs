using System;

namespace CrimsonBoard
{
    [Serializable]
    public class KnockbackConfig
    {
        public float initialSpeed = 8f;
        public float friction = 12f;
        public float playerInfluence = 4f;
    }
}
