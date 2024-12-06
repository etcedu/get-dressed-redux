using UnityEngine;
using System.Collections;

public class SetActive : MonoBehaviour {

    public void True()
    {
        //Debug.Log(gameObject.name + " active");
        gameObject.SetActive(true);
    }

    public void False()
    {
        //Debug.Log(gameObject.name + " inactive");
        gameObject.SetActive(false);
    }

    public void Toggle()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
