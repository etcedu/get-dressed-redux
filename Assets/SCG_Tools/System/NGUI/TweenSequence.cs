using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A sequence of tweens that play one after another, with the option to play on awake and/or loop the sequence.
/// </summary>
public class TweenSequence : MonoBehaviour
{
	public UITweener[] tweens;
	public bool playOnAwake = true, loop = false;
	int _currentTween = -1;

	void Awake() {
		foreach(UITweener tween in tweens) {
			if(tween.onFinished.Find(eD=>eD.target == this && eD.methodName == "PlayNext") == null)
				tween.onFinished.Add(new EventDelegate(this, "PlayNext"));
		}
		if(playOnAwake)
			PlayNext();
	}

	public void PlayNext() {
		++_currentTween;
		if(_currentTween >= tweens.Length && loop) _currentTween = 0;
		if(_currentTween < tweens.Length)
			tweens[_currentTween].PlayForward_FromBeginning();
	}
}

