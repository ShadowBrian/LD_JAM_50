using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLook : MonoBehaviour
{
    public static Action<float> OnForwardChanged;
    //Properties
    //====================================================================================================================//
    
    [SerializeField]
    private float xSpeed;
    [SerializeField]
    private float ySpeed;

    [SerializeField]
    private float verticalLookBounds;
    
    [SerializeField]
    private Transform cameraTransform;
    private Transform transform;

    private Vector2 _lastMousePosition;
    private Vector2 _currentMousePosition;
    [SerializeField]
    private Vector2 _mouseDelta;

    private bool _cursorLocked;

    //Unity Functions
    //====================================================================================================================//
    
    // Start is called before the first frame update
    private void Start()
    {
        transform = gameObject.transform;

        _lastMousePosition = _currentMousePosition = Vector2.zero;
        LockCursor(true);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            LockCursor(!_cursorLocked);
        }

        _mouseDelta.x = Input.GetAxis("Mouse X");
        _mouseDelta.y = Input.GetAxis("Mouse Y");
        //_lastMousePosition = _currentMousePosition;
        //_currentMousePosition = Input.mousePosition;
//
        //_mouseDelta = (_currentMousePosition - _lastMousePosition) / Time.deltaTime;
        
        UpdateCameraPitch();
    }

    private void LateUpdate()
    {
        UpdateForwardFacingDirection();
    }

    //====================================================================================================================//
    
    private void UpdateForwardFacingDirection()
    {
        var yDeltaDegrees = _mouseDelta.x * xSpeed;

        var currentRotation = transform.rotation;
        var rotation = Quaternion.Euler(0, yDeltaDegrees, 0);

        transform.rotation = currentRotation * rotation;
        
        if (Mathf.Abs(yDeltaDegrees) < 1f)
            OnForwardChanged?.Invoke(yDeltaDegrees);
    }
    
    private void UpdateCameraPitch()
    {
        var xDeltaDegrees = _mouseDelta.y * ySpeed;
        
        var currentLocalRotation = cameraTransform.localRotation;
        var yQuaternion = Quaternion.AngleAxis(xDeltaDegrees, -Vector3.right);
        var temp = currentLocalRotation * yQuaternion;

        var angle = Quaternion.Angle(Quaternion.identity, temp);
        
        if(angle < verticalLookBounds) 
            cameraTransform.localRotation = temp;
        
    }

    private void LockCursor(in bool state)
    {
        _cursorLocked = state;
        
        Cursor.lockState = _cursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !_cursorLocked;
    }

    //====================================================================================================================//
    
}
