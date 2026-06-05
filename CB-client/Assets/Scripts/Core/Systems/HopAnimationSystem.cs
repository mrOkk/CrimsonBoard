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
            if (_board == null) return;
            foreach (var enemy in _board.ActiveEnemies)
                enemy.TickHop(deltaTime);
        }

        public void Dispose() { }
    }
}
