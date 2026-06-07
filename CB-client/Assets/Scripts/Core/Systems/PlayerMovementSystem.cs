using UnityEngine;

namespace CrimsonBoard
{
    public class PlayerMovementSystem : IGameSystem
    {
        private readonly GameContext _context;
        private readonly GridMovementSystem _gridMovement;
        private float _cooldownRemaining;
        private WeaponPickupSystem _weaponPickup;

        public PlayerMovementSystem(GameContext context, GridMovementSystem gridMovement)
        {
            _context = context;
            _gridMovement = gridMovement;
        }

        public void SetWeaponPickup(WeaponPickupSystem weaponPickup) => _weaponPickup = weaponPickup;

        public void Initialize()
        {
            _cooldownRemaining = 0f;
        }

        public void Tick(float deltaTime)
        {
            _cooldownRemaining -= deltaTime;
            if (_cooldownRemaining > 0f) return;

            var cmd = _context.InputState.MoveCommand;
            if (cmd == null) return;

            var result = _gridMovement.TryMove(_context.Player, cmd.Value);
            if (result == MoveResult.Moved)
            {
                var timing = _context.Config.timing;
                _cooldownRemaining = timing.beatDuration / Mathf.Max(1, _context.Config.player.movesPerBeat);

                if (_context.Player.DirectionIndicator != null)
                    _context.Player.DirectionIndicator.rotation =
                        Quaternion.LookRotation(new Vector3(cmd.Value.x, 0f, cmd.Value.y), Vector3.up);

                _weaponPickup?.TryPickupAt(_context.Player.CurrentCell);
            }
        }

        public void Dispose() { }
    }
}
