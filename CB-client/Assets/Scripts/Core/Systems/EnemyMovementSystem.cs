using System.Collections.Generic;
using UnityEngine;

namespace CrimsonBoard
{
    public class EnemyMovementSystem : IGameSystem
    {
        private readonly GameContext _context;
        private readonly GridMovementSystem _gridMovement;
        private readonly Dictionary<EnemyView, EnemyMoveState> _states
            = new Dictionary<EnemyView, EnemyMoveState>();
        private readonly Dictionary<EnemyType, IMoveStrategy> _strategies;

        private float _beatTimer;
        private readonly List<EnemyView> _tempKeys = new();

        public EnemyMovementSystem(GameContext context, GridMovementSystem gridMovement)
        {
            _context = context;
            _gridMovement = gridMovement;
            _strategies = new Dictionary<EnemyType, IMoveStrategy>
            {
                { EnemyType.Pawn,   new PawnMoveStrategy()   },
                { EnemyType.Knight, new KnightMoveStrategy() },
                { EnemyType.Rook,   new RookMoveStrategy()   },
                { EnemyType.Tower,  new TowerMoveStrategy()  },
                { EnemyType.Queen,  new QueenMoveStrategy()  },
            };
        }

        public void Initialize() { _beatTimer = 0f; }

        public void Tick(float deltaTime)
        {
            float beatDuration = _context.Config.timing.beatDuration;
            float prevTimer = _beatTimer;
            _beatTimer += deltaTime;

            _tempKeys.Clear();
            _tempKeys.AddRange(_states.Keys);

            for (var index = 0; index < _tempKeys.Count; index++)
            {
                var enemy = _tempKeys[index];

                if (!_states.TryGetValue(enemy, out var state)) continue;

                float triggerTime = state.phaseOffset * beatDuration;

                if (!CrossedThreshold(prevTimer, _beatTimer, triggerTime, beatDuration)) continue;

                if (state.cooldownTicksLeft > 0)
                {
                    state.cooldownTicksLeft--;
                    _states[enemy] = state;

                    continue;
                }

                if (!_strategies.TryGetValue(enemy.Config.enemyType, out var strategy)) continue;

                var dir = strategy.GetMoveDirection(enemy, _context, _context.SharedRandom);

                if (dir.HasValue)
                {
                    _gridMovement.TryMove(enemy, dir.Value);
                    state.cooldownTicksLeft = enemy.Config.moveCooldownTicks;
                    _states[enemy] = state;
                }
            }

            if (_beatTimer >= beatDuration) _beatTimer -= beatDuration;
        }

        public void Dispose() => _states.Clear();

        // ── Public callbacks ────────────────────────────────────────────────

        public void OnEnemySpawned(EnemyView enemy)
        {
            _states[enemy] = new EnemyMoveState
            {
                phaseOffset = (float)_context.SharedRandom.NextDouble(),
                phaseTimer = 0f,
                cooldownTicksLeft = 0,
            };
        }

        public void OnEnemyDied(EnemyView enemy) => _states.Remove(enemy);

        // ── Private helpers ─────────────────────────────────────────────────

        /// <summary>
        /// Returns true if <paramref name="threshold"/> was crossed in (prev, next]
        /// accounting for wrapping at <paramref name="period"/>.
        /// </summary>
        private static bool CrossedThreshold(float prev, float next, float threshold, float period)
        {
            if (next < period)
            {
                return prev < threshold && next >= threshold;
            }

            // Wrapped around
            float wrapped = next - period;
            return prev < threshold || wrapped >= threshold;
        }
    }
}
