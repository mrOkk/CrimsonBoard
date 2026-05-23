using UnityEngine;

namespace CrimsonBoard
{
    public class BoardTileView : MonoBehaviour
    {
        [SerializeField] private MeshFilter _meshFilter;

        public MeshFilter MeshFilter => _meshFilter;
    }
}
