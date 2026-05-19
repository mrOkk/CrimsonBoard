using UnityEngine;

namespace CrimsonBoard
{
    /// <summary>
    /// Bootstrap MonoBehaviour. Place on the root GameObject of the main scene.
    /// Creates GameContext and GameStateMachine, then kicks off InitState.
    /// </summary>
    public class EntryPoint : MonoBehaviour
    {
        private GameStateMachine _fsm;

        private void Awake()
        {
            var context = new GameContext();
            _fsm = new GameStateMachine();
            _fsm.ChangeState(new InitState(context, _fsm));
        }

        private void Update()
        {
            _fsm.Tick(Time.deltaTime);
        }
    }
}
