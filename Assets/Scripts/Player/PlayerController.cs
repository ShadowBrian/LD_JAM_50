using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public static Action<int, int> OnInputChanged;
    //Properties
    //====================================================================================================================//
    public float CurrentMoveSpeed => moveSpeed * _inputState.y * SpeedMultiplier;

    public float SpeedMultiplier = 1f;
    
    [SerializeField]
    private float moveSpeed;
    
    //====================================================================================================================//

    private Vector2Int _inputState;
    private Vector2Int _lastInputState;
    
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

    private void OnEnable()
    {
        VolcanoController.OnGameOver += OnGameOver;
    }

    // Start is called before the first frame update
    private void Start()
    {
        transform = gameObject.transform;
    }

    private void FixedUpdate()
    {
        var moveToApply = Vector3.zero;
        moveToApply += transform.forward * _inputState.y * moveSpeed * SpeedMultiplier;
        moveToApply += transform.right * _inputState.x * moveSpeed * SpeedMultiplier;

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

        if (_lastInputState == _inputState)
            return;

        _lastInputState = _inputState;
        OnInputChanged?.Invoke(_inputState.x, _inputState.y);
    }
    
    private void OnDisable()
    {
        VolcanoController.OnGameOver -= OnGameOver;
    }

    //====================================================================================================================//

    private void OnGameOver()
    {
        enabled = false;
    }
}
