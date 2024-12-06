using UnityEngine;
using System.Collections;

public delegate void FC_Collision(Collision other, GameObject target);
public delegate void FC_Trigger(Collider other, GameObject target);

public class ForwardCollision : MonoBehaviour
{
    public GameObject target
    {
        set { _target = value; }
    }
    private GameObject _target;
    public FC_Collision CollisionEnter;
    public FC_Collision CollisionStay;
    public FC_Collision CollisionExit;

    public FC_Trigger TriggerEnter;
    public FC_Trigger TriggerStay;
    public FC_Trigger TriggerExit;

    void Awake()
    {
        if (GetComponent<Collider>() == null)
        {
            Debug.LogWarning("No collider attached to " + gameObject.name + ". Cannot forward touches.");
            enabled = false;
        }
        _target = gameObject;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (CollisionEnter != null) CollisionEnter(collision, _target);
    }

    void OnCollisionStay(Collision collision)
    {
        if (CollisionStay != null) CollisionStay(collision, _target);
    }

    void OnCollisionExit(Collision collision)
    {
        if (CollisionExit != null) CollisionExit(collision, _target);
    }

    void OnTriggerEnter(Collider other)
    {
        if (TriggerEnter != null) TriggerEnter(other, _target);
    }

    void OnTriggerStay(Collider other)
    {
        if (TriggerStay != null) TriggerStay(other, _target);
    }

    void OnTriggerExit(Collider other)
    {
        if (TriggerExit != null) TriggerExit(other, _target);
    }
}
