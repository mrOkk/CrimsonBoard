namespace CrimsonBoard
{
    [System.Serializable]
    public class SpawnConfig
    {
        public float waveInterval;       // seconds between wave promotions
        public int randomSeed;           // seed for deterministic spawn RNG
        public WaveConfig[] waves;
    }
}
