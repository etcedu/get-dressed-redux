using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class TouchTracker : MonoBehaviour
{
	private static TouchTracker instance;
	private static Dictionary<int, TouchInfo> currentTouches;
	private static bool setup;

	private static void CheckInstance() {
		if(instance == null) {
			setup = true;
			GameObject touchTrackerGO = new GameObject("TouchTracker", typeof(TouchTracker));
			DontDestroyOnLoad(touchTrackerGO);
			instance = touchTrackerGO.GetComponent<TouchTracker>();
			currentTouches = new Dictionary<int, TouchInfo>();
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
			foreach(Touch t in Input.touches)
				currentTouches.Add(t.fingerId, new TouchInfo(t));
#endif
		}
	}

	private void Start() {
		if(setup && instance != this) DestroyImmediate(this);
		else {
			setup = true;
			DontDestroyOnLoad(gameObject);
			instance = this;
			currentTouches = new Dictionary<int, TouchInfo>();
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
			foreach(Touch t in Input.touches)
				currentTouches.Add(t.fingerId, new TouchInfo(t));
#endif
		}
	}

	void OnDestroy() {
		if(instance == this) {
			setup = false;
			instance = null;
			currentTouches = null;
		}
	}

	private void Update() {
		List<int> rmTouch = new List<int>();
		foreach(KeyValuePair<int, TouchInfo> kvp in currentTouches) {
			if(kvp.Value.phase == TouchPhase.Ended)
				rmTouch.Add(kvp.Key);
		}
		foreach(int i in rmTouch)
			currentTouches.Remove(i);

#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
		foreach(Touch t in Input.touches)
			UpdateTouch(t);
#else
		foreach(KeyValuePair<int, TouchInfo> kvp in currentTouches)
			UpdateTouch(kvp.Key);
		
		if(Input.GetMouseButton(0) && !currentTouches.ContainsKey(0))
			UpdateTouch(0);
		if(Input.GetMouseButton(1) && !currentTouches.ContainsKey(1))
			UpdateTouch(1);
		if(Input.GetMouseButton(2) && !currentTouches.ContainsKey(2))
			UpdateTouch(2);
#endif
	}
	
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
	void UpdateTouch(Touch t)
	{
		if(!currentTouches.ContainsKey(t.fingerId))
			currentTouches.Add(t.fingerId, new TouchInfo(t));
		else
			currentTouches[t.fingerId].Update(t);
	}
#else
	void UpdateTouch(int id)
	{
		if(!currentTouches.ContainsKey(id))
			currentTouches.Add(id, new TouchInfo(id));
		else
			currentTouches[id].Update();
	}
#endif

	public static bool HasTouches(int minAmount) {
#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
		return Input.touchCount >= minAmount;
#else
		int mouseButtons = 0;
		if(Input.GetMouseButton(0)) ++mouseButtons;
		if(Input.GetMouseButton(1)) ++mouseButtons;
		if(Input.GetMouseButton(2)) ++mouseButtons;
		return mouseButtons >= minAmount;
#endif
	}

	public static TouchInfo GetTouchByIndex(int index) {
		CheckInstance();
		if(index >= Input.touchCount) return null;
		return currentTouches[Input.touches[index].fingerId];
	}
	
	public static TouchInfo GetTouchByID(int id) {
		CheckInstance();
		if(!currentTouches.ContainsKey(id)) return null;
		return currentTouches[id];
	}

	public static float GetTouchDistance(Vector2 one, Vector2 two) {
		return Vector2.Distance(one, two);
	}
	public static float GetTouchDistance(int one, int two, bool id) {
		if(!id)
		{
			one = Input.touches[one].fingerId;
			two = Input.touches[two].fingerId;
		}

		if(!currentTouches.ContainsKey(one) || !currentTouches.ContainsKey(two))
			return 0;
		return GetTouchDistance(currentTouches[one].position, currentTouches[two].position);
	}
	
	public static Vector2 GetTouchCenter(params Vector2[] touches) {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
		if(touches == null || touches.Length == 0)
			return Vector2.zero;

		Vector2 center = Vector2.zero;
		for(int i=0; i<touches.Length; ++i) {
			center += touches[i];
		}

		return new Vector2(center.x/touches.Length, center.y/touches.Length);
#else
		if(HasTouches(1))
			return Input.mousePosition.MakeV2XY();
		return Vector2.zero;
#endif
	}
	public static Vector2 GetTouchCenter(int one, int two, bool id) {
		if(!id)
		{
			one = Input.touches[one].fingerId;
			two = Input.touches[two].fingerId;
		}

		if(!currentTouches.ContainsKey(one) || !currentTouches.ContainsKey(two))
			return Vector2.zero;
		return GetTouchCenter(new Vector2[]{currentTouches[one].position, currentTouches[two].position});
	}
	public static Vector2 GetTouchCenter(TouchInfo one, TouchInfo two)
	{
		return GetTouchCenter(new Vector2[]{one.position, two.position});
	}
	public static Vector2 GetLastTouchCenter(int one, int two, bool id) {
		if(!id)
		{
			one = Input.touches[one].fingerId;
			two = Input.touches[two].fingerId;
		}

		if(!currentTouches.ContainsKey(one) || !currentTouches.ContainsKey(two))
			return Vector2.zero;
		return GetTouchCenter(new Vector2[]{currentTouches[one].lastPosition, currentTouches[two].lastPosition});
	}
	public static Vector2 GetLastTouchCenter(TouchInfo one, TouchInfo two)
	{
		return GetTouchCenter(new Vector2[]{one.lastPosition, two.lastPosition});
	}
	public static Vector2 GetTouchCenterChange(int one, int two, bool id) {
		if(!id)
		{
			one = Input.touches[one].fingerId;
			two = Input.touches[two].fingerId;
		}

		if(!currentTouches.ContainsKey(one) || !currentTouches.ContainsKey(two))
			return Vector2.zero;
		return GetTouchCenterChange(currentTouches[one], currentTouches[two]);
	}
	public static Vector2 GetTouchCenterChange(TouchInfo one, TouchInfo two)
	{
		return GetTouchCenter(new Vector2[]{one.position, two.position}) - 
				GetTouchCenter(new Vector2[]{one.lastPosition, two.lastPosition});
	}
}

