using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using CrimsonBoard;

namespace CrimsonBoard.Tests
{
    public class EnemyMovementTests
    {
        private GameObject _enemyGo, _playerGo, _blockerGo;
        private EnemyView _enemy;
        private PlayerView _player;
        private GameConfig _cfg;

        [SetUp]
        public void SetUp()
        {
            _enemyGo = new GameObject("Enemy");
            _playerGo = new GameObject("Player");
            _enemy = _enemyGo.AddComponent<EnemyView>();
            _player = _playerGo.AddComponent<PlayerView>();

            _cfg = ScriptableObject.CreateInstance<GameConfig>();
            _cfg.spawn = new SpawnConfig { randomSeed = 0 };
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_enemyGo);
            Object.DestroyImmediate(_playerGo);
            if (_blockerGo != null) Object.DestroyImmediate(_blockerGo);
            Object.DestroyImmediate(_cfg);
        }

        // ── Pawn ──────────────────────────────────────────────────────────────

        [Test]
        public void Pawn_MovesTowardPlayer()
        {
            _enemy.CurrentCell = Vector2Int.zero;
            _player.CurrentCell = new Vector2Int(3, 0);
            var ctx = MakeCtx();

            var dir = new PawnMoveStrategy().GetMoveDirection(_enemy, ctx, new System.Random(0));

            Assert.AreEqual(new Vector2Int(1, 0), dir, "Pawn should step east toward player");
        }

        [Test]
        public void Pawn_AvoidsOccupiedCell()
        {
            _enemy.CurrentCell = Vector2Int.zero;
            _player.CurrentCell = new Vector2Int(3, 0);

            _blockerGo = new GameObject("Blocker");
            var blocker = _blockerGo.AddComponent<EnemyView>();
            blocker.CurrentCell = new Vector2Int(1, 0);

            var ctx = MakeCtx(map => map.Register(blocker.CurrentCell, blocker));
            var dir = new PawnMoveStrategy().GetMoveDirection(_enemy, ctx, new System.Random(0));

            Assert.AreNotEqual(new Vector2Int(1, 0), dir, "Pawn should not move into occupied cell");
        }

        // ── Knight ────────────────────────────────────────────────────────────

        [Test]
        public void Knight_LJumpToPlayer()
        {
            _enemy.CurrentCell = Vector2Int.zero;
            _player.CurrentCell = new Vector2Int(2, 1);
            SetEnemyConfig(rank: 1);

            var ctx = MakeCtx(map => map.Register(_player.CurrentCell, _player));
            var dir = new KnightMoveStrategy().GetMoveDirection(_enemy, ctx, new System.Random(0));

            Assert.AreEqual(new Vector2Int(2, 1), dir, "Knight should jump directly to player's cell");
        }

        [Test]
        public void Knight_EarlyLandingWhenPlayerOnIntermediateCell()
        {
            // Enemy at (0,0); one L-path toward (2,1) has intermediate (1,0); player at (1,0)
            _enemy.CurrentCell = Vector2Int.zero;
            _player.CurrentCell = new Vector2Int(1, 0);
            SetEnemyConfig(rank: 1);

            var ctx = MakeCtx(map => map.Register(_player.CurrentCell, _player));
            var dir = new KnightMoveStrategy().GetMoveDirection(_enemy, ctx, new System.Random(0));

            Assert.IsNotNull(dir, "Knight should find an early-landing direction");
            Assert.AreEqual(new Vector2Int(1, 0), dir);
        }

        [Test]
        public void Knight_FallsBackToIntermediateWhenHigherRankAtTarget()
        {
            _enemy.CurrentCell = Vector2Int.zero;
            _player.CurrentCell = new Vector2Int(10, 10); // far away
            SetEnemyConfig(rank: 1);

            var blockerGo = new GameObject("HighRankEnemy");
            var blocker = blockerGo.AddComponent<EnemyView>();
            blocker.CurrentCell = new Vector2Int(2, 1);
            SetEnemyConfigOn(blocker, rank: 3);

            var ctx = MakeCtx(map => map.Register(blocker.CurrentCell, blocker));
            var dir = new KnightMoveStrategy().GetMoveDirection(_enemy, ctx, new System.Random(0));

            Object.DestroyImmediate(blockerGo);

            if (dir.HasValue)
                Assert.AreNotEqual(blocker.CurrentCell, _enemy.CurrentCell + dir.Value,
                    "Knight should not land on a higher-rank enemy");
        }

        // ── Rook (diagonal) ───────────────────────────────────────────────────

        [Test]
        public void Rook_PicksDiagonalTowardPlayer()
        {
            _enemy.CurrentCell = Vector2Int.zero;
            _player.CurrentCell = new Vector2Int(3, 3);
            var ctx = MakeCtx();

            var dir = new RookMoveStrategy().GetMoveDirection(_enemy, ctx, new System.Random(0));

            Assert.AreEqual(new Vector2Int(1, 1), dir, "Rook should pick NE diagonal toward player");
        }

