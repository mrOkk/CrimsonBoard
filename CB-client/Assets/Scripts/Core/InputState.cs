using UnityEngine;

namespace CrimsonBoard
{
    public class InputState
    {
        public Vector2Int? MoveCommand { get; set; }
        public bool ShootCommandBuffered { get; set; }
    }
}
