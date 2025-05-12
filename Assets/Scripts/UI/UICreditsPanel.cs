using UnityEngine;
using UnityEngine.UI;

public class UICreditsPanel : MonoBehaviour
{
    [SerializeField] float autoScrollSpeed = 10;

    // [Space]
    // [SerializeField] bool hideHideables;
    // [SerializeField] List<GameObject> hideableObjects;

    TweenAlpha _tweenAlpha;
    ScrollRect _scrollRect;
    
    bool _autoScroll = true;

    void Awake()
    {
        _scrollRect = GetComponentInChildren<ScrollRect>();
        if (!_scrollRect)
        {
            Debug.LogError("Could not find scroll rect.");
        }

        _tweenAlpha = GetComponent<TweenAlpha>();
        if (!_tweenAlpha)
        {
            Debug.LogError("Could not find tween alpha.");
        }
        
        // if(hideHideables)
        // {
        //     foreach (var hideableObject in hideableObjects)
        //         hideableObject.SetActive(false);
        // }
    }

    void Update()
    {
        if (!_autoScroll || !gameObject.activeInHierarchy)
            return;

        _scrollRect.velocity = new Vector2(0, autoScrollSpeed);
    }

    public void Button_Open()
    {
        _autoScroll = true;
        _scrollRect.normalizedPosition = new Vector2(0, 1);
        
        _tweenAlpha.PlayForward();
    }

    public void Button_Close()
    {
        _autoScroll = false;
        _tweenAlpha.PlayReverse();
    }

    //WTP: Hookup to event trigger to disable autoscroll on user input
    public void Button_OnInput()
    {
        _autoScroll = false;
    }
}