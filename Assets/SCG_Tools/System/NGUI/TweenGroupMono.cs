using UnityEngine;
using System.Collections;

public class TweenGroupMono : MonoBehaviour
{
    public TweenGroup group;
	public TweenGroup_SA group_SA;

    public void Play()
    {
        group.Play();
        group_SA.Play();
    }

	public void PlayForward()
	{
		group.PlayForward();
        group_SA.PlayForward();
    }

	public void PlayForward_FromBeginning()
	{
		group.PlayForward_FromBeginning();
        group_SA.PlayForward_FromBeginning();
    }

	public void PlayReverse()
	{
		group.PlayReverse();
        group_SA.PlayReverse();
    }
	
	public void PlayReverse_FromBeginning()
	{
		group.PlayReverse_FromBeginning();
        group_SA.PlayReverse_FromBeginning();
    }
}
