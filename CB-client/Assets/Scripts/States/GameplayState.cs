using UnityEngine;

namespace CrimsonBoard
{
    public class GameplayState : IGameState
    {
        private readonly GameContext _context;
        private readonly GameStateMachine _fsm;
        private readonly GameplaySystemRunner _systemRunner;
        private GridMovementSystem _gridMovementSystem;
        private HealthSystem _healthSystem;

        public HealthSystem HealthSystem => _healthSystem;
        public GridMovementSystem GridMovementSystem => _gridMovementSystem;

        public GameplayState(GameContext context, GameStateMachine fsm)
        {
            _context = context;
            _fsm = fsm;
            _systemRunner = new GameplaySystemRunner();

            _gridMovementSystem = new GridMovementSystem(context);

            // Field and player are already initialized in TapToStartState; reuse existing systems.
            _systemRunner.RegisterSystem(context.GameFieldSystem);
            _systemRunner.RegisterSystem(new CameraFollowSystem(context));
            _healthSystem = new HealthSystem(context, fsm);
            _gridMovementSystem.HealthSystem = _healthSystem;
            _systemRunner.RegisterSystem(_healthSystem);
            _systemRunner.RegisterSystem(_gridMovementSystem);
            _systemRunner.RegisterSystem(new PlayerMovementSystem(context, _gridMovementSystem));
        }

        public void Enter()
        {
            Debug.Log("[GameplayState] Enter");
            _systemRunner.Initialize();
        }

        public void Exit()
        {
            Debug.Log("[GameplayState] Exit");
            _systemRunner.Dispose();
        }

        public void Tick(float deltaTime) => _systemRunner.Tick(deltaTime);
    }
}
