using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    //Properties
    //====================================================================================================================//

    [SerializeField]
    private float horizontalSpeed;
    [SerializeField]
    private float forwardSpeed;
    
    [SerializeField]
    private Transform cameraTransform;

    //====================================================================================================================//

    private Vector2Int _inputState;
    
    private Rigidbody rigidbody
    {
        get
        {
            if (_rigidbody == null)
                _rigidbody = GetComponent<Rigidbody>();

            return _rigidbody;
        }
    }
    private Rigidbody _rigidbody;
    
    private Transform transform;

    //Unity Functions
    //====================================================================================================================//
    
    // Start is called before the first frame update
    private void Start()
    {
        transform = gameObject.transform;
    }

    private void FixedUpdate()
    {
        var moveToApply = Vector3.zero;
        moveToApply += transform.forward * _inputState.y * forwardSpeed;
        moveToApply += transform.right * _inputState.x * horizontalSpeed;

        rigidbody.position += moveToApply * Time.fixedDeltaTime;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKey(KeyCode.A))
            _inputState.x = -1;
        else if (Input.GetKey(KeyCode.D))
            _inputState.x = 1;
        else
            _inputState.x = 0;
        
        if (Input.GetKey(KeyCode.W))
            _inputState.y = 1;
        else if (Input.GetKey(KeyCode.S))
            _inputState.y = -1;
        else
            _inputState.y = 0;
    }

    //====================================================================================================================//
    
}
