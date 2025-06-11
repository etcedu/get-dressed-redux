using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ClothingNameTextObject : MonoBehaviour
{
    TMP_Text text;
    TweenAlpha tween;    

    public void Show(string message, GameObject followObject)
    {
        if(text == null)
            text = GetComponent<TMP_Text>();
        text.text = message;

        if (tween == null)
            tween = GetComponent<TweenAlpha>();
        tween?.PlayForward_FromBeginning();

        transform.SetParent(followObject.transform);
        transform.position = followObject.transform.position;
    }

}
