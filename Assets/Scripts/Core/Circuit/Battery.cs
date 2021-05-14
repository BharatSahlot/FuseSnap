using UnityEngine;

namespace Game.Circuit 
{
	public class Battery : MonoEdge 
	{
		public float voltage = 1.0f;

		[SerializeField] private Sprite _onnSprite;
		public Terminal PositiveTerminal => From;
		public Terminal NegativeTerminal => To;

		private Sprite _offSprite;

		protected override void Awake()
		{
			base.Awake();
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
