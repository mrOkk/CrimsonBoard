using UnityEngine;

namespace CrimsonBoard
{
    public class TapToStartState : IGameState
    {
        private readonly GameContext _context;
        private readonly GameStateMachine _fsm;

        public TapToStartState(GameContext context, GameStateMachine fsm)
        {
            _context = context;
            _fsm = fsm;
        }

        public void Enter() => Debug.Log("[TapToStartState] Enter");
        public void Exit() => Debug.Log("[TapToStartState] Exit");

        public void Tick(float deltaTime)
        {
            // TODO: detect tap/click and transition to GameplayState
        }
    }
}
