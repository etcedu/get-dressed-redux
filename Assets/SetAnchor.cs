using UnityEngine;
using System.Collections;

public class SetAnchor : MonoBehaviour {
	public UIWidget[] targets;

	public void ToOnEnable()
	{
		foreach(UIWidget target in targets)
		{
			target.updateAnchors = UIRect.AnchorUpdate.OnEnable;
		}
	}
	public void ToOnStart()
	{
		foreach(UIWidget target in targets)
		{
			target.updateAnchors = UIRect.AnchorUpdate.OnStart;
		}
	}
	public void ToOnUpdate()
	{
		foreach(UIWidget target in targets)
		{
			target.updateAnchors = UIRect.AnchorUpdate.OnUpdate;
		}
	}
}
