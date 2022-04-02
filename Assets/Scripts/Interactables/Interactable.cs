using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    //Properties
    //====================================================================================================================//

    public bool Interacting { get; protected set; }

    public int foodValue = 1;

    protected Collider Collider{
        get
        {
            if (_collider == null)
                _collider = GetComponent<Collider>();

            return _collider;
        }
    }
    private Collider _collider;

    protected Rigidbody Rigidbody
    {
        get
        {
            if (_rigidbody == null)
                _rigidbody = GetComponent<Rigidbody>();

            return _rigidbody;
        }
    }
    private Rigidbody _rigidbody;

    private Transform _originalParent;

    //Unity Functions
    //====================================================================================================================//
    
    // Start is called before the first frame update
    protected virtual void Start()
    {
        _originalParent = transform.parent;
    }

    //Interactable Functions
    //====================================================================================================================//

    public virtual void StartInteraction()
    {
        Interacting = true;
        SetColliderActive(false);
    }
    
    public virtual void StopInteraction()
    {
        Interacting = false;
        ResetParent();
        SetColliderActive(true);
    }
    
    protected void SetColliderActive(in bool state)
    {
        Collider.enabled = state;
        Rigidbody.isKinematic = !state;
    }

    protected void ResetParent()
    {
        transform.SetParent(_originalParent, true);
    }

    //====================================================================================================================//
    
}
