namespace CrimsonBoard
{
    public class HopAnimationSystem : IGameSystem
    {
        private readonly GameContext _context;
        private readonly GameBoard _board;

        public HopAnimationSystem(GameContext context, GameBoard board)
        {
            _context = context;
            _board = board;
        }

        public void Initialize() { }

        public void Tick(float deltaTime)
        {
            _context.Player?.TickHop(deltaTime);
            if (_board == null)
            {
                return;
            }

            for (var index = 0; index < _board.ActiveEnemies.Count; index++)
            {
                var enemy = _board.ActiveEnemies[index];
                enemy.TickHop(deltaTime);
            }
        }

        public void Dispose() { }
    }
}
