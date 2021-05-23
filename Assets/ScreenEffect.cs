using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

[ExecuteInEditMode]
public class ScreenEffect : MonoBehaviour
{
	[SerializeField] private Material mat;
	[SerializeField] private float refreshSpeed = 1.0f;
	[SerializeField] private float g1Speed = 1.0f, g2Speed = 0.5f;
	private float _offset, _flicker, _g1Offset, _g2Offset;

	private void Awake()
	{
		_g1Offset = Random.value;
		_g2Offset = Random.value;
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		_offset += Time.deltaTime * refreshSpeed;
		_g1Offset += Time.deltaTime * g1Speed;
		_g2Offset += Time.deltaTime * g2Speed;
	//	if(_offset > 1.0f) _offset = 0;

		_flicker = Mathf.Lerp(_flicker, Random.Range(0.5f, 1.0f), 0.5f);

		mat.SetFloat("_Offset", _offset);
		// mat.SetFloat("_Flicker", _flicker);
		mat.SetFloat("_Gradient1_Offset", _g1Offset);
		mat.SetFloat("_Gradient2_Offset", _g2Offset);
		Graphics.Blit(source, destination, mat);
	}
}
