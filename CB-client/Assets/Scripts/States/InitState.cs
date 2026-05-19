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
            // TODO: load resources, init UI, load configs
            _fsm.ChangeState(new TapToStartState(_context, _fsm));
        }

        public void Exit() => Debug.Log("[InitState] Exit");

        public void Tick(float deltaTime) { }
    }
}
