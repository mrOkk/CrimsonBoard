using UnityEngine;

namespace CrimsonBoard
{
    [System.Serializable]
    public class PlayerConfig
    {
        public Mesh mesh;
        public float health;
        public int movesPerBeat;
        public float movementInputDelay = 0.1f;
        public float inputBufferWindow = 0.15f;
        public InventoryEntry[] startingInventory;
    }
}
