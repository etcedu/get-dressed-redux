﻿using UnityEngine;
using System.Collections.Generic;

public class TrackOnClick : MonoBehaviour {
	[SerializeField]
	private List<Collider2D> clickTargets;
	[SerializeField]
	private Collider2D zoomTarget;
	[SerializeField]
	private ClickType actOn;
	[SerializeField]
	private bool offset = false;
	[SerializeField]
	private Vector3 offsetAmount = Vector3.zero;
	[SerializeField]
	private bool useZoomTargetOffset = false;

	[SerializeField]
	private bool zoom = false;
	[SerializeField]
	private float zoomBoarderAmount = 1.2f;

	[SerializeField]
	private bool onClickOverride = false;
	[SerializeField]
	private List<EventDelegate> onClick = new List<EventDelegate>();

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
				m_Zoom = zoomBoarderAmount;
				//m_Size = transform.localScale;
			}
			offsetAmount = m_Offset;
			zoomBoarderAmount = m_Zoom;
			if(transform.position != m_Pos)
			{
				m_Offset += transform.position - m_Pos;
				m_Pos = transform.position;
				offsetAmount = m_Offset;
			}
		} else wasMaintaining = false;
	}
#endif

	public void AddToOnClick(EventDelegate newEvent)
	{
		onClick.Add(newEvent);
	}

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

		if(actOn == ClickType.CLICK)
			foreach(Collider2D c2D in clickTargets)
				c2D.gameObject.AddMissingComponent<ForwardTouch>().Clicked += TargetClicked;
		else if(actOn == ClickType.DOUBLECLICK)
			foreach(Collider2D c2D in clickTargets)
				c2D.gameObject.AddMissingComponent<ForwardTouch>().DoubleClicked += TargetClicked;
	}

	public void Focus()
	{
		TargetClicked();
	}

	void TargetClicked(GameObject target)
	{
		if(onClickOverride)
			EventDelegate.Execute(onClick);
		else
			TargetClicked();
	}

	void TargetClicked()
	{
		if(offset)
		{
			if(zoom)
				CameraTrack2D.SetTarget(zoomTarget.transform, offsetAmount, zoomBoarderAmount, zoomMax);
			else
				CameraTrack2D.SetTarget(zoomTarget.transform, offsetAmount, zoomMax);
		} else if(zoom)
			CameraTrack2D.SetTarget(zoomTarget.transform, zoomBoarderAmount, zoomMax);
		else
			CameraTrack2D.SetTarget(zoomTarget.transform, zoomMax);
	}

	private enum ClickType
	{
		CLICK, DOUBLECLICK
	}
}