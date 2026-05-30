namespace CrimsonBoard
{
    public class GameStats
    {
        public int Score { get; private set; }
        public float ElapsedBattleTime { get; private set; }

        public void Reset()
        {
            Score = 0;
            ElapsedBattleTime = 0f;
        }

        public void AddScore(int amount) => Score += amount;

        public void Tick(float deltaTime) => ElapsedBattleTime += deltaTime;
    }
}