[System.Serializable]
public class TouchInfo {
	Touch? touch;
	
#if !(UNITY_ANDROID || UNITY_IOS) || UNITY_EDITOR
	private Vector2 _rawPosition;
	private Vector2 _deltaPosition;
	private float _deltaTime;
	private float _lastTime;
	private int _id;
	private TouchPhase _phase;
	private int _tapCount;
#endif
	private Vector2 _originPosition;
	private Vector2 _lastRestingPosition;
	private float _lastRestingTime;
	private float _lastMovingTime;

	public TouchInfo() {
		touch = null;
	}
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
	public TouchInfo(Touch t) {
		_originPosition = t.position;
		_lastRestingPosition = t.position;
		_lastRestingTime = Time.time;
		_lastMovingTime = Time.time;
		Update(t);
	}
#else
	public TouchInfo(int mouseButton)
	{
		_id = mouseButton;
		_originPosition = Input.mousePosition;
		_rawPosition = Input.mousePosition;
		_deltaPosition = Vector2.zero;
		_deltaTime = Time.deltaTime;
		_lastTime = Time.time;
		_phase = TouchPhase.Began;
		_tapCount = 0;
	}
#endif

	public Vector2 originPosition {
		get{ return _originPosition; }
	}

	public Vector2 position {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
		get{ return ((Touch)touch).position; }
#else
		get{ return _rawPosition; }
#endif
	}

	public Vector2 lastPosition {
		get{ return position - deltaPosition; }
	}

	public Vector2 deltaPosition {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
		get{ return ((Touch)touch).deltaPosition; }
#else
		get{ return _deltaPosition; }
#endif
	}
	public Vector2 rawPosition {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
		get{ return ((Touch)touch).rawPosition; }
#else
		get{ return _rawPosition; }
#endif
	}
	public Vector2 LastRestingPosition {
		get{ return _lastRestingPosition; }
	}
	public float deltaTime {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
		get{ return ((Touch)touch).deltaTime; }
#else
		get{ return _deltaTime; }
#endif
	}
	public int id {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
		get{ return ((Touch)touch).fingerId; }
#else
		get{ return _id; }
#endif
	}
	public TouchPhase phase {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
		get{ return ((Touch)touch).phase; }
#else
		get{ return _phase; }
#endif
	}
	public int tapCount {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
		get{ return ((Touch)touch).tapCount; }
#else
		get{ return _tapCount; }
#endif
	}
	public float lastRestingTime {
		get{ return _lastRestingTime; }
	}
	public float lastMovingTime {
		get{ return _lastMovingTime; }
	}
	public float DistanceTo(Vector2 other)
	{
		return Vector2.Distance(position, other);
	}
	public float DistanceTo(TouchInfo other)
	{
		return DistanceTo(other.position);
	}
	public float LastDistanceTo(Vector2 other)
	{
		return Vector2.Distance(lastPosition, other);
	}
	public float LastDistanceTo(TouchInfo other)
	{
		return LastDistanceTo(other.lastPosition);
	}

	
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
	public void Update(Touch t) {
		touch = t;

		switch(t.phase) {
		case TouchPhase.Stationary:
			_lastRestingPosition = ((Touch)touch).position;
			_lastRestingTime = Time.time;
			break;
		case TouchPhase.Moved:
			_lastMovingTime = Time.time;
			break;
		}
	}
#else
	public void Update() {
		_deltaPosition = (Vector2)Input.mousePosition - _rawPosition;
		_rawPosition = Input.mousePosition;
		_deltaTime = Time.time - _lastTime;
		_lastTime = Time.time;
		
		if(Input.GetMouseButtonUp(_id))
			_phase = TouchPhase.Ended;
		else if(_deltaPosition != Vector2.zero)
			_phase = TouchPhase.Moved;
		else
			_phase = TouchPhase.Stationary;
	}
#endif
}

