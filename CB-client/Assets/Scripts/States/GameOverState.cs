using UnityEngine;

namespace CrimsonBoard
{
    public class GameOverState : IGameState
    {
        private readonly GameContext _context;
        private readonly GameStateMachine _fsm;

        public GameOverState(GameContext context, GameStateMachine fsm)
        {
            _context = context;
            _fsm = fsm;
        }

        public void Enter() => Debug.Log("[GameOverState] Enter");
        public void Exit() => Debug.Log("[GameOverState] Exit");

        public void Tick(float deltaTime)
        {
            // TODO: detect restart input → ChangeState(new TapToStartState(...))
        }
    }
}
