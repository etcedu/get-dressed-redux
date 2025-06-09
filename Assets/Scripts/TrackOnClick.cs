using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TrackOnClick : MonoBehaviour 
{
	[SerializeField]	private List<Collider2D> clickTargets;
	[SerializeField] 	private Collider2D zoomTarget;
	[SerializeField]	private bool offset = false;
	[SerializeField]	private Vector3 offsetAmount = Vector3.zero;
	[SerializeField]	private bool useZoomTargetOffset = false;

	[SerializeField]	private bool zoom = false;
	[SerializeField]	private float zoomBorderAmount = 1.2f;

	float zoomMax;

#if UNITY_EDITOR
	public bool maintain;
	private bool wasMaintaining;
	private Vector3 m_Offset;
	private Vector3 m_Pos;
	private float m_Zoom;
	private float m_Size;

	void LateUpdate()
	{
		if(Application.isPlaying)
			return;

		if(maintain)
		{
			if(!wasMaintaining)
			{
				wasMaintaining = true;
				m_Offset = offsetAmount;
				m_Pos = transform.position;
				m_Zoom = zoomBorderAmount;
				//m_Size = transform.localScale;
			}
			offsetAmount = m_Offset;
			zoomBorderAmount = m_Zoom;
			if(transform.position != m_Pos)
			{
				m_Offset += transform.position - m_Pos;
				m_Pos = transform.position;
				offsetAmount = m_Offset;
			}
		} else wasMaintaining = false;
	}
#endif

	void Awake() 
	{
		if(clickTargets.Count == 0)
		{
			clickTargets.Add(GetComponent<Collider2D>());
			if(clickTargets[0] == null)
				this.enabled = false;
		}
		if(zoomTarget == null)
		{
			zoomTarget = GetComponent<Collider2D>();
		}
		zoomMax = zoomTarget.bounds.size.Max();
		if(useZoomTargetOffset)
			offsetAmount += (Vector3)(zoomTarget.offset * zoomTarget.transform.lossyScale.x);
		if(!clickTargets.Contains(zoomTarget))
			zoomTarget.enabled = false;
	}

	public void Focus()
	{
		TargetClicked();
	}

	public void EventTriggerClick()
	{
		TargetClicked();
	}

    void TargetClicked()
	{
		if (offset)
		{
			if(zoom)
				CameraTrack2D.SetTarget(zoomTarget.transform, offsetAmount, zoomBorderAmount, zoomMax);
			else
				CameraTrack2D.SetTarget(zoomTarget.transform, offsetAmount, zoomMax);
		} else if(zoom)
			CameraTrack2D.SetTarget(zoomTarget.transform, zoomBorderAmount, zoomMax);
		else
			CameraTrack2D.SetTarget(zoomTarget.transform, zoomMax);
	}
}
