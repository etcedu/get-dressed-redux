using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleSet : MonoBehaviour
{
    [SerializeField] Toggle toggle;

    public void SetOnWithoutNotify()
    {
        toggle.SetIsOnWithoutNotify(true);
    }
    public void SetOffWithoutNotify()
    {
        toggle.SetIsOnWithoutNotify(false);
    }
}
