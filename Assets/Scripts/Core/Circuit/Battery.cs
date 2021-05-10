using UnityEngine;

namespace Game.Circuit 
{
	public class Battery : MonoBehaviour, IEdge
	{
		public float voltage = 1.0f;

		[SerializeField] private Sprite _onnSprite;

		[field: SerializeField] public Terminal From { get; set; }
		[field: SerializeField] public Terminal To { get; set; }
        public int Id { get; set; } = -1;
        public float Current { get; internal set; }

        private SpriteRenderer _sprite;
		private Sprite _offSprite;

		private void Awake()
		{
			_sprite = GetComponent<SpriteRenderer>();
			_offSprite = _sprite.sprite;
		}

		private void Update()
		{
			if(Current != 0)
			{
				_sprite.sprite = _onnSprite;
			} else
				_sprite.sprite = _offSprite;
		}
	}
}
