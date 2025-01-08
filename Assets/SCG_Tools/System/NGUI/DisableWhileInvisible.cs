using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DisableWhileInvisible : MonoBehaviour {
    
    CanvasGroup mCanvas;

    [Tooltip("How visible the element should be to be interactable")]
    [SerializeField] float alphaCutoff = 0.001f;

    // Use this for initialization
    void Start()
    {
        mCanvas = GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        if (mCanvas != null)
        {
            mCanvas.interactable = mCanvas.alpha >= alphaCutoff;
            mCanvas.blocksRaycasts = mCanvas.alpha >= alphaCutoff;
        }
    }
}
