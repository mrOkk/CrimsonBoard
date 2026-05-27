using UnityEngine;

namespace CrimsonBoard
{
    /// <summary>
    /// Returns a direction vector (not an absolute cell) to pass to GridMovementSystem.TryMove.
    /// Returns null if no valid move exists.
    /// </summary>
    public interface IMoveStrategy
    {
        Vector2Int? GetMoveDirection(EnemyView enemy, GameContext context, System.Random rng);
    }
}
