using UnityEngine;
using System.Collections;

public class TweenGroupMono : MonoBehaviour
{
    public TweenGroup group;

    public void Play()
    {
        group.Play();
    }

	public void PlayForward()
	{
		group.PlayForward();
	}

	public void PlayForward_FromBeginning()
	{
		group.PlayForward_FromBeginning();
	}

	public void PlayReverse()
	{
		group.PlayReverse();
	}
	
	public void PlayReverse_FromBeginning()
	{
		group.PlayReverse_FromBeginning();
	}
}
