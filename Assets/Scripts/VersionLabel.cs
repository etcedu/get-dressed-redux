using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VersionLabel : MonoBehaviour
{
    private void Start()
    {
        GetComponent<TMP_Text>().text = string.Format("v{0}", Application.version);
    }
}
