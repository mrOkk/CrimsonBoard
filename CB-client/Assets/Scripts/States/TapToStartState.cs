using UnityEngine;

namespace CrimsonBoard
{
    public class TapToStartState : IGameState
    {
        private readonly GameContext _context;
        private readonly GameStateMachine _fsm;
        private CameraFollowSystem _cameraFollowSystem;

        public TapToStartState(GameContext context, GameStateMachine fsm)
        {
            _context = context;
            _fsm = fsm;
        }

        public void Enter()
        {
            Debug.Log("[TapToStartState] Enter");

            _context.GameFieldSystem = new GameFieldSystem(_context);
            _context.GameFieldSystem.Initialize();

            new PlayerSpawnSystem(_context).Initialize();

            _cameraFollowSystem = new CameraFollowSystem(_context);
            _cameraFollowSystem.Initialize();
        }

        public void Exit()
        {
            Debug.Log("[TapToStartState] Exit");
            _cameraFollowSystem?.Dispose();
            _cameraFollowSystem = null;
        }

        public void Tick(float deltaTime)
        {
            _cameraFollowSystem?.Tick(deltaTime);

            // DEBUG: keyboard shortcut to skip to gameplay without tap
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("[TapToStartState] Debug: Space pressed → GameplayState");
                _fsm.ChangeState(new GameplayState(_context, _fsm));
                return;
            }

            // TODO: replace with proper UI tap/click handler (button event or input action)
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                _fsm.ChangeState(new GameplayState(_context, _fsm));
            }
        }
    }
}
