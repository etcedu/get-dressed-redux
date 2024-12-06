using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class AnimatorParameterSet : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    [SerializeField]
	private string _trigger = "", _bool = "", _float = "", _int = "";
    [SerializeField]
    private bool _bool_value;
    [SerializeField]
    private float _float_value;
    [SerializeField]
    private int _int_value;

    void OnEnable()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    public void SetTrigger()
    {
        if (animator == null) return;

        animator.SetTrigger(this._trigger);
    }

    public void SetBool()
    {
        if (animator == null) return;

        animator.SetBool(this._bool, this._bool_value);
    }

    public void SetFloat()
    {
        if (animator == null) return;

        animator.SetFloat(this._float, this._float_value);
    }

    public void SetInt()
    {
        if (animator == null) return;

        animator.SetInteger(this._int, this._int_value);
    }

}
