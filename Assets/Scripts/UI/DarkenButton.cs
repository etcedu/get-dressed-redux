using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UISprite))]
public class DarkenButton : MonoBehaviour
{
	private UISprite _sprite;
	private Color _defaultColor;
	private Coroutine _currentAnimation;

	private Transform _transform;

	public void Awake()
	{
		_sprite = gameObject.GetComponent<UISprite>();
		_defaultColor = _sprite.color;
		_transform = gameObject.transform;
	}

	public Color DefaultColor
	{
		set
		{
			_defaultColor = value;
			if (_sprite != null) _sprite.color = value;
		}
	}

	public void OnPress(bool isDown)
	{
		if (_currentAnimation != null) StopCoroutine(_currentAnimation);
		_currentAnimation = StartCoroutine(isDown ? press () : unPress());
	}

	private IEnumerator press()
	{
		float progress = 0;
		var darker = GameColors.Darken(_defaultColor);
		while (progress < 1)
		{
			progress += Time.deltaTime * 8;
			if (progress > 1) progress = 1;
			_sprite.color = Color.Lerp(_defaultColor, darker, progress);
			_transform.localScale = (1f - (.05f * progress)) * Vector3.one;
			yield return null;
		}
	}

	private IEnumerator unPress()
	{
		float progress = 0;
		var darker = GameColors.Darken(_defaultColor);
		while (progress < 1)
		{
			progress += Time.deltaTime * 8;
			if (progress > 1) progress = 1;
			_sprite.color = Color.Lerp(darker, _defaultColor, progress);
			_transform.localScale = (.95f + (.05f * progress)) * Vector3.one;
			yield return null;
		}
	}
}