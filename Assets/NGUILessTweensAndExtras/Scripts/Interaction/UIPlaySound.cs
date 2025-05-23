//-------------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2017 Tasharen Entertainment Inc
//-------------------------------------------------
//
//   Edited for use with UUI and non-NGUI games
//               Garrett Kimball
//             Simcoach Games 2016
//
//-------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Plays the specified sound.
/// </summary>

[AddComponentMenu("NGUILessTweens/Extras/Play Sound")]
public class UIPlaySound : MonoBehaviour
{
	public enum Trigger
	{
		OnClick,
		OnMouseOver,
		OnMouseOut,
		OnPress,
		OnRelease,
		Custom,
		OnEnable,
		OnDisable,
	}

	public AudioClip audioClip;
	public Trigger trigger = Trigger.OnClick;

	[Range(0f, 1f)] public float volume = 1f;
	[Range(0f, 2f)] public float pitch = 1f;

	bool mIsOver = false;

	bool canPlay
	{
		get
		{
			if (!enabled) return false;
			Button btn = GetComponent<Button>();
			return (btn == null || btn.isActiveAndEnabled);
		}
	}

	void OnEnable ()
	{
		if (trigger == Trigger.OnEnable)
			NGUITools.PlaySound(audioClip, volume, pitch);
	}

	void OnDisable ()
	{
		if (trigger == Trigger.OnDisable)
			NGUITools.PlaySound(audioClip, volume, pitch);
	}

	void OnHover (bool isOver)
	{
		if (trigger == Trigger.OnMouseOver)
		{
			if (mIsOver == isOver) return;
			mIsOver = isOver;
		}

		if (canPlay && ((isOver && trigger == Trigger.OnMouseOver) || (!isOver && trigger == Trigger.OnMouseOut)))
			NGUITools.PlaySound(audioClip, volume, pitch);
	}

	void OnPress (bool isPressed)
	{
		if (trigger == Trigger.OnPress)
		{
			if (mIsOver == isPressed) return;
			mIsOver = isPressed;
		}

		if (canPlay && ((isPressed && trigger == Trigger.OnPress) || (!isPressed && trigger == Trigger.OnRelease)))
			NGUITools.PlaySound(audioClip, volume, pitch);
	}

	void OnClick ()
	{
		if (canPlay && trigger == Trigger.OnClick)
			NGUITools.PlaySound(audioClip, volume, pitch);
	}

	void OnSelect (bool isSelected)
	{
		if (canPlay && (!isSelected))
			OnHover(isSelected);
	}

	public void Play ()
	{
		NGUITools.PlaySound(audioClip, volume, pitch);
	}
}
