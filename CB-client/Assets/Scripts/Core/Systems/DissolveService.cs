using System;
using System.Collections.Generic;
using UnityEngine;

namespace CrimsonBoard
{
    public static class DissolveService
    {
        /// <summary>
        /// Unregisters the enemy from the occupancy map, disables its collider,
        /// plays the dissolve animation and returns the enemy to the pool on completion.
        /// </summary>
        public static void DissolveAndReturn(
            EnemyView enemy,
            Vector2Int cell,
            TileMap tileMap,
            GamePools pools,
            Action onComplete = null)
        {
            tileMap.UnregisterEntity(cell);
            enemy.Collider.enabled = false;
            enemy.Dissolve.Play(() =>
            {
                enemy.Collider.enabled = true;
                pools.Enemies.Return(enemy);
                onComplete?.Invoke();
            });
        }

        /// <summary>
        /// Dissolves all enemies in the list and returns each to the pool.
        /// <paramref name="onAllComplete"/> is invoked once the last dissolve finishes.
        /// Safe to call with an empty list — <paramref name="onAllComplete"/> is invoked immediately.
        /// </summary>
        public static void DissolveAllAndReturn(
            IReadOnlyList<EnemyView> enemies,
            TileMap tileMap,
            GamePools pools,
            Action onAllComplete = null)
        {
            if (enemies.Count == 0)
            {
                onAllComplete?.Invoke();
                return;
            }

            int remaining = enemies.Count;

            // Snapshot cells before any mutations
            var cells = new Vector2Int[enemies.Count];
            for (int i = 0; i < enemies.Count; i++)
                cells[i] = enemies[i].CurrentCell;

            for (int i = 0; i < enemies.Count; i++)
            {
                var enemy = enemies[i];
                var cell = cells[i];
                DissolveAndReturn(enemy, cell, tileMap, pools, () =>
                {
                    if (--remaining == 0)
                        onAllComplete?.Invoke();
                });
            }
        }
    }
}
