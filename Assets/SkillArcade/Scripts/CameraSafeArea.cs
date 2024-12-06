using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSafeArea : MonoBehaviour
{
    Camera thisCamera;

    // Start is called before the first frame update
    void Start()
    {
        Rect safeArea = Screen.safeArea;
        thisCamera = GetComponent<Camera>();

#if UNITY_EDITOR
        if (Screen.width == 2688 && Screen.height == 1242)
        {
            safeArea = new Rect(132, 63, 2424, 1179);
        }
#endif
        thisCamera.pixelRect = safeArea;
    }
}
