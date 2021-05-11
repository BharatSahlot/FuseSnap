using UnityEngine;

namespace Game.Circuit 
{
	public class Fuse : MonoEdge
	{
		public float resistance = 1.0f;
		public float max_current = 1.0f;
		[SerializeField] private Sprite _onnSprite;

		private Sprite _offSprite;

		protected override void Awake()
		{
			base.Awake();
			_offSprite = _sprite.sprite;
		}

		private void Update()
		{
			float current = (From.Voltage - To.Voltage) / resistance;
			if(Mathf.Abs(current) > 0.0f)
			{
				_sprite.sprite = _onnSprite;
			} else
				_sprite.sprite = _offSprite;
		}
	}
}
