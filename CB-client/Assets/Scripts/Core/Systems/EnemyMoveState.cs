namespace CrimsonBoard
{
    public struct EnemyMoveState
    {
        public float phaseOffset;      // 0..1 — when in the beat cycle to fire
        public float phaseTimer;       // current position in beat cycle (seconds)
        public int cooldownTicksLeft;  // beats remaining before allowed to move
    }
}
