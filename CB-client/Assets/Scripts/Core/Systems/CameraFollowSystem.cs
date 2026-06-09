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
        private readonly CameraConfig _config;

        private float _pitchDegrees;
        private float _height;
        private float _backOffset;

        public CameraFollowSystem(GameContext context)
        {
            _context = context;
            _config = context.Config.camera;
        }

        public void Initialize()
        {
            _context.Camera.fieldOfView = _config.CameraFov;
            _pitchDegrees = _config.CameraAngle;
            var pitchRadians = _pitchDegrees * Mathf.Deg2Rad;
            _height = _config.CameraDistance * Mathf.Sin(pitchRadians);
            _backOffset = _config.CameraDistance * Mathf.Cos(pitchRadians);
            SnapCamera();
        }

        public void Tick(float deltaTime)
        {
            if (_config.IsDebugMode)
            {
                Initialize();
            }
            else
            {
                SnapCamera();
            }
        }

        public void Dispose() { }

        private void SnapCamera()
        {
            if (_context.Camera == null || _context.Player == null)
            {
                return;
            }

            var playerPos = _context.Player.transform.position;
            playerPos.y = 0f; // Ignore player's y position for camera follow
            _context.Camera.transform.position = new Vector3(playerPos.x, playerPos.y+ _height, playerPos.z - _backOffset);
            _context.Camera.transform.rotation = Quaternion.Euler(_pitchDegrees, 0f, 0f);
        }
    }
}
