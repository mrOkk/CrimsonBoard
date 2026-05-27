namespace CrimsonBoard
{
    public class HopAnimationSystem : IGameSystem
    {
        private readonly GameContext _context;

        public HopAnimationSystem(GameContext context)
        {
            _context = context;
        }

        public void Initialize() { }

        public void Tick(float deltaTime)
        {
            _context.Player?.TickHop(deltaTime);
        }

        public void Dispose() { }
    }
}
