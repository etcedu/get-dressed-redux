using UnityEngine;
using System.Collections;

public class SetGender : MonoBehaviour
{
	public CrossSceneInfo.GenderEnum gender;

	public void Set()
	{
		CrossSceneInfo.Gender = gender;
	}
}

