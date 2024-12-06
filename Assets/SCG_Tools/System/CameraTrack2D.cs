using UnityEngine;
using System.Collections;

#pragma warning disable 0649
[RequireComponent (typeof(Camera))]
public class CameraTrack2D : MonoBehaviour {
	[SerializeField]
	private bool parentOnTrack;

	[SerializeField]
	private bool startingInstance;

	[SerializeField]
	private bool holdXPos, holdYPos, holdZPos;

	[SerializeField]
	[Range(.001f, 2)]
	private float trackingDuration = 1;

	private Transform _target;
	private Vector3 _targetPos;
	private Vector3 _goalPos;
	private float _goalSize;
	private Camera _camera;

	private Transform _originalParent;

	private Vector3 _startingPos;
	private float _startingSize;

	private IEnumerator _reposition;
	private IEnumerator _resize;
	private static CameraTrack2D _instance;


	void Start()
	{
		if(startingInstance && _instance == null)
			_instance = this;

		_camera = GetComponent<Camera>();

		if(parentOnTrack)
			_originalParent = transform.parent;

		_goalPos = transform.position;
		_goalSize = _camera.orthographicSize;

		_startingPos = _goalPos;
		_startingSize = _goalSize;
	}
	
	/// <summary>
	/// Resets the instance's position and size.
	/// </summary>
	/// <param name="target">Target.</param>
	public static void ResetTarget()
	{
		if(_instance == null)
			return;
		
		_instance.Reset();
	}
	/// <summary>
	/// Reset this tracker to it's original position and size
	/// </summary>
	public void Reset()
	{
		if(parentOnTrack)
			transform.SetParent(_originalParent);

		_target = null;
		_goalPos = _startingPos;
		_goalSize = _startingSize;

#if UNITY_EDITOR
		if(!Application.isPlaying && _camera == null)
			_camera = GetComponent<Camera>();
#endif
		
		if(Vector3.Distance(transform.position, _goalPos) > .01f)
		{
			if(_reposition != null) StopCoroutine(_reposition);
			_reposition = reposition();
			StartCoroutine(_reposition);
		}
		
		if(Mathf.Abs(_camera.orthographicSize - _goalSize) > .01f)
		{
			if(_resize != null) StopCoroutine(_resize);
			_resize = resize();
			StartCoroutine(_resize);
		}
	}

	void LateUpdate()
	{
		if(parentOnTrack && transform.parent != _originalParent)
		{
			transform.position = new Vector3(holdXPos? _startingPos.x : transform.position.x,
			                                      holdYPos? _startingPos.y : transform.position.y,
			                                      holdZPos? _startingPos.z : transform.position.z);
		}
	}

	/// <summary>
	/// Sets the target of this tracker
	/// </summary>
	public void SetThisTarget(Transform target, Vector3 offset, float zoomBoarderAmount, float colliderMax)
	{
		// Ensure target can/should be set
		if(target == null)
			return;

		if(target == _target)
		{
			Reset();
			_target = null;
			return;
		}

		// Keep track of current target
		_target = target;
		if(parentOnTrack)
			transform.SetParent(_target);

		// Set size and position variables
		_goalSize = colliderMax * zoomBoarderAmount;
		if(!parentOnTrack)
			_goalPos = new Vector3(holdXPos? transform.position.x : (target.position.x + offset.x),
			                       holdYPos? transform.position.y : (target.position.y + offset.y),
									holdZPos? transform.position.z : (target.position.z + offset.z));
		else
			_goalPos = new Vector3(holdXPos? transform.localPosition.x : offset.x,
			                       holdYPos? transform.localPosition.y : offset.y,
			                       holdZPos? transform.localPosition.z : offset.z);
		
		
		// Reposition if necessary
		if((!parentOnTrack && transform.position != _goalPos) || (parentOnTrack && transform.localPosition != _goalPos))
		{
			if(_reposition != null) StopCoroutine(_reposition);
			_reposition = reposition();
			StartCoroutine(_reposition);
		}

		// Resize if necessary
		if(_camera.orthographicSize != _goalSize)
		{
			if(_resize != null) StopCoroutine(_resize);
			_resize = resize();
			StartCoroutine(_resize);
		}
	}

	/// <summary>
	/// Sets the instance's target.
	/// </summary>
	/// <param name="target">Target.</param>
	public static void SetTarget(Transform target, Vector3 offset, float zoomBoarderAmount, float colliderMax)
	{
		if(_instance == null)
			return;

		_instance.SetThisTarget(target, offset, zoomBoarderAmount, colliderMax);
	}
	public static void SetTarget(Transform target, float colliderMax)
	{
		if(_instance == null)
			return;

		_instance.SetThisTarget(target, _instance._startingPos - target.position, _instance._startingSize / colliderMax, colliderMax);
	}
	public static void SetTarget(Transform target, Vector3 offset, float colliderMax)
	{
		if(_instance == null)
			return;

		_instance.SetThisTarget(target, offset, _instance._startingSize / colliderMax, colliderMax);
	}
	public static void SetTarget(Transform target, float zoomBoarderAmount, float colliderMax)
	{
		if(_instance == null)
			return;

		_instance.SetThisTarget(target, Vector3.zero, zoomBoarderAmount, colliderMax);
	}
	
	/// <summary>
	/// Reposition this camera over time
	/// </summary>
	IEnumerator reposition() {
		float step = 0f, smoothStep = 0f, lastStep = 0f;
		float amount = Vector3.Distance((parentOnTrack? transform.localPosition : transform.position), _goalPos);
		//Debug.Log("reposition amount: " + amount);
		do{
			step += Time.deltaTime / trackingDuration;
			smoothStep = Mathf.SmoothStep(0f, 1f, step);
			if(parentOnTrack && _target != null)
			{
				Vector3 invPos = _target.InverseTransformPoint(_startingPos);
				if(holdXPos)
					_goalPos.x = invPos.x;
				if(holdYPos)
					_goalPos.y = invPos.y;
				if(holdZPos)
					_goalPos.z = invPos.z;
				transform.localPosition = Vector3.MoveTowards(transform.localPosition, _goalPos, amount * (smoothStep - lastStep));
			}
			else
				transform.position = Vector3.MoveTowards(transform.position, _goalPos, amount * (smoothStep - lastStep));
			lastStep = smoothStep;
			yield return null;
		} while(step < 1.0f);

		if(step > 1.0f) 
		{
			if(parentOnTrack && _target != null)
			{
				Vector3 inv = _target.InverseTransformPoint(_startingPos);
				transform.localPosition = new Vector3(holdXPos? inv.x : _goalPos.x,
				                                      holdYPos? inv.y : _goalPos.y,
				                                      holdZPos? inv.z : _goalPos.z);
			}
			else
				transform.position = _goalPos;
		}
		//Debug.Log("done with reposition");
		_reposition = null;
	}
	
	/// <summary>
	/// Adjust this camera's orthographic size over time
	/// </summary>
	IEnumerator resize() {
		float step = 0f, smoothStep = 0f, lastStep = 0f;
		float amount = _goalSize - _camera.orthographicSize;
		float startingSize = _camera.orthographicSize;
		//Debug.Log("resize amount: " + amount);
		do{
			step += Time.deltaTime / trackingDuration;
			smoothStep = Mathf.SmoothStep(0f, 1f, step);
			_camera.orthographicSize += amount * (smoothStep - lastStep);
			lastStep = smoothStep;
			yield return null;
		} while(step < 1.0f);
		if(step > 1.0f) _camera.orthographicSize = startingSize + amount;
		//Debug.Log("done with resize");
		_resize = null;
	}
}
#pragma warning restore 0649
