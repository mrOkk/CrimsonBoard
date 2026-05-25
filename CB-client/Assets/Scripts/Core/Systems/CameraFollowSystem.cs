using UnityEngine;

namespace CrimsonBoard
{
    /// <summary>
    /// Positions and orients the main camera above the player each frame.
    /// Height and pitch angle are configurable via constructor parameters.
    /// </summary>
    public class CameraFollowSystem : IGameSystem
    {
        private readonly GameContext _context;
        private readonly float _height;
        private readonly float _pitchDegrees;

        public CameraFollowSystem(GameContext context, float height = 20f, float pitchDegrees = 60f)
        {
            _context = context;
            _height = height;
            _pitchDegrees = pitchDegrees;
        }

        public void Initialize()
        {
            SnapCamera();
        }

        public void Tick(float deltaTime)
        {
            SnapCamera();
        }

        public void Dispose() { }

        private void SnapCamera()
        {
            if (_context.Camera == null || _context.Player == null) return;

            var playerPos = _context.Player.transform.position;
            float pitchRad = _pitchDegrees * Mathf.Deg2Rad;
            float backOffset = _height / Mathf.Tan(pitchRad);

            _context.Camera.transform.position = new Vector3(playerPos.x, playerPos.y + _height, playerPos.z - backOffset);
            _context.Camera.transform.rotation = Quaternion.Euler(_pitchDegrees, 0f, 0f);
        }
    }
}
