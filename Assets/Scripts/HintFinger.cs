using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintFinger : MonoBehaviour
{
    [SerializeField] public string id;
    UITweener tween;

    void Start()
    {
        tween = GetComponent<UITweener>();
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
