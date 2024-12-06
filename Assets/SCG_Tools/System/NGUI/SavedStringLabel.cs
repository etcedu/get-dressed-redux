using UnityEngine;
using System.Collections;

[RequireComponent (typeof(UILabel))]
public class SavedStringLabel : MonoBehaviour {

	public string stringKey;

	void Start ()
	{
		GetComponent<UILabel>().text = GameBase.Strings.GetValue(stringKey);
	}
}
