using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Toggle;

[RequireComponent(typeof(Toggle))]
public class ToggleCustom : MonoBehaviour
{
    Toggle toggle;

    [SerializeField]
    Graphic[] graphics;

    void OnToggleChanged(bool bValue)
    {
        PlayEffect(toggle.toggleTransition == ToggleTransition.None);
    }

    private void PlayEffect(bool instant)
    {
        if (graphics == null || graphics.Length == 0)
            return;

#if UNITY_EDITOR
        if (!Application.isPlaying)
            for (int i = 0; i < graphics.Length; i++)
                graphics[i].canvasRenderer.SetAlpha(toggle.isOn ? 1f : 0f);
        else
#endif
            for (int i = 0; i < graphics.Length; i++)
                graphics[i].CrossFadeAlpha(toggle.isOn ? 1f : 0f, instant ? 0f : 0.1f, true);
    }


    void OnEnable()
    {
        toggle = GetComponent<Toggle>();

        toggle.onValueChanged.AddListener(OnToggleChanged);
        PlayEffect(true);
    }
    void OnDisable()
    {
        toggle.onValueChanged.RemoveListener(OnToggleChanged);
    }

    public void SetIsOnWithoutNotify(bool on) 
    { 
        toggle.SetIsOnWithoutNotify(on);
        PlayEffect(true);
    }

}
