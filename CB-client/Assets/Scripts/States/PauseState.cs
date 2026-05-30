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

        public void Enter()
        {
            Debug.Log("[PauseState] Enter");
            var menu = _context.UiRoot.GetView<MenuView>();
            menu.OnContinue = () => _fsm.ResumePreviousState();
            menu.OnRestart  = () => _fsm.ChangeState(new TapToStartState(_context, _fsm, autoStart: true));
            _context.UiRoot.Show<MenuView>();
        }

        public void Exit()
        {
            Debug.Log("[PauseState] Exit");
            _context.UiRoot.Hide<MenuView>();
        }

        public void Tick(float deltaTime) { }
    }
}
