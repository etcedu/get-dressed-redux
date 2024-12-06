using UnityEngine;
using System.Collections;

public class ColorUse : MonoBehaviour
{
	public GameColors.Uses Use;

	public void Awake()
	{
		var sprite = gameObject.GetComponent<UISprite>();
		var label = gameObject.GetComponent<UILabel>();

		if (sprite != null) sprite.color = GameColors.ForUse(Use);
		if (label != null) label.color = GameColors.ForUse(Use);
	}
}

