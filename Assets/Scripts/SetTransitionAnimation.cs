using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetTransitionAnimationForToggle : MonoBehaviour
{
    Toggle toggle;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        toggle = GetComponent<Toggle>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (toggle.isOn)
        {
            animator.SetBool("Selected", true);
        }
        toggle.interactable = !toggle.isOn;
    }
}
