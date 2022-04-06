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

    private CapsuleCollider _capsulecol;

    [SerializeField]
    private Transform HeadTransform;

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
        _capsulecol = GetComponent<CapsuleCollider>();
    }

    private void FixedUpdate()
    {
        Vector3 zeroHeadForward = HeadTransform.forward;
        zeroHeadForward.y = 0f;

        var moveToApply = Vector3.zero;
        moveToApply += Quaternion.Inverse(transform.rotation) * Quaternion.LookRotation(zeroHeadForward, Vector3.up) * transform.forward * _inputState.y * moveSpeed * SpeedMultiplier;
        moveToApply += Quaternion.Inverse(transform.rotation) * Quaternion.LookRotation(zeroHeadForward, Vector3.up) * transform.right * _inputState.x * moveSpeed * SpeedMultiplier;

        _capsulecol.center = new Vector3(HeadTransform.localPosition.x, 0, HeadTransform.localPosition.z) * 1.4065f;

        rigidbody.position += moveToApply * Time.fixedDeltaTime;
    }

    bool JustRotated;

    // Update is called once per frame
    private void Update()
    {
        if (UnityXRInputBridge.instance.GetVec2(XR2DAxisMasks.primary2DAxis, XRHandSide.LeftHand).x < -0.1f)
            _inputState.x = -1;
        else if (UnityXRInputBridge.instance.GetVec2(XR2DAxisMasks.primary2DAxis, XRHandSide.LeftHand).x > 0.1f)
            _inputState.x = 1;
        else
            _inputState.x = 0;

        if (UnityXRInputBridge.instance.GetVec2(XR2DAxisMasks.primary2DAxis, XRHandSide.LeftHand).y > 0.1f)
            _inputState.y = 1;
        else if (UnityXRInputBridge.instance.GetVec2(XR2DAxisMasks.primary2DAxis, XRHandSide.LeftHand).y < -0.1f)
            _inputState.y = -1;
        else
            _inputState.y = 0;


        if ((UnityXRInputBridge.instance.GetButtonDown(XRButtonMasks.primaryButton, XRHandSide.LeftHand) || UnityXRInputBridge.instance.GetVec2(XR2DAxisMasks.primary2DAxis, XRHandSide.RightHand).x < -0.5f) && !JustRotated)
        {
            transform.RotateAround(HeadTransform.position, Vector3.up, -45f);
            JustRotated = true;
        }

        if ((UnityXRInputBridge.instance.GetButtonDown(XRButtonMasks.primaryButton, XRHandSide.RightHand) || UnityXRInputBridge.instance.GetVec2(XR2DAxisMasks.primary2DAxis, XRHandSide.RightHand).x > 0.5f) && !JustRotated)
        {
            transform.RotateAround(HeadTransform.position, Vector3.up, 45f);
            JustRotated = true;
        }

        if (UnityXRInputBridge.instance.GetVec2(XR2DAxisMasks.primary2DAxis, XRHandSide.RightHand).x < 0.5f && UnityXRInputBridge.instance.GetVec2(XR2DAxisMasks.primary2DAxis, XRHandSide.RightHand).x > -0.5f)
        {
            JustRotated = false;
        }

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
