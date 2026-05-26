namespace CrimsonBoard
{
    public enum MoveResult
    {
        Moved,    // entity moved successfully
        Blocked,  // target cell is occupied by a non-combat entity
        Combat    // enemy entered player cell — HealthSystem was notified
    }
}
