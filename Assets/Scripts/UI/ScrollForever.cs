using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UIWidget))]
public class ScrollForever : MonoBehaviour
{
	private UIWidget _widget;
	private Transform _transform;
	private Vector3 _startPosition;

	public void Awake()
	{
		_widget = gameObject.GetComponent<UIWidget>();
		_transform = gameObject.transform;
		_startPosition = _transform.localPosition;
	}

	public void Update()
	{
		_transform.localPosition += Time.deltaTime * 75 * Vector3.up;
		if (_transform.localPosition.y > 600 + _widget.height) _transform.localPosition = _startPosition;
	}
}