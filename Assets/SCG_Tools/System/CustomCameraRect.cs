using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent (typeof(Camera))]
public class CustomCameraRect : MonoBehaviour
{
	[SerializeField]
	private WhenToRun execute;
	[SerializeField]
	private VerticalOffset top = new VerticalOffset(VerticalSide.TOP),
							bottom = new VerticalOffset(VerticalSide.BOTTOM);
	public VerticalOffset Top
	{
		get{ return top; }
		set{ top = value;
			_update = true; }
	}
	public VerticalOffset Bottom
	{
		get{ return Bottom; }
		set{ bottom = value;
			_update = true; }
	}
	[SerializeField]
	private HorizontalOffset left = new HorizontalOffset(HorizontalSide.LEFT), 
								right = new HorizontalOffset(HorizontalSide.RIGHT);
	public HorizontalOffset Left
	{
		get{ return left; }
		set{ left = value;
			_update = true; }
	}
	public HorizontalOffset Right
	{
		get{ return right; }
		set{ right = value;
			_update = true; }
	}
	[SerializeField]
	private bool baseOffContentHeight;
	public bool BaseOffContentHeight
	{
		get{ return baseOffContentHeight; }
	}
	[SerializeField]
	private int contentHeight = 720;
	public int ContentHeight
	{
		get{ return contentHeight; }
	}

	private Camera _camera;
	private bool _update;

	void OnEnable()
	{
#if UNITY_EDITOR
		if(!Application.isPlaying)
			return;
#endif
		if(execute == WhenToRun.OnEnable)
			SetRect();
	}

	void Start ()
	{
#if UNITY_EDITOR
		if(!Application.isPlaying)
			return;
#endif
		if(execute == WhenToRun.OnStart)
			SetRect();
	}

	void Update ()
	{
#if UNITY_EDITOR
		if(!Application.isPlaying)
		{
			SetRect();
			return;
		}
#endif
		if(execute == WhenToRun.OnUpdate || _update)
		{
			SetRect();
			_update = false;
		}
	}

	void SetRect()
	{
		float baseHeight = baseOffContentHeight ? contentHeight : Screen.height;

		float _top = 1 - (float)top.pixelOffset / baseHeight;
		if(top.relativeTo == VerticalSide.CENTER)
			_top -= .5f;
		else if(top.relativeTo == VerticalSide.BOTTOM)
			_top -= 1f;
		else if(top.relativeTo == VerticalSide.CUSTOM)
			_top -= top.percentOffset;

		// Bottom
		float _bottom = (float)bottom.pixelOffset / baseHeight;
		if(bottom.relativeTo == VerticalSide.CENTER)
			_bottom += .5f;
		else if(bottom.relativeTo == VerticalSide.TOP)
			_bottom += 1f;
		else if(bottom.relativeTo == VerticalSide.CUSTOM)
			_bottom += bottom.percentOffset;

		float baseWidth = baseOffContentHeight ? ((float)Screen.width / Screen.height) * contentHeight : Screen.width;

		// Left
		float _left = (float)left.pixelOffset / baseWidth;
		if(left.relativeTo == HorizontalSide.CENTER)
			_left += .5f;
		else if(left.relativeTo == HorizontalSide.RIGHT)
			_left += 1f;
		else if(left.relativeTo == HorizontalSide.CUSTOM)
			_left += left.percentOffset;

		// Right
		float _right = 1f - (float)right.pixelOffset / baseWidth;
		if(right.relativeTo == HorizontalSide.CENTER)
			_right -= .5f;
		else if(right.relativeTo == HorizontalSide.LEFT)
			_right -= 1f;
		else if(right.relativeTo == HorizontalSide.CUSTOM)
			_right -= right.percentOffset;

		if(_camera == null)
			_camera = GetComponent<Camera>();

		_camera.rect = new Rect(_left, _bottom, _right - _left, _top - _bottom);

		/*Debug.Log("left: " + _left + ", top: " + _top + 
		          ", width: " + (_right - _left) + ", height: " + (_bottom - _top));*/
	}

	[System.Serializable]
	public class VerticalOffset
	{
		public VerticalSide relativeTo;
		[Range(0, 1)]
		public float percentOffset;
		public int pixelOffset;

		public VerticalOffset(VerticalSide side)
		{
			relativeTo = side;
		}
	}
	[System.Serializable]
	public class HorizontalOffset
	{
		public HorizontalSide relativeTo;
		[Range(0, 1)]
		public float percentOffset;
		public int pixelOffset;

		public HorizontalOffset(HorizontalSide side)
		{
			relativeTo = side;
		}
	}

	[System.Serializable]
	private enum WhenToRun
	{
		OnEnable, OnStart, OnUpdate
	}
	[System.Serializable]
	public enum VerticalSide
	{
		TOP, CENTER, BOTTOM, CUSTOM
	}
	[System.Serializable]
	public enum HorizontalSide
	{
		LEFT, CENTER, RIGHT, CUSTOM
	}
}

