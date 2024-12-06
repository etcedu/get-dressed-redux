/// <summary>
/// Go-to for persisting data
/// </summary>
using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public static class GameBase {
	static SaveSystem<string> strings = new SaveSystem<string>("stringSave", (Func<string>)(()=>{ return ""; }));
	public static SaveSystem<string> Strings
	{
		get{ return strings; }
	}
	static SaveSystem<string[]> stringArrays = new SaveSystem<string[]>("stringArraySave", (Func<string[]>)(()=>{ return new string[0]; }));
	public static SaveSystem<string[]> StringArrays
	{
		get{ return stringArrays; }
	}
	static SaveSystem<bool> bools = new SaveSystem<bool>("boolSave", (Func<bool>)(()=>{ return false; }));
	public static SaveSystem<bool> Bools
	{
		get{ return bools; }
	}
	static SaveSystem<int> ints = new SaveSystem<int>("intSave", (Func<int>)(()=>{ return 0; }));
	public static SaveSystem<int> Ints
	{
		get{ return ints; }
	}

	public static void SaveAll() {
		strings.Save();
		stringArrays.Save();
		bools.Save();
		ints.Save();
	}

#if UNITY_EDITOR
	[UnityEditor.MenuItem ("System/Delete Saved Info")]
#endif
	public static void DeleteSavedInfo()
	{
		strings.Delete();
		stringArrays.Delete();
		bools.Delete();
		ints.Delete();
	}

	public static void ClearActiveInfo()
	{

	}
}
