using UnityEngine;
using Game.Data;

namespace Game.Graphics
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class MBattery : MonoBehaviour
    {
        public Battery Battery { get; set; }
        public float Height { get; set; }

        private SpriteRenderer _sprite;
        public void Init()
        {
            _sprite = GetComponent<SpriteRenderer>();
            CircuitGrid.Instance?.AddComponent(_sprite.bounds, _sprite.sprite.bounds, transform);
        }
    }
}
