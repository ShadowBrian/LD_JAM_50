using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    //Properties
    //====================================================================================================================//
    
    private Collider Collider{
        get
        {
            if (_collider == null)
                _collider = GetComponent<Collider>();

            return _collider;
        }
    }
    private Collider _collider;

    private Rigidbody Rigidbody
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
    private void Start()
    {
        _originalParent = transform.parent;
    }

    //Interactable Functions
    //====================================================================================================================//
    
    public void SetColliderActive(in bool state)
    {
        Collider.enabled = state;
        Rigidbody.isKinematic = !state;
    }

    public void ResetParent()
    {
        transform.SetParent(_originalParent, true);
    }

    //====================================================================================================================//
    
}
