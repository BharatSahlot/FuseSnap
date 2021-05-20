using UnityEngine;
using Game.Graphics;

namespace Game.Circuit
{
    public abstract class MonoEdge : MonoBehaviour, IEdge
    {
        [field: SerializeField] public Terminal From { get; set; }
        [field: SerializeField] public Terminal To { get; set; }
        public int Id { get; set; }
		public float Current { get; set; }

        protected SpriteRenderer _sprite;

		protected virtual void Awake()
		{
			_sprite = GetComponent<SpriteRenderer>();
		}

		protected virtual void Start()
		{
			CircuitGrid.Instance.AddComponent(_sprite.bounds, _sprite.sprite.bounds, transform);
		}
    }
}
