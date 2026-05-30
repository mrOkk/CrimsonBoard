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

        private GameStateMachine _fsm;

        private void Awake()
        {
            var context = new GameContext(_config);
            context.Camera = _camera != null ? _camera : Camera.main;
            _fsm = new GameStateMachine();
            _fsm.ChangeState(new InitState(context, _fsm));
            _uiRoot?.Init();
            context.UiRoot = _uiRoot;
        }

        private void Update()
        {
            _fsm.Tick(Time.deltaTime);
            _uiRoot?.Tick(Time.deltaTime);
        }
    }
}
