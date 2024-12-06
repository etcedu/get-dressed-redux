using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralErrorMessageBox : MonoBehaviour
{
    public static GeneralErrorMessageBox instance;

    [SerializeField] TMPro.TMP_Text topBarLabel;
    [SerializeField] TMPro.TMP_Text label;
    //UITweener tween;

    private void Start()
    {
        if (instance != null)
            Destroy(this.gameObject);
        instance = this;
        gameObject.SetActive(false);
        GetComponent<CanvasGroup>().alpha = 0;

        //tween = GetComponent<UITweener>();
    }

    public static void Show(string _topBarMsg, string _errorMsg)
    {
        instance.topBarLabel.text = _topBarMsg;
        instance.label.text = _errorMsg;
        instance.gameObject.SetActive(true);
        instance.GetComponent<CanvasGroup>().alpha = 1;
    }
}
