using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintFinger : MonoBehaviour
{
    [SerializeField] public string id;
    [SerializeField] GameObject linkedObject;
    UITweener tween;

    void Start()
    {
        tween = GetComponent<UITweener>();
    }

    private void Update()
    {
        if (linkedObject != null)
        {
            transform.position = linkedObject.transform.position;
        }
    }

    public void Show()
    {
        tween?.PlayForward();
    }
    public void Hide()
    {
        tween?.PlayReverse();
    }
}
