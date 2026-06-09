namespace CrimsonBoard
{
    [System.Serializable]
    public class SpawnConfig
    {
        public float waveInterval;       // seconds between wave promotions
        public int randomSeed;           // seed for deterministic spawn RNG
        public float minDistanceFromPlayer = 2; // minimum spawn distance from player (in world units)
        public WaveConfig[] waves;
    }
}
