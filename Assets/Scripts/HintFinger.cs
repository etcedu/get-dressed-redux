using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintFinger : MonoBehaviour
{
    [SerializeField] public string id;
    [SerializeField] GameObject linkedObject;
    UITweener tween;

    private void Update()
    {
        if (linkedObject != null)
        {
            transform.position = linkedObject.transform.position;
        }
    }

    public void Show()
    {
        if (!tween)
            tween = GetComponent<UITweener>();
        tween?.PlayForward();
    }
    public void Hide()
    {
        if (!tween)
            tween = GetComponent<UITweener>();
        tween?.PlayReverse();
    }
}
