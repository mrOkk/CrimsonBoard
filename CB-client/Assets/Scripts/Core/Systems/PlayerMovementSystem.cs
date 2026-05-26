using UnityEngine;
using UnityEngine.InputSystem;

namespace CrimsonBoard
{
    public class PlayerMovementSystem : IGameSystem
    {
        private readonly GameContext _context;
        private readonly GridMovementSystem _gridMovement;
        private InputSystem_Actions _input;
        private float _cooldownRemaining;

        public PlayerMovementSystem(GameContext context, GridMovementSystem gridMovement)
        {
            _context = context;
            _gridMovement = gridMovement;
        }

        public void Initialize()
        {
            _input = new InputSystem_Actions();
            _input.Player.Enable();
            _cooldownRemaining = 0f;
        }

        public void Tick(float deltaTime)
        {
            _cooldownRemaining -= deltaTime;
            if (_cooldownRemaining > 0f) return;

            var raw = _input.Player.Move.ReadValue<Vector2>();
            if (raw.sqrMagnitude < 0.1f) return;

            var dir = RoundToGridDir(raw);
            if (dir == Vector2Int.zero) return;

            var result = _gridMovement.TryMove(_context.Player, dir);
            if (result == MoveResult.Moved)
            {
                var timing = _context.Config.timing;
                _cooldownRemaining = timing.beatDuration / Mathf.Max(1, _context.Config.player.movesPerBeat);

                if (_context.Player.DirectionIndicator != null)
                    _context.Player.DirectionIndicator.rotation =
                        Quaternion.LookRotation(new Vector3(dir.x, 0f, dir.y), Vector3.up);
            }
        }

        public void Dispose()
        {
            _input?.Player.Disable();
            _input?.Dispose();
        }

        // Converts a raw Vector2 into one of the 8 cardinal/diagonal grid directions.
        private static Vector2Int RoundToGridDir(Vector2 raw)
        {
            float angle = Mathf.Atan2(raw.y, raw.x) * Mathf.Rad2Deg;
            int snapped = Mathf.RoundToInt(angle / 45f) * 45;
            float rad = snapped * Mathf.Deg2Rad;
            int x = Mathf.RoundToInt(Mathf.Cos(rad));
            int y = Mathf.RoundToInt(Mathf.Sin(rad));
            return new Vector2Int(x, y);
        }
    }
}
