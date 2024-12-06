using UnityEngine;
using System.Collections;

public class ToggleEnabled : MonoBehaviour
{
    [SerializeField]
    private bool toggleButtonIsEnable = true;
    [SerializeField]
    private bool enabledByDefault = true;

    void Awake()
    {
        if (gameObject.GetComponent<UIButton>() != null && toggleButtonIsEnable)
        {
            UIButton uibutton = gameObject.GetComponent<UIButton>();
            uibutton.isEnabled = enabledByDefault;
        }
        else
            gameObject.SetActive(enabledByDefault);
    }

    public void Toggle()
    {
        if (gameObject.GetComponent<UIButton>() != null && toggleButtonIsEnable) {
            UIButton uibutton = gameObject.GetComponent<UIButton>();
            uibutton.isEnabled = !uibutton.isEnabled;
        }
        else
            gameObject.SetActive(!gameObject.activeSelf);
    }

    public void Enable()
    {
        if (gameObject.GetComponent<UIButton>() != null && toggleButtonIsEnable)
        {
            gameObject.GetComponent<UIButton>().isEnabled = true;
        }
        else
            gameObject.SetActive(true);
    }

    public void Disable()
    {
        if (gameObject.GetComponent<UIButton>() != null && toggleButtonIsEnable)
        {
            gameObject.GetComponent<UIButton>().isEnabled = false;
        }
        else
            gameObject.SetActive(false);
    }
    

}
