using UnityEngine;
using System.Collections;

public class PlaySoundList : UIPlaySound {
	public AudioClip[] soundList = new AudioClip[0];
	[SerializeField]
	private PlayStyle soundSelection;
	[SerializeField]
	private AssignOrder assignOrder;
	private int soundIndex = -1;
	private bool onceThrough = false;
	
	
	protected override void OnEnable ()
	{
		if (!(canPlay && trigger != Trigger.OnEnable))
			return;
		if(assignOrder == AssignOrder.ASSIGN_THEN_PLAY)
			Assign();
		base.OnEnable();
		if(assignOrder == AssignOrder.PLAY_THEN_ASSIGN)
			StartCoroutine(delayAssign());
	}
	
	protected override void OnDisable ()
	{
		if (!(canPlay && trigger == Trigger.OnDisable))
			return;
		if(assignOrder == AssignOrder.ASSIGN_THEN_PLAY)
			Assign();
		base.OnDisable();
		if(assignOrder == AssignOrder.PLAY_THEN_ASSIGN)
			StartCoroutine(delayAssign());
	}
	
	protected override void OnHover (bool isOver)
	{
		if (!(canPlay && mIsOver != isOver && ((isOver && trigger == Trigger.OnMouseOver) || (!isOver && trigger == Trigger.OnMouseOut))))
			return;
		if(assignOrder == AssignOrder.ASSIGN_THEN_PLAY)
			Assign();
		base.OnHover(isOver);
		if(assignOrder == AssignOrder.PLAY_THEN_ASSIGN)
			StartCoroutine(delayAssign());
	}
	
	protected override void OnPress (bool isPressed)
	{
		if (!(canPlay && mIsOver != isPressed && ((isPressed && trigger == Trigger.OnPress) || (!isPressed && trigger == Trigger.OnRelease))))
			return;
		if(assignOrder == AssignOrder.ASSIGN_THEN_PLAY)
			Assign();
		base.OnPress(isPressed);
		if(assignOrder == AssignOrder.PLAY_THEN_ASSIGN)
			StartCoroutine(delayAssign());
	}
	
	protected override void OnClick ()
	{
		if (!(canPlay && trigger == Trigger.OnClick))
			return;
		if(assignOrder == AssignOrder.ASSIGN_THEN_PLAY)
			Assign();
		base.OnClick();
		if(assignOrder == AssignOrder.PLAY_THEN_ASSIGN)
			StartCoroutine(delayAssign());
	}
	
	protected override void OnSelect (bool isSelected)
	{
		if (!(canPlay && (!isSelected || UICamera.currentScheme == UICamera.ControlScheme.Controller)))
			return;
		if(assignOrder == AssignOrder.ASSIGN_THEN_PLAY)
			Assign();
		base.OnSelect(isSelected);
		if(assignOrder == AssignOrder.PLAY_THEN_ASSIGN)
			StartCoroutine(delayAssign());
	}

	public override void Play()
	{
		if(SoundMuter.Muted) return;
		if(!onceThrough && assignOrder == AssignOrder.ASSIGN_THEN_PLAY)
			Assign();
		base.Play();
		if(!onceThrough && assignOrder == AssignOrder.PLAY_THEN_ASSIGN)
			StartCoroutine(delayAssign());
	}

	IEnumerator delayAssign()
	{
		yield return null;
		Assign();
	}

	public void ForceAssign()
	{
		Assign();
	}

	void Assign()
	{
		if(soundList.Length == 0 || (soundSelection == PlayStyle.RANDOM_SINGLE && onceThrough)) return;

		onceThrough = true;
		if(soundSelection == PlayStyle.SEQUENTIAL)
			soundIndex = Mathf.Clamp(soundIndex + 1, 0, soundList.Length-1);
		else
			soundIndex = UnityEngine.Random.Range(0, soundList.Length - 1);

		audioClip = soundList[soundIndex];
	}

	[System.Serializable]
	private enum PlayStyle
	{
		SEQUENTIAL, RANDOM_ALWAYS, RANDOM_SINGLE
	}
	[System.Serializable]
	private enum AssignOrder
	{
		ASSIGN_THEN_PLAY, PLAY_THEN_ASSIGN
	}
}
