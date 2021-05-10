using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Game.UI
{
	[RequireComponent(typeof(Button))]
	public class PlayButton : MonoBehaviour
	{
		private Button _button;
		private void Awake()
		{
			_button = GetComponent<Button>();
			_button.onClick.AddListener(() => 
			{
				SceneManager.LoadScene(1);
			});
		}
	}
}
