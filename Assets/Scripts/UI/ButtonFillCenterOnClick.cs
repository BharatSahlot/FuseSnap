using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class ButtonFillCenterOnClick : MonoBehaviour
	{
		private Image _image;
		private void Awake()
		{
			_image = GetComponent<Image>();
		}

		public void Click()
		{
			_image.fillCenter = !_image.fillCenter;
		}
	}
}
