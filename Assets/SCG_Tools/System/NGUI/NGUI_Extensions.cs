using UnityEngine;

public static class NGUI_Extensions
{
	public static void Pause(this UITweener t) {
		t.enabled = false;
	}
	
	public static void PlayForward_FromBeginning(this UITweener t) {
		t.PlayForward();
		t.ResetToBeginning();
		t.PlayForward();
	}
	public static void PauseForward_FromBeginning(this UITweener t) {
		t.PlayForward();
		t.Pause();
		t.ResetToBeginning();
	}
	public static void PlayReverse_FromBeginning(this UITweener t) {
		t.PlayReverse();
		t.ResetToBeginning();
		t.PlayReverse();
	}
	public static void PauseReverse_FromBeginning(this UITweener t) {
		t.PlayReverse();
		t.Pause();
		t.ResetToBeginning();
	}

	public static Vector2 CalculateOffsetFromStart(this UICenterOnChild center, Transform target)
	{
		UIScrollView mScrollView = NGUITools.FindInParents<UIScrollView>(center.gameObject);
		if (target != null && mScrollView != null && mScrollView.panel != null)
		{
			/*Vector3[] corners = mScrollView.panel.worldCorners;
			Vector3 panelCenter = (corners[2] + corners[0]) * 0.5f;

			Transform panelTrans = mScrollView.panel.cachedTransform;
			GameObject mCenteredObject = target.gameObject;
			
			// Figure out the difference between the chosen child and the panel's center in local coordinates
			Vector3 cp = panelTrans.InverseTransformPoint(target.position);
			Vector3 cc = panelTrans.InverseTransformPoint(panelCenter);
			Vector3 localOffset = cp - cc;*/
			return mScrollView.panel.drawCallOffset;
		}
		return Vector2.zero;
	}
}

