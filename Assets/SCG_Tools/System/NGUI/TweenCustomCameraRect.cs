using UnityEngine;
using System.Collections;

[AddComponentMenu("NGUI/Tween/Tween Custom Camera Rect")]
public class TweenCustomCameraRect : UITweener
{
	public CustomCameraRect target;
	public CustomCameraRect.VerticalOffset toTop, toBottom;
	public CustomCameraRect.HorizontalOffset toLeft, toRight;

	private Camera _camera;
	private float? _fromTop, _fromBottom;
	private float? _fromLeft, _fromRight;
	private float? _toTop, _toBottom;
	private float? _toLeft, _toRight;

	private float top, bottom, left, right;
	
	/// <summary>
	/// Interpolate the rect's sides.
	/// </summary>
	
	protected override void OnUpdate(float factor, bool isFinished)
	{
		if (_fromTop == null)
		{
			_camera = target.GetComponent<Camera>();
			_fromTop = _camera.rect.yMax;
			_fromBottom = _camera.rect.yMin;
			_fromLeft = _camera.rect.xMin;
			_fromRight = _camera.rect.xMax;
			CalculateTo();
		}

		top = (float)_fromTop * (1f - factor) + (float)_toTop * factor;
		bottom = (float)_fromBottom * (1f - factor) + (float)_toBottom * factor;
		left = (float)_fromLeft * (1f - factor) + (float)_toLeft * factor;
		right = (float)_fromRight * (1f - factor) + (float)_toRight * factor;

		_camera.rect = new Rect(left, bottom, right - left, top - bottom);
		
		if (isFinished)
		{
			target.Top = toTop;
			target.Bottom = toBottom;
			target.Left = toLeft;
			target.Right = toRight;
		}
	}


	void CalculateTo()
	{
		float baseHeight = target.BaseOffContentHeight ? target.ContentHeight : Screen.height;
		
		_toTop = 1 - (float)toTop.pixelOffset / baseHeight;
		if(toTop.relativeTo == CustomCameraRect.VerticalSide.CENTER)
			_toTop -= .5f;
		else if(toTop.relativeTo == CustomCameraRect.VerticalSide.BOTTOM)
			_toTop -= 1f;
		else if(toTop.relativeTo == CustomCameraRect.VerticalSide.CUSTOM)
			_toTop -= toTop.percentOffset;
		
		// Bottom
		_toBottom = (float)toBottom.pixelOffset / baseHeight;
		if(toBottom.relativeTo == CustomCameraRect.VerticalSide.CENTER)
			_toBottom += .5f;
		else if(toBottom.relativeTo == CustomCameraRect.VerticalSide.TOP)
			_toBottom += 1f;
		else if(toBottom.relativeTo == CustomCameraRect.VerticalSide.CUSTOM)
			_toBottom += toBottom.percentOffset;
		
		float baseWidth = target.BaseOffContentHeight ? ((float)Screen.width / Screen.height) * target.ContentHeight : Screen.width;
		
		// Left
		_toLeft = (float)toLeft.pixelOffset / baseWidth;
		if(toLeft.relativeTo == CustomCameraRect.HorizontalSide.CENTER)
			_toLeft += .5f;
		else if(toLeft.relativeTo == CustomCameraRect.HorizontalSide.RIGHT)
			_toLeft += 1f;
		else if(toLeft.relativeTo == CustomCameraRect.HorizontalSide.CUSTOM)
			_toLeft += toLeft.percentOffset;
		
		// Right
		_toRight = 1f - (float)toRight.pixelOffset / baseWidth;
		if(toRight.relativeTo == CustomCameraRect.HorizontalSide.CENTER)
			_toRight -= .5f;
		else if(toRight.relativeTo == CustomCameraRect.HorizontalSide.LEFT)
			_toRight -= 1f;
		else if(toRight.relativeTo == CustomCameraRect.HorizontalSide.CUSTOM)
			_toRight -= toRight.percentOffset;
	}
	
	void OnDisable()
	{
		if (_fromTop != null)
		{
			_fromTop = null;
			_fromBottom = null;
			_fromLeft = null;
			_fromRight = null;
			_toTop = null;
			_toBottom = null;
			_toLeft = null;
			_toRight = null;
		}
	}
	
	/// <summary>
	/// Start the tweening operation.
	/// </summary>
	
	static public TweenCustomCameraRect Begin(GameObject go, float duration, CustomCameraRect.VerticalOffset Top, 
	                                          CustomCameraRect.VerticalOffset Bottom, CustomCameraRect.HorizontalOffset Left,
	                                          CustomCameraRect.HorizontalOffset Right)
	{
		TweenCustomCameraRect comp = UITweener.Begin<TweenCustomCameraRect>(go, duration);
		/*GameObject f = new GameObject("from");
        f.transform.position = go.transform.position;
        f.transform.rotation = go.transform.rotation;
        f.transform.localScale = go.transform.localScale;*/
		comp._fromTop = null;
		comp.toTop = Top;
		comp.toBottom = Bottom;
		comp.toLeft = Left;
		comp.toRight = Right;
		
		if (duration <= 0f)
		{
			comp.Sample(1f, true);
			comp.enabled = false;
		}
		return comp;
	}
}

