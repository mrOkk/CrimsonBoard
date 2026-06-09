using System;

namespace CrimsonBoard
{
    [Serializable]
    public class CameraConfig
    {
        public float CameraAngle = 45f;
        public float CameraDistance = 10f;
        public float CameraFov = 40f;
        public bool IsDebugMode = false;
    }
}
