using UnityEngine;

namespace CrimsonBoard
{
    public class GameplayState : IGameState
    {
        private readonly GameContext _context;
        private readonly GameStateMachine _fsm;
        private readonly GameplaySystemRunner _systemRunner;
        private readonly GameFieldSystem _gameFieldSystem;
        private GridMovementSystem _gridMovementSystem;
        private HealthSystem _healthSystem;
        private EnemySpawnSystem _enemySpawnSystem;
        private EnemyMovementSystem _enemyMovementSystem;
        private WeaponPickupSystem _weaponPickupSystem;
        private WeaponUsageSystem _weaponUsageSystem;

        public HealthSystem HealthSystem => _healthSystem;
        public GridMovementSystem GridMovementSystem => _gridMovementSystem;
        public WeaponPickupSystem WeaponPickupSystem => _weaponPickupSystem;

        public GameplayState(GameContext context, GameStateMachine fsm, GameFieldSystem gameFieldSystem)
        {
            _context = context;
            _fsm = fsm;
            _systemRunner = new GameplaySystemRunner();
            _gameFieldSystem = gameFieldSystem;

            _gridMovementSystem = new GridMovementSystem(context);

            // Field and player are already initialized in TapToStartState; reuse existing systems.
            _systemRunner.RegisterSystem(new PlayerInputSystem(context));
            _systemRunner.RegisterSystem(_gameFieldSystem);
            _systemRunner.RegisterSystem(new CameraFollowSystem(context));
            _healthSystem = new HealthSystem(context, fsm);
            _gridMovementSystem.HealthSystem = _healthSystem;
            _systemRunner.RegisterSystem(_healthSystem);
            _systemRunner.RegisterSystem(_gridMovementSystem);
            var playerMovement = new PlayerMovementSystem(context, _gridMovementSystem);
            _systemRunner.RegisterSystem(playerMovement);
            _enemySpawnSystem = new EnemySpawnSystem(context, _gameFieldSystem, _healthSystem);
            _systemRunner.RegisterSystem(new HopAnimationSystem(context, context.Board));
            _enemyMovementSystem = new EnemyMovementSystem(context, _gridMovementSystem);
            _systemRunner.RegisterSystem(_enemyMovementSystem);
            _healthSystem.EnemyDeathCallback += _enemySpawnSystem.OnEnemyDied;
            _healthSystem.EnemyDeathCallback += _enemyMovementSystem.OnEnemyDied;
            _healthSystem.EnemyDeathCallback += _ => _context.Stats.AddScore(1);
            _enemySpawnSystem.EnemySpawned += _enemyMovementSystem.OnEnemySpawned;
            _systemRunner.RegisterSystem(_enemySpawnSystem);
            _weaponPickupSystem = new WeaponPickupSystem(context);
            playerMovement.SetWeaponPickup(_weaponPickupSystem);
            _systemRunner.RegisterSystem(_weaponPickupSystem);
            _weaponUsageSystem = new WeaponUsageSystem(context);
            _systemRunner.RegisterSystem(_weaponUsageSystem);
        }

        public void Enter()
        {
            Debug.Log("[GameplayState] Enter");
            _context.Stats.Reset();
            var hud = _context.UiRoot.GetView<HudView>();
            hud.OnMenuRequested = () => _fsm.RequestPause(new PauseState(_context, _fsm));
            _context.UiRoot.Show<HudView>();
            _systemRunner.Initialize();
        }

        public void Exit()
        {
            Debug.Log("[GameplayState] Exit");
            _context.UiRoot.Hide<HudView>();
            _systemRunner.Dispose();
        }

        public void Tick(float deltaTime)
        {
            _context.Stats.Tick(deltaTime);
            _systemRunner.Tick(deltaTime);
        }
    }
}
