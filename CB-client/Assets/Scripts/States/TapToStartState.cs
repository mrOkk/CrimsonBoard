using UnityEngine;

namespace CrimsonBoard
{
    public class TapToStartState : IGameState
    {
        private readonly GameContext _context;
        private readonly GameStateMachine _fsm;
        private readonly bool _autoStart;
        private CameraFollowSystem _cameraFollowSystem;
        private GameFieldSystem _gameFieldSystem;

        public TapToStartState(GameContext context, GameStateMachine fsm, bool autoStart = false)
        {
            _context = context;
            _fsm = fsm;
            _autoStart = autoStart;
        }

        public void Enter()
        {
            Debug.Log("[TapToStartState] Enter");

            _gameFieldSystem = new GameFieldSystem(_context);
            _gameFieldSystem.Initialize();
            _context.Board = new GameBoard(_gameFieldSystem);

            new PlayerSpawnSystem(_context).Initialize();

            _cameraFollowSystem = new CameraFollowSystem(_context);
            _cameraFollowSystem.Initialize();

            if (_autoStart)
            {
                _fsm.ChangeState(new GameplayState(_context, _fsm));
                return;
            }

            var view = _context.UiRoot.GetView<PreBattleView>();
            view.OnPlayerInput = () => _fsm.ChangeState(new GameplayState(_context, _fsm));
            _context.UiRoot.Show<PreBattleView>();
        }

        public void Exit()
        {
            Debug.Log("[TapToStartState] Exit");
            if (!_autoStart)
                _context.UiRoot.Hide<PreBattleView>();
            _cameraFollowSystem?.Dispose();
            _cameraFollowSystem = null;
        }

        public void Tick(float deltaTime)
        {
            _cameraFollowSystem?.Tick(deltaTime);
        }
    }
}
