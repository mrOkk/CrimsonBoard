using UnityEngine;

namespace CrimsonBoard
{
    /// <summary>
    /// Bootstrap MonoBehaviour. Place on the root GameObject of the main scene.
    /// Creates GameContext and GameStateMachine, then kicks off InitState.
    /// </summary>
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private GameConfig _config;
        [SerializeField] private Camera _camera;
        [SerializeField] private UiRoot _uiRoot;
        [SerializeField] private int _targetFrameRate = 60;

        private GameStateMachine _fsm;

        private void Awake()
        {
            Application.targetFrameRate = _targetFrameRate;
            var context = new GameContext(_config);
            context.Camera = _camera != null ? _camera : Camera.main;
            context.UiRoot = _uiRoot;
            _uiRoot.Init();
            _fsm = new GameStateMachine();
            _fsm.ChangeState(new InitState(context, _fsm));
        }

        private void Update()
        {
            _fsm.Tick(Time.deltaTime);
            _uiRoot.Tick(Time.deltaTime);
        }
    }
}
