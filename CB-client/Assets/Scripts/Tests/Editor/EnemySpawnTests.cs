using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using CrimsonBoard;

namespace CrimsonBoard.Tests
{
    public class EnemySpawnTests
    {
        // ── Border tiles ────────────────────────────────────────────────────

        [Test]
        public void BorderTiles_WindowRadius1_ChunkSize2_Returns24Tiles()
        {
            var tiles = GameFieldSystem.ComputeBorderTiles(Vector2Int.zero, windowRadius: 1, chunkSize: 2);
            Assert.AreEqual(24, tiles.Count);
        }

        [Test]
        public void BorderTiles_AllOnPerimeter()
        {
            // r=1, cs=2 → tile range x:[-2..3], y:[-2..3]
            var tiles = GameFieldSystem.ComputeBorderTiles(Vector2Int.zero, windowRadius: 1, chunkSize: 2);
            foreach (var t in tiles)
                Assert.IsTrue(t.x == -2 || t.x == 3 || t.y == -2 || t.y == 3,
                    $"Tile {t} is not on perimeter");
        }

        [Test]
        public void BorderTiles_NonZeroCenter_CorrectRange()
        {
            // center=(1,1), r=1, cs=4 → tile range x:[-4..11], y:[-4..11]... wait
            // center=(1,1): minX=(1-1)*4=0, maxX=(1+1+1)*4-1=11, minY=0, maxY=11
            var tiles = GameFieldSystem.ComputeBorderTiles(new Vector2Int(1, 1), windowRadius: 1, chunkSize: 4);
            foreach (var t in tiles)
                Assert.IsTrue(t.x == 0 || t.x == 11 || t.y == 0 || t.y == 11,
                    $"Tile {t} is not on perimeter");
        }

        [Test]
        public void BorderTiles_NoDuplicates()
        {
            var tiles = GameFieldSystem.ComputeBorderTiles(Vector2Int.zero, windowRadius: 1, chunkSize: 3);
            var set = new HashSet<Vector2Int>(tiles);
            Assert.AreEqual(tiles.Count, set.Count, "Duplicate border tiles detected");
        }

        // ── Weighted enemy pick ─────────────────────────────────────────────

        [Test]
        public void WeightedPick_HeavierEntryPickedMoreOften()
        {
            var entries = new[]
            {
                new EnemySpawnEntry { enemyId = 1, weight = 1f },
                new EnemySpawnEntry { enemyId = 2, weight = 3f },
            };
            var rng = new System.Random(42);
            int count2 = 0;
            for (int i = 0; i < 1000; i++)
                if (EnemySpawnSystem.PickEnemyId(entries, rng) == 2) count2++;
            // Expect ~75% ± 10%
            Assert.That(count2, Is.InRange(650, 850),
                $"Expected ~750 picks of id=2, got {count2}");
        }

        [Test]
        public void WeightedPick_SingleEntry_AlwaysReturnsThatId()
        {
            var entries = new[] { new EnemySpawnEntry { enemyId = 7, weight = 1f } };
            var rng = new System.Random(0);
            for (int i = 0; i < 100; i++)
                Assert.AreEqual(7, EnemySpawnSystem.PickEnemyId(entries, rng));
        }

        [Test]
        public void WeightedPick_Deterministic_SameSeedSameSequence()
        {
            var entries = new[]
            {
                new EnemySpawnEntry { enemyId = 1, weight = 1f },
                new EnemySpawnEntry { enemyId = 2, weight = 1f },
            };
            var rng1 = new System.Random(12345);
            var rng2 = new System.Random(12345);
            for (int i = 0; i < 50; i++)
                Assert.AreEqual(
                    EnemySpawnSystem.PickEnemyId(entries, rng1),
                    EnemySpawnSystem.PickEnemyId(entries, rng2));
        }

        // ── Wave timer ──────────────────────────────────────────────────────

        [Test]
        public void WaveTimer_AdvancesAfterInterval()
        {
            float waveInterval = 30f;
            float timer = 0f;
            int waveIndex = 0;
            const int maxWave = 2;
            float dt = 10f;

            // Tick 4 times (total 40s > 30s) → wave should advance once
            for (int i = 0; i < 4; i++)
            {
                timer += dt;
                if (waveIndex < maxWave && timer >= waveInterval)
                {
                    timer -= waveInterval;
                    waveIndex++;
                }
            }

            Assert.AreEqual(1, waveIndex);
            Assert.AreEqual(10f, timer, 0.001f);
        }

        [Test]
        public void WaveTimer_ClampsAtLastWave()
        {
            float waveInterval = 10f;
            float timer = 0f;
            int waveIndex = 0;
            const int maxWave = 1; // only 2 waves (index 0 and 1)
            float dt = 15f;

            // Tick many times — wave must not exceed maxWave
            for (int i = 0; i < 10; i++)
            {
                timer += dt;
                if (waveIndex < maxWave && timer >= waveInterval)
                {
                    timer -= waveInterval;
                    waveIndex++;
                }
            }

            Assert.AreEqual(maxWave, waveIndex, "Wave index exceeded maximum");
        }
    }
}
