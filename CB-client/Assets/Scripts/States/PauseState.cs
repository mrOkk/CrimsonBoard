using UnityEngine;

namespace CrimsonBoard
{
    public class PauseState : IGameState
    {
        private readonly GameContext _context;
        private readonly GameStateMachine _fsm;

        public PauseState(GameContext context, GameStateMachine fsm)
        {
            _context = context;
            _fsm = fsm;
        }

        public void Enter() => Debug.Log("[PauseState] Enter");
        public void Exit() => Debug.Log("[PauseState] Exit");

        public void Tick(float deltaTime)
        {
            // TODO: handle settings changes, resume input
        }
    }
}