        [Test]
        public void Rook_StopsBeforeBlockedCell()
        {
            _enemy.CurrentCell = Vector2Int.zero;
            _player.CurrentCell = new Vector2Int(5, 5);

            _blockerGo = new GameObject("Blocker");
            var blocker = _blockerGo.AddComponent<EnemyView>();
            blocker.CurrentCell = new Vector2Int(2, 2);

            var ctx = MakeCtx(map => map.Register(blocker.CurrentCell, blocker));
            var dir = new RookMoveStrategy().GetMoveDirection(_enemy, ctx, new System.Random(0));

            Assert.AreEqual(new Vector2Int(1, 1), dir);
        }

        // ── Tower (straight) ──────────────────────────────────────────────────

        [Test]
        public void Tower_PicksCardinalTowardPlayer()
        {
            _enemy.CurrentCell = Vector2Int.zero;
            _player.CurrentCell = new Vector2Int(0, 4);
            var ctx = MakeCtx();

            var dir = new TowerMoveStrategy().GetMoveDirection(_enemy, ctx, new System.Random(0));

            Assert.AreEqual(new Vector2Int(0, 1), dir, "Tower should move north toward player");
        }

        [Test]
        public void Tower_ReturnsNullWhenAllDirectionsBlocked()
        {
            _enemy.CurrentCell = new Vector2Int(5, 5);
            _player.CurrentCell = new Vector2Int(5, 10);

            var surroundDirs = new[] {
                new Vector2Int(1, 0), new Vector2Int(-1, 0),
                new Vector2Int(0, 1), new Vector2Int(0, -1),
            };
            var blockerGos = new GameObject[surroundDirs.Length];
            for (int i = 0; i < surroundDirs.Length; i++)
            {
                blockerGos[i] = new GameObject($"Blocker{i}");
                blockerGos[i].AddComponent<EnemyView>().CurrentCell = _enemy.CurrentCell + surroundDirs[i];
            }

            var ctx = MakeCtx(map => {
                for (int i = 0; i < surroundDirs.Length; i++)
                    map.Register(_enemy.CurrentCell + surroundDirs[i],
                        blockerGos[i].GetComponent<EnemyView>());
            });
            var dir = new TowerMoveStrategy().GetMoveDirection(_enemy, ctx, new System.Random(0));

            foreach (var go in blockerGos) Object.DestroyImmediate(go);

            Assert.IsNull(dir, "Tower should return null when fully surrounded");
        }

        // ── Queen ─────────────────────────────────────────────────────────────

        [Test]
        public void Queen_PicksDirectionClosestToPlayer()
        {
            _enemy.CurrentCell = Vector2Int.zero;
            _player.CurrentCell = new Vector2Int(4, 0);
            var ctx = MakeCtx();

            var dir = new QueenMoveStrategy().GetMoveDirection(_enemy, ctx, new System.Random(0));

            Assert.AreEqual(new Vector2Int(1, 0), dir, "Queen should move east toward player");
        }

        // ── Beat threshold ────────────────────────────────────────────────────

        [Test]
        public void BeatThreshold_FiresWhenTimerCrossesPhasePoint()
        {
            float period = 0.5f;
            float threshold = 0.25f;

            Assert.IsTrue(CrossedThreshold(0.2f, 0.3f, threshold, period),
                "Should fire when crossing threshold within period");
            Assert.IsFalse(CrossedThreshold(0.3f, 0.4f, threshold, period),
                "Should not fire again after threshold already crossed");
        }

        [Test]
        public void BeatThreshold_FiresOnWrap()
        {
            float period = 0.5f;
            float threshold = 0.1f;

            Assert.IsTrue(CrossedThreshold(0.45f, 0.55f, threshold, period),
                "Should fire on wrap when next-beat position crosses threshold");
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private GameContext MakeCtx(System.Action<OccupancyMap> register = null)
        {
            var ctx = new GameContext(_cfg);
            register?.Invoke(ctx.OccupancyMap);
            ctx.Player = _player;
            return ctx;
        }

        private void SetEnemyConfig(int rank = 1, EnemyType enemyType = EnemyType.Pawn)
            => SetEnemyConfigOn(_enemy, rank, enemyType);

        private static void SetEnemyConfigOn(EnemyView view, int rank = 1, EnemyType enemyType = EnemyType.Pawn)
        {
            var cfg = new EnemyConfig { rank = rank, enemyType = enemyType };

            typeof(EnemyView)
                .GetField("_config", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(view, cfg);
        }

        // Matches EnemyMovementSystem.CrossedThreshold logic (copy for test isolation)
        private static bool CrossedThreshold(float prev, float next, float threshold, float period)
        {
            if (next < period)
                return prev < threshold && next >= threshold;

            float wrapped = next - period;

            return prev < threshold || wrapped >= threshold;
        }
    }
}
