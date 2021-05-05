using UnityEngine;

namespace Game.Circuit 
{
	public class Fuse : MonoBehaviour, IEdge
	{
		public float resistance = 1.0f;
		public float max_current = 1.0f;
		[SerializeField] private Sprite _onnSprite;

		[field: SerializeField] public Terminal From { get; set; }
		[field: SerializeField] public Terminal To { get; set; }

		private SpriteRenderer _sprite;
		private Sprite _offSprite;

		private void Awake()
		{
			_sprite = GetComponent<SpriteRenderer>();
			_offSprite = _sprite.sprite;

			From.AddEdge(this);
			To.AddEdge(this);
		}

		private void Update()
		{
			float current = (From.voltage - To.voltage) / resistance;
			if(Mathf.Abs(current) > 0.0f)
			{
				_sprite.sprite = _onnSprite;
			} else
				_sprite.sprite = _offSprite;
		}
	}
}
