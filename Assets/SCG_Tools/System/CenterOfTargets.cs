using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CenterOfTargets : MonoBehaviour
{
	public Transform[] targets = new Transform[0];
	public bool x = true, y = true, z = true;
	public Vector3 offset;

	Vector3 _startingPosition;

	void OnEnable()
	{
		if(_startingPosition == null)
		{
			setStartingPosition();
		}
	}

	[ContextMenu ("Reset Starting Position")]
	void setStartingPosition()
	{
		_startingPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update ()
	{
		Vector3 center = transform.position;
		if(targets.Length != 0)
			center = targets.Center();

		center = center.WithX(x? center.x : _startingPosition.x)
						.WithY(y? center.y : _startingPosition.y)
						.WithZ(z? center.z : _startingPosition.z)
				+ offset;
		transform.position = center;
	}
}

