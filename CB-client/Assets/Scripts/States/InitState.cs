using UnityEngine;

namespace CrimsonBoard
{
    public class InitState : IGameState
    {
        private readonly GameContext _context;
        private readonly GameStateMachine _fsm;

        public InitState(GameContext context, GameStateMachine fsm)
        {
            _context = context;
            _fsm = fsm;
        }

        public void Enter()
        {
            Debug.Log("[InitState] Enter");
            _context.Pools = new GamePools(_context.Config.prefabs);
            Debug.Log("[InitState] Pools initialized.");
            _fsm.ChangeState(new TapToStartState(_context, _fsm));
        }

        public void Exit() => Debug.Log("[InitState] Exit");

        public void Tick(float deltaTime) { }
    }
}
