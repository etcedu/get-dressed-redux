using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchSizeOfOtherCamera : MonoBehaviour
{
    [SerializeField] Camera otherCam;
    Camera thisCam;

    private void Start()
    {
        thisCam = GetComponent<Camera>();
    }

    void Update()
    {
        thisCam.orthographicSize = otherCam.orthographicSize;
    }
}
