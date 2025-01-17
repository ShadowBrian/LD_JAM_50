﻿using System;
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

    private Vector2 _mouseDelta;



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

    private void Update()
    {
        //_mouseDelta.x = Input.GetAxis("Mouse X");
        // _mouseDelta.y = Input.GetAxis("Mouse Y");

        UpdateCameraPitch();
    }

    private void LateUpdate()
    {
        UpdateForwardFacingDirection();
    }

    private void OnDisable()
    {
        VolcanoController.OnGameOver -= OnGameOver;
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

        if (angle < verticalLookBounds)
            cameraTransform.localRotation = temp;

    }



    //====================================================================================================================//

    private void OnGameOver()
    {
        enabled = false;
    }

}
