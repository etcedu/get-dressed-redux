using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Camera))]
public class CustomCameraPan2D : MonoBehaviour
{
	// Variables //
	#region Serialized Fields
	[SerializeField]
	private Transform movementTarget;
	[SerializeField]
	private Vector2 lowLimit, highLimit;
	[SerializeField]
	private float speed = 0.02f;
	[SerializeField]
	private float androidSpeed = 0.02f;
	[SerializeField]
	private float mouseDeadZone = 4;
	[SerializeField]
	private float touchDeadZone = 40;
	[SerializeField]
	private bool canZoom;
	[SerializeField]
	private float lowZoomLimit = 1, highZoomLimit = 5;
	[SerializeField]
	private float wheelZoomSpeed = .06f;
	[SerializeField]
	private float iosZoomSpeed = .001f;
	[SerializeField]
	private float androidZoomSpeed = .006f;
	public bool moveSelf = false;
	[SerializeField]
	private bool reverseMovement = false;
	[SerializeField]
	private bool localPosition;
	#endregion

	#region Nonserialized Fields
	private Camera _cam;
	private Vector2 _lowLimit, _highLimit;
	private float _size, _aspect;
	private float _startingSize;
	private float _speed;
	private float _zoomSpeed;
	private float _deadZone;
	private bool _moved;
	private bool _zoomed;
	private bool _resizedOnStart;
	#endregion

	// Functions //
	#region Functionality
	void Awake()
	{
		if(movementTarget == null)
			movementTarget = transform;
		_cam = GetComponent<Camera>();
		if(_cam.orthographicSize < lowZoomLimit)
			_cam.orthographicSize = lowZoomLimit;
		_startingSize = _cam.orthographicSize;

		speed *= _startingSize;

		float minLimit = Mathf.Min((highLimit.y - lowLimit.y) / 2, (highLimit.x - lowLimit.x) / _cam.aspect / 2);
		if(highZoomLimit > minLimit)
			highZoomLimit = minLimit;

		if(highZoomLimit < lowZoomLimit)
			lowZoomLimit = highZoomLimit;

		float oldOrthographicSize = _cam.orthographicSize;
		_cam.orthographicSize = Mathf.Clamp(_cam.orthographicSize, lowZoomLimit, highZoomLimit);
		_resizedOnStart = oldOrthographicSize != _cam.orthographicSize;

		refreshAllAttributes();
		
#if !(UNITY_IOS || UNITY_ANDROID) || UNITY_EDITOR
		_deadZone = mouseDeadZone;
		_zoomSpeed = wheelZoomSpeed;
#elif UNITY_ANDROID
		speed = androidSpeed;
		_deadZone = touchDeadZone;
		_zoomSpeed = androidZoomSpeed;
#else
		_deadZone = touchDeadZone;
		_zoomSpeed = iosZoomSpeed;
#endif
	}

	void refreshAllAttributes()
	{
		refreshCameraAttributes();
		refreshMovementAttributes();
		Pan(Vector2.zero);
	}

	void refreshCameraAttributes()
	{
		_size = Mathf.Clamp(_cam.orthographicSize, lowZoomLimit, highZoomLimit);
		//_cam.orthographicSize = _size;
		_aspect = _cam.aspect;
	}

	void refreshMovementAttributes()
	{
		if(reverseMovement)
			_speed = -speed * (_size / _startingSize) / Screen.height;
		else
			_speed = speed * (_size / _startingSize) / Screen.height;
		
		float xDiff = _size * _aspect;
		_lowLimit = new Vector2(lowLimit.x + xDiff, lowLimit.y + _size);
		_highLimit = new Vector2(highLimit.x - xDiff, highLimit.y - _size);
	}

	public void Pan(Vector2 movement)
	{
		movement *= _speed;

		if(localPosition)
			movementTarget.localPosition = new Vector3(Mathf.Clamp( movementTarget.localPosition.x + movement.x, _lowLimit.x, _highLimit.x ),
			                                           Mathf.Clamp( movementTarget.localPosition.y + movement.y, _lowLimit.y, _highLimit.y ),
			                                           movementTarget.localPosition.z);
		else
		{
			movementTarget.position = new Vector3(Mathf.Clamp( movementTarget.position.x + movement.x, _lowLimit.x, _highLimit.x ),
			                                      Mathf.Clamp( movementTarget.position.y + movement.y, _lowLimit.y, _highLimit.y ),
			                                      movementTarget.localPosition.z);
		}
	}
	#endregion

	#region Updates
	void Update()
	{
#if !(UNITY_IOS || UNITY_ANDROID) || UNITY_EDITOR
		if(canZoom && Input.mouseScrollDelta != Vector2.zero)
		{
			_cam.orthographicSize = Mathf.Clamp(_cam.orthographicSize - (Input.mouseScrollDelta.y * _zoomSpeed * (_size / _startingSize)),
			                                   lowZoomLimit,
			                                   highZoomLimit);
			refreshAllAttributes();
		}
#else
		if(canZoom && TouchTracker.HasTouches(2))
		{
			TouchInfo t0 = TouchTracker.GetTouchByIndex(0);
			TouchInfo t1 = TouchTracker.GetTouchByIndex(1);
			
			if(t0.phase == TouchPhase.Moved || t1.phase == TouchPhase.Moved)
			{
				if(_moved ||
				   Vector2.Distance(t0.originPosition, t0.position) > _deadZone ||
				   Vector2.Distance(t1.originPosition, t1.position) > _deadZone)
				{
					_moved = true;
					Pan(TouchTracker.GetTouchCenterChange(t0, t1));
					if(_zoomed || t0.DistanceTo(t1) != t0.LastDistanceTo(t1))
					{
						_zoomed = true;
						float distDiff = t0.DistanceTo(t1) - t0.LastDistanceTo(t1);
						_cam.orthographicSize = Mathf.Clamp(_cam.orthographicSize - (distDiff * _zoomSpeed * (_size / _startingSize)),
						                                   lowZoomLimit,
						                                   highZoomLimit);
						refreshAllAttributes();
					}
					
				} else
				{
					_zoomed = false;
					_moved = false;
				}
			}
		}
		
		else
#endif
		if(TouchTracker.HasTouches(1) && _resizedOnStart)
		{
			TouchInfo ti = TouchTracker.GetTouchByID(0);

			if(ti != null && ti.phase == TouchPhase.Moved && (_moved || Vector2.Distance(ti.originPosition, ti.position) > _deadZone))
			{
				_moved = true;
				Pan(ti.deltaPosition);
			}
		} else 
		{
			_moved = false;
			_zoomed = false;
		}
	}

	void LateUpdate()
	{
		if(_cam.orthographicSize != _size || _cam.aspect != _aspect)
		{
			refreshAllAttributes();
		}
	}
	#endregion

	void OnDrawGizmos() 
	{
		Vector2 bottomLeft = lowLimit;
		Vector2 topLeft = lowLimit.WithY(highLimit.y);
		Vector2 bottomRight = lowLimit.WithX(highLimit.x);
		Vector2 topRight = highLimit;
		
		Gizmos.matrix = Matrix4x4.identity;

		Gizmos.color = Color.white;
		Gizmos.DrawLine(bottomLeft, topLeft);
		Gizmos.DrawLine(bottomRight, topRight);
		Gizmos.DrawLine(topLeft, topRight);
		Gizmos.DrawLine(bottomLeft, bottomRight);
	}
}

