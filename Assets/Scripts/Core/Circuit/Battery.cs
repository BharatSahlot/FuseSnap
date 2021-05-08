using UnityEngine;

namespace Game.Circuit 
{
	public class Battery : MonoBehaviour, IEdge
	{
		internal int id = -1;

		public float voltage = 1.0f;
		public float current = 0.0f;

		[SerializeField] private Sprite _onnSprite;

		[field: SerializeField] public Terminal From { get; private set; }
		[field: SerializeField] public Terminal To { get; private set; }
        public int Id { get; set; }

        private SpriteRenderer _sprite;
		private Sprite _offSprite;

		private void Awake()
		{
			_sprite = GetComponent<SpriteRenderer>();
			_offSprite = _sprite.sprite;
		}

		private void Update()
		{
			if(From.voltage != To.voltage)
			{
				_sprite.sprite = _onnSprite;
			} else
				_sprite.sprite = _offSprite;
		}
	}
}
