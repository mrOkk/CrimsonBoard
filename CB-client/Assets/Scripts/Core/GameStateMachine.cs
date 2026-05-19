using UnityEngine;

namespace CrimsonBoard
{
    public class GameStateMachine
    {
        private IGameState _currentState;
        private IGameState _previousState;

        public IGameState CurrentState => _currentState;

        public void ChangeState(IGameState newState)
        {
            _previousState = _currentState;
            _currentState?.Exit();
            _currentState = newState;
            _currentState.Enter();
        }

        /// <summary>
        /// Transitions to PauseState from any post-init state.
        /// Call ResumePreviousState() to return.
        /// </summary>
        public void RequestPause(IGameState pauseState)
        {
            if (_currentState == pauseState) return;
            ChangeState(pauseState);
        }

        public void ResumePreviousState()
        {
            if (_previousState == null)
            {
                Debug.LogWarning("[GameStateMachine] No previous state to resume.");
                return;
            }
            ChangeState(_previousState);
        }

        public void Tick(float deltaTime)
        {
            _currentState?.Tick(deltaTime);
        }
    }
}
