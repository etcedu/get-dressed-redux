using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustScaleWithAspectRatio : MonoBehaviour {

    public Canvas refCanvas;
    public float scalingAmount;
    public float maxScale;
    public bool fourByThreeFix;

    RectTransform rectTrans;

    private void Start()
    {
        UICanvasHelper.onResolutionChange.AddListener(()=> { AdjustScale(); });
        AdjustScale();
    }

    [ContextMenu("AdjustScale")]
    private void AdjustScale()
    {
        if (rectTrans == null) rectTrans = GetComponent<RectTransform>();

        float ar = UICanvasHelper.GetMainGameViewSize().y / UICanvasHelper.GetMainGameViewSize().x;

        //Debug.Log("Adjusting for: " + ar);

        float scale =  Mathf.Min(maxScale, scalingAmount * ar);

        if (fourByThreeFix)
        {
            if (ar >= 0.7f)
                scale = 0.75f;
            else
                scale = 0.9f;
        }

        rectTrans.localScale = new Vector3(scale,scale, 1);
    }

}
