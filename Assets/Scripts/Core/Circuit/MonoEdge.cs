using UnityEngine;
using Game.Graphics;

namespace Game.Circuit
{
    public abstract class MonoEdge : MonoBehaviour
    {
        public Edge Edge { get; set; }
        public MonoTerminal A { get; set; }
        public MonoTerminal B { get; set; }

        protected SpriteRenderer _sprite;

		protected virtual void Awake()
		{
			_sprite = GetComponent<SpriteRenderer>();
		}

		protected virtual void Start()
		{
            if(_sprite == null) return;
			CircuitGrid.Instance?.AddComponent(_sprite.bounds, _sprite.sprite.bounds, transform);
		}

        // FIXME doing anything in ondestroy have problems in unity editor when exiting play mode.
        protected virtual void OnDestroy()
        {
            // transform.DetachChildren();
            if(_sprite == null) return;
            CircuitGrid.Instance?.RemoveComponent(_sprite.bounds, _sprite.sprite.bounds, transform);
        }
    }
}
