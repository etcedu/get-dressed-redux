using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Simcoach.SkillArcade;

[System.Serializable]
public class TweenGroup_SA {
	[SerializeField]
	private List<GameObject> targets;
	[SerializeField]
	private int group;
	[SerializeField]
	private TweenDirection direction;

    public List<GameObject> Targets
    {
        get { return targets; }
        set { targets = value; }
    }

	public static UITweener_SA[] Tweens(GameObject target, int group) {
        UITweener_SA[] tweens = target.GetComponents<UITweener_SA>();
		if(tweens == null)
			return new UITweener_SA[0];
		return tweens.Where(tween=>tween.tweenGroup == group).ToArray();
	}
	
	public UITweener_SA[] Tweens() {
		List<UITweener_SA> tweens = new List<UITweener_SA>();
		foreach(GameObject target in targets) {
			if(target == null) continue;
			tweens.AddRange(target.GetComponents<UITweener_SA>());
		}
		if(tweens.Count == 0)
			return new UITweener_SA[0];
		return tweens.Where(tween=>tween.tweenGroup == group).ToArray();
	}

	public float LongestTween {
		get{
			float longest = 0;
			foreach(GameObject target in targets) {
				if(target == null) continue;
                UITweener_SA[] targetTweens = target.GetComponents<UITweener_SA>();

				foreach(UITweener_SA tween in targetTweens) {
					if(tween == null || tween.tweenGroup != group) continue;
					
					if(tween.duration > longest)
						longest = tween.duration;
				}
			}

			return longest;
		}
	}

	public void Play() {
        UITweener_SA[] tweens = Tweens();
		switch(direction) {
		case TweenDirection.FORWARD:
			foreach(UITweener_SA tween in tweens)
				tween.PlayForward();
			break;
		case TweenDirection.REVERSE:
			foreach(UITweener_SA tween in tweens)
				tween.PlayReverse();
			break;
		case TweenDirection.FORWARD_FROM_BEGINNING:
			foreach(UITweener_SA tween in tweens)
				tween.PlayForward_FromBeginning();
			break;
		case TweenDirection.REVERSE_FROM_BEGINNING:
			foreach(UITweener_SA tween in tweens)
				tween.PlayReverse_FromBeginning();
		break;
		}
	}
	
	public void PlayForward() {
        UITweener_SA[] tweens = Tweens();
		direction = TweenDirection.FORWARD;
		foreach(UITweener_SA tween in tweens)
			tween.PlayForward();
	}
	
	public void PlayForward_FromBeginning() {
        UITweener_SA[] tweens = Tweens();
		direction = TweenDirection.FORWARD_FROM_BEGINNING;
		foreach(UITweener_SA tween in tweens)
			tween.PlayForward_FromBeginning();
	}
	
	public void PlayReverse() {
        UITweener_SA[] tweens = Tweens();
		direction = TweenDirection.REVERSE;
		foreach(UITweener_SA tween in tweens)
			tween.PlayReverse();
	}
	
	public void PlayReverse_FromBeginning() {
        UITweener_SA[] tweens = Tweens();
		direction = TweenDirection.REVERSE_FROM_BEGINNING;
		foreach(UITweener_SA tween in tweens)
			tween.PlayReverse_FromBeginning();
	}

    public void Play(GameObject holder, List<EventDelegate> events)
    {
        UITweener_SA[] tweens = Tweens();
        int longIndex = -1;
        float longest = 0f;

        for (int i = 0; i < tweens.Length; i++ )
        {
            if (tweens[i].duration > longest)
            {
                longest = tweens[i].duration;
                longIndex = i;
            }
        }
        
        EventDelegate.Execute(events);
        

        switch (direction)
        {
            case TweenDirection.FORWARD:
                foreach (UITweener_SA tween in tweens)
                    tween.PlayForward();
                break;
            case TweenDirection.REVERSE:
                foreach (UITweener_SA tween in tweens)
                    tween.PlayReverse();
                break;
            case TweenDirection.FORWARD_FROM_BEGINNING:
                foreach (UITweener_SA tween in tweens)
                    tween.PlayForward_FromBeginning();
                break;
            case TweenDirection.REVERSE_FROM_BEGINNING:
                foreach (UITweener_SA tween in tweens)
                    tween.PlayReverse_FromBeginning();
                break;
        }
    }

    public void Sample(float factor)
    {
        UITweener_SA[] tweens = Tweens();
        foreach (UITweener_SA tween in tweens)
        {
            tween.Sample(factor, false);
        }
    }

	public enum TweenDirection {
		FORWARD, REVERSE, FORWARD_FROM_BEGINNING, REVERSE_FROM_BEGINNING
	}

    private class EventRemover : MonoBehaviour
    {
        public UITweener tween;
        public List<EventDelegate> events;

        void RemoveEvents()
        {
            foreach (EventDelegate eve in events)
                tween.onFinished.Remove(eve);
            Destroy(this);
        }
    }
}

