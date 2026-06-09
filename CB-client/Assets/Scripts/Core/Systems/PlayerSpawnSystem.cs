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
            var board = _context.Config.board;
            var spawnCell = new Vector2Int(board.boardSize.x / 2, board.boardSize.y / 2);
            var spawnPos = _context.TileMap.CellToWorld(spawnCell);

            if (_context.Player == null)
            {
                var player = Object.Instantiate(_context.Config.prefabs.playerPrefab, spawnPos, Quaternion.identity);
                player.MeshFilter.sharedMesh = _context.Config.player.mesh;
                _context.Player = player;
                Debug.Log("[PlayerSpawnSystem] Player spawned.");
            }
            else
            {
                _context.TileMap.UnregisterEntity(_context.Player.CurrentCell);
                _context.Player.transform.position = spawnPos;
                Debug.Log("[PlayerSpawnSystem] Player respawned (repositioned).");
            }

            _context.Player.CurrentCell = spawnCell;
            _context.TileMap.RegisterEntity(spawnCell, _context.Player);

            var inventory = new PlayerInventory();

            for (var index = 0; index < _context.Config.player.startingInventory.Length; index++)
            {
                var entry = _context.Config.player.startingInventory[index];
                var weaponCfg = System.Array.Find(_context.Config.weapons, w => w.id == entry.weaponId);

                if (weaponCfg == null)
                {
                    continue;
                }

                inventory.TryAddWeapon(entry.weaponId);
                var ammo = weaponCfg.infiniteAmmo ? int.MaxValue : entry.ammoCount;
                inventory.AddAmmo(entry.weaponId, ammo);
            }

            _context.Inventory = inventory;
        }

        public void Tick(float deltaTime) { }

        public void Dispose() { }
    }
}
