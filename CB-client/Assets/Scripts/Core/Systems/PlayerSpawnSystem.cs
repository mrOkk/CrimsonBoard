using UnityEngine;

namespace CrimsonBoard
{
    public class PlayerSpawnSystem : IGameSystem
    {
        private readonly GameContext _context;

        public PlayerSpawnSystem(GameContext context)
        {
            _context = context;
        }

        public void Initialize()
        {
            var spawnPos = GetSpawnPosition();
            if (_context.Player == null)
            {
                var player = Object.Instantiate(_context.Config.prefabs.playerPrefab, spawnPos, Quaternion.identity);
                player.MeshFilter.sharedMesh = _context.Config.player.mesh;
                _context.Player = player;
                Debug.Log("[PlayerSpawnSystem] Player spawned.");
            }
            else
            {
                _context.OccupancyMap.Unregister(_context.Player.CurrentCell);
                _context.Player.transform.position = spawnPos;
                Debug.Log("[PlayerSpawnSystem] Player respawned (repositioned).");
            }

            var spawnCell = ChunkCoordConverter.WorldToTile(spawnPos, _context.Config.board);
            _context.Player.CurrentCell = spawnCell;
            _context.OccupancyMap.Register(spawnCell, _context.Player);

            var inventory = new PlayerInventory();
            foreach (var entry in _context.Config.player.startingInventory)
            {
                var weaponCfg = System.Array.Find(_context.Config.weapons, w => w.id == entry.weaponId);
                if (weaponCfg == null) continue;
                inventory.TryAddWeapon(entry.weaponId);
                int ammo = weaponCfg.infiniteAmmo ? int.MaxValue : entry.ammoCount;
                inventory.AddAmmo(entry.weaponId, ammo);
            }
            _context.Inventory = inventory;
        }

        public void Tick(float deltaTime) { }

        public void Dispose() { }

        private Vector3 GetSpawnPosition()
        {
            var board = _context.Config.board;
            return new Vector3(
                board.chunkSize * board.tileSize.x / 2f,
                0f,
                board.chunkSize * board.tileSize.y / 2f
            );
        }
    }
}
