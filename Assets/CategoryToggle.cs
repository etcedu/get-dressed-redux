using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CategoryToggle : MonoBehaviour
{
    [SerializeField]
    UnityEvent offEvents;
    [SerializeField]
    UnityEvent onEvents;

    public void SetToOff()
    {
        offEvents.Invoke();
    }

    public void SetToOn()
    {
        onEvents.Invoke();
    }
}
