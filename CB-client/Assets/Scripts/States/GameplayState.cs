using UnityEngine;

namespace CrimsonBoard
{
    public class GameplayState : IGameState
    {
        private readonly GameContext _context;
        private readonly GameStateMachine _fsm;
        private readonly GameplaySystemRunner _systemRunner;

        public GameplayState(GameContext context, GameStateMachine fsm)
        {
            _context = context;
            _fsm = fsm;
            _systemRunner = new GameplaySystemRunner();
            _systemRunner.RegisterSystem(new GameFieldSystem(context));
            _systemRunner.RegisterSystem(new PlayerSpawnSystem(context));
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
