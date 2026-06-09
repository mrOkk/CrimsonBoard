namespace CrimsonBoard
{
    public class KnockbackSystem : IGameSystem
    {
        private readonly GameContext _context;

        public KnockbackSystem(GameContext context)
        {
            _context = context;
        }

        public void Initialize() { }

        public void Tick(float deltaTime)
        {
            var player = _context.Player;
            if (player == null || !player.IsKnockback)
            {
                return;
            }

            var inputDir = _context.InputState.IsKeysHeld ? _context.InputState.MoveCommand : null;
            player.TickKnockback(deltaTime, inputDir, _context.Config.knockback, _context.Config.board);
        }

        public void Dispose() { }
    }
}
