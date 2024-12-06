using UnityEngine;
using System.Collections;

public class SaveStrings : MonoBehaviour
{
	[SerializeField]
	private StringEntry[] strings = new StringEntry[0];

	public void Execute()
	{
		foreach(StringEntry sE in strings)
			GameBase.Strings.SetValue(sE.key, sE.value);
		GameBase.Strings.Save();
	}

	[System.Serializable]
	private class StringEntry
	{
		public string key;
		public string value;
	}
}

