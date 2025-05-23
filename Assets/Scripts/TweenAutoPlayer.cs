using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UITweener))]
public class TweenAutoPlayer : MonoBehaviour
{
    [SerializeField] bool resetOnDisable = true;
    [Header("Optional")]
    [SerializeField] List<UITweener> tweeners;

    void GetTweeners() => tweeners.AddRange(GetComponents<UITweener>());

    void OnEnable()
    {
        if(tweeners.Count == 0) GetTweeners();

        foreach (var tweener in tweeners)
        {
            tweener.PlayForward_FromBeginning();    
        }
        
    }

    void OnDisable()
    {
        if (tweeners.Count == 0) GetTweeners();

        if (resetOnDisable)
        {
            foreach (var tweener in tweeners)
            {
                tweener.ResetToBeginning();
            }
        }
    }
}