using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ScrollRect))]
public class AutoScroll : MonoBehaviour, IDragHandler
{
    public Vector2 targetVelocity = Vector3.up;
    public float delayToRestart = 2;
    ScrollRect sr;
    bool paused;
    float pausedTime;

    void Awake()
    {
        sr = GetComponent<ScrollRect>();
        paused = false;
        ResetScroll();
    }

    void Update()
    {
        if (!paused && sr.velocity != targetVelocity)
        {
            sr.velocity = targetVelocity;
        }


        if (paused)
        {
            pausedTime += Time.deltaTime;
            if (pausedTime >= delayToRestart)
            {
                paused = false;
            }
        }

    }

    public void ResetScroll()
    {
        sr.verticalNormalizedPosition = 1;
        PauseOnDrag();
    }



    public void PauseOnDrag()
    {
        paused = true;
        pausedTime = 0;
    }

    public void OnDrag(PointerEventData eventData)
    {
        PauseOnDrag();
    }
}

