using UnityEngine;
using UnityEngine.InputSystem;

namespace CrimsonBoard
{
    public class PlayerInputSystem : IGameSystem
    {
        private readonly GameContext _context;
        private InputSystem_Actions _input;

        private bool _wasActive;
        private bool _isDelaying;
        private float _delayTimer;
        private Vector2Int _lastDir;
        private float _moveBufferTimer;

        private float _shootBufferTimer;

        public PlayerInputSystem(GameContext context)
        {
            _context = context;
        }

        public void Initialize()
        {
            _input = new InputSystem_Actions();
            _input.Player.Enable();
        }

        public void Tick(float deltaTime)
        {
            TickMovement(deltaTime);
            TickShoot(deltaTime);
        }

        public void Dispose()
        {
            _input?.Player.Disable();
            _input?.Dispose();
        }

        private void TickMovement(float deltaTime)
        {
            var raw = _input.Player.Move.ReadValue<Vector2>();
            bool isActive = raw.sqrMagnitude >= 0.1f;
            var rawDir = isActive ? RoundToGridDir(raw) : Vector2Int.zero;
            isActive = rawDir != Vector2Int.zero;

            if (isActive)
            {
                _lastDir = rawDir;
                _moveBufferTimer = 0f;
                _context.InputState.IsKeysHeld = true;

                if (!_wasActive)
                {
                    // Fresh press — skip delay if there's already a buffered command (re-press mid-buffer)
                    if (_context.InputState.MoveCommand == null)
                    {
                        _isDelaying = true;
                        _delayTimer = 0f;
                    }
                    else
                    {
                        _isDelaying = false;
                    }
                }

                if (_isDelaying)
                {
                    _delayTimer += deltaTime;
                    if (_delayTimer >= _context.Config.player.movementInputDelay)
                        _isDelaying = false;
                }

                _context.InputState.MoveCommand = _isDelaying ? (Vector2Int?)null : _lastDir;
            }
            else
            {
                _context.InputState.IsKeysHeld = false;
                if (_wasActive)
                {
                    // Just released — emit immediately and start buffer
                    _isDelaying = false;
                    _moveBufferTimer = _context.Config.player.inputBufferWindow;
                    _context.InputState.MoveCommand = _lastDir;
                }
                else if (_moveBufferTimer > 0f)
                {
                    _moveBufferTimer -= deltaTime;
                    _context.InputState.MoveCommand = _moveBufferTimer > 0f ? _lastDir : (Vector2Int?)null;
                }
                else
                {
                    _context.InputState.MoveCommand = null;
                }
            }

            _wasActive = isActive;
        }

        private void TickShoot(float deltaTime)
        {
            if (_input.Player.Next.WasPressedThisFrame())
            {
                _context.Inventory.CycleNext();
            }
            else if (_input.Player.Previous.WasPressedThisFrame())
            {
                _context.Inventory.CyclePrevious();
            }

            if (_shootBufferTimer > 0f)
            {
                _shootBufferTimer -= deltaTime;
                if (_shootBufferTimer <= 0f)
                    _context.InputState.ShootCommandBuffered = false;
            }
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
