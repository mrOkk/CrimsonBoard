using UnityEngine;

namespace CrimsonBoard
{
    public class PowerUpView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Collider _collider;

        public SpriteRenderer SpriteRenderer => _spriteRenderer;
        public Collider Collider => _collider;
    }
}
