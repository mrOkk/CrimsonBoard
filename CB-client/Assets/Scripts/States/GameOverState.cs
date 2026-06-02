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

        public void Enter()
        {
            Debug.Log("[GameOverState] Enter");

            var enemies = _context.EnemySpawnSystem?.ActiveEnemies;
            if (enemies != null && enemies.Count > 0)
            {
                var snapshot = new System.Collections.Generic.List<EnemyView>(enemies);
                DissolveService.DissolveAllAndReturn(snapshot, _context.OccupancyMap, _context.Pools);
            }

            var view = _context.UiRoot.GetView<PostBattleView>();
            view.OnRestart = () => _fsm.ChangeState(new GameplayState(_context, _fsm));
            _context.UiRoot.Show<PostBattleView>();
        }

        public void Exit()
        {
            Debug.Log("[GameOverState] Exit");
            _context.UiRoot.Hide<PostBattleView>();
        }

        public void Tick(float deltaTime) { }
    }
}
