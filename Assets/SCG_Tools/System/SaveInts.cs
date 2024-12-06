using UnityEngine;
using System.Collections;

public class SaveInts : MonoBehaviour
{
	[SerializeField]
	private IntEntry[] ints = new IntEntry[0];
	
	public void Execute()
	{
		foreach(IntEntry iE in ints)
			GameBase.Ints.SetValue(iE.key, iE.value);
		GameBase.Ints.Save();
	}
	
	[System.Serializable]
	private class IntEntry
	{
		public string key;
		public int value;
	}
}

