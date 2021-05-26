using UnityEngine;
using Game.Graphics;
using Game.Circuit;
using MessagePack;

namespace Game.Map
{
    [MessagePackObject]
    public class MapEdgeData
    {
        [Key(0)]
        public ushort R { get; set; }
        [Key(1)]
        public ushort C { get; set; }
        [Key(2)]
        public short Rotation { get; set; } // 1 => Positive above negative, -1 => Negative above positive
        [Key(3)]
        public Edge Edge { get; set; }
    }

    public abstract class MonoEdge : MonoBehaviour
    {
        public MapEdgeData MapEdge { get; set; }

        public Edge Edge => MapEdge.Edge;
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
