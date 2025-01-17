﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerPushInteract : MonoBehaviour
{
    [SerializeField]
    private Vector2 speedMultRange;
    [SerializeField]
    private Vector2 ballSizeRange;

    [SerializeField]
    private float pushObjectDistanceMult = 6f;

    [SerializeField, Min(1)]
    private float maxDistance = 10f;

    [SerializeField]
    private PlayerController playerController;
    [SerializeField]
    private Transform cameraTransform;

    [SerializeField]
    private LayerMask raycastMask;

    [SerializeField]
    private float castLength = 5f;

    [SerializeField]
    private string interactButton = "Trigger";

    [SerializeField]
    private Arms arms;

    [SerializeField]
    private PushSphereInteractable pushSphereInteractablePrefab;

    private bool PushingObject => _pushingInteractable != null;
    private Interactable _pushingInteractable;
    private float _interactableDistance;

    private bool LookingAtInteractable => PushingObject == false && _lookingAtInteractable != null;
    private Interactable _lookingAtInteractable;
    private Dictionary<Transform, Interactable> _lookAtHistoryDict;

    private Vector2Int _currentInput;

    //Unity Functions
    //====================================================================================================================//

    private void OnEnable()
    {
        //CameraLook.OnForwardChanged += ForwardChanged;
        PlayerController.OnInputChanged += OnInputChanged;
    }

    // Start is called before the first frame update
    private void Start()
    {
        Assert.IsNotNull(pushSphereInteractablePrefab);
        Assert.IsNotNull(cameraTransform);
        _lookAtHistoryDict = new Dictionary<Transform, Interactable>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (PushingObject)
        {
            if (UnityXRInputBridge.instance.GetButton(XRButtonMasks.triggerButton, XRHandSide.LeftHand) == false && UnityXRInputBridge.instance.GetButton(XRButtonMasks.triggerButton, XRHandSide.RightHand) == false)
            {
                StopPushingObject();
                arms.SetPush(false, UnityXRInputBridge.instance.GetButton(XRButtonMasks.triggerButton, XRHandSide.LeftHand), UnityXRInputBridge.instance.GetButton(XRButtonMasks.triggerButton, XRHandSide.RightHand));
                return;
            }

            if (_pushingInteractable is PushSphereInteractable pushingInteractable)
                TryPushObject(pushingInteractable);

            ForwardChanged(default);
        }

        UIManager.Instance.ShowPromptWindow(LookingAtInteractable,
            LookingAtInteractable
                ? $"{_lookingAtInteractable.ActionVerb} <b>{interactButton}</b> {_lookingAtInteractable.Action}"
                : string.Empty);

        if (LookingAtInteractable && (UnityXRInputBridge.instance.GetButtonDown(XRButtonMasks.triggerButton, XRHandSide.LeftHand) || UnityXRInputBridge.instance.GetButtonDown(XRButtonMasks.triggerButton, XRHandSide.RightHand)))
        {
            StartPushingObject();
            arms.SetPush(false, UnityXRInputBridge.instance.GetButton(XRButtonMasks.triggerButton, XRHandSide.LeftHand), UnityXRInputBridge.instance.GetButton(XRButtonMasks.triggerButton, XRHandSide.RightHand));
            return;
        }
        if (_lookingAtInteractable == false && (UnityXRInputBridge.instance.GetButtonDown(XRButtonMasks.triggerButton, XRHandSide.LeftHand) || UnityXRInputBridge.instance.GetButtonDown(XRButtonMasks.triggerButton, XRHandSide.RightHand)))
        {
            arms.SetPush(false, UnityXRInputBridge.instance.GetButton(XRButtonMasks.triggerButton, XRHandSide.LeftHand), UnityXRInputBridge.instance.GetButton(XRButtonMasks.triggerButton, XRHandSide.RightHand));
        }
        if (_lookingAtInteractable == false && (UnityXRInputBridge.instance.GetButtonUp(XRButtonMasks.triggerButton, XRHandSide.LeftHand) || UnityXRInputBridge.instance.GetButtonUp(XRButtonMasks.triggerButton, XRHandSide.RightHand)))
        {
            arms.SetPush(false, UnityXRInputBridge.instance.GetButton(XRButtonMasks.triggerButton, XRHandSide.LeftHand), UnityXRInputBridge.instance.GetButton(XRButtonMasks.triggerButton, XRHandSide.RightHand));
        }

        _lookingAtInteractable = LookForInteractable();
    }

    private void OnDisable()
    {
        CameraLook.OnForwardChanged -= ForwardChanged;
        PlayerController.OnInputChanged -= OnInputChanged;
    }

    //====================================================================================================================//

    private Interactable LookForInteractable()
    {
        var origin = cameraTransform.position;
        var direction = cameraTransform.forward.normalized;

        var didHitInteractable = Physics.Raycast(
            origin,
            direction,
            out var hit,
            castLength,
            raycastMask.value);

        Debug.DrawRay(origin, direction * castLength, didHitInteractable ? Color.green : Color.yellow);

        if (didHitInteractable == false)
            return null;

        if (_lookAtHistoryDict.TryGetValue(hit.transform, out var lookingAt))
            return lookingAt;

        var newInteractable = hit.transform.gameObject.GetComponent<Interactable>();
        _lookAtHistoryDict.Add(hit.transform, newInteractable);

        return newInteractable;
    }

    private void StartPushingObject()
    {
        switch (_lookingAtInteractable)
        {
            //--------------------------------------------------------------------------------------------------------//
            case PushSphereInteractable pushSphereInteractable:
                {
                    _pushingInteractable = pushSphereInteractable;
                    _lookingAtInteractable = null;
                    _pushingInteractable.StartInteraction();
                    arms.SetPush(true, false);
                    break;
                }
            //--------------------------------------------------------------------------------------------------------//
            case BreakableInteractable breakableInteractable:
                {
                    breakableInteractable.TryStartInteraction();
                    arms.SetPushT(1f);
                    return;
                }
            //--------------------------------------------------------------------------------------------------------//
            case Interactable interactable:
                {
                    interactable.StartInteraction();
                    var lookingAtTransform = interactable.transform;
                    var pushInteractable = Instantiate(pushSphereInteractablePrefab, lookingAtTransform.position,
                        Quaternion.identity);

                    pushInteractable.StartInteraction(interactable);
                    _pushingInteractable = pushInteractable;
                    _lookingAtInteractable = null;
                    arms.SetPush(true, false);
                    break;
                }
            //--------------------------------------------------------------------------------------------------------//
            default:
                throw new Exception();
        }

        _interactableDistance = Vector3.Distance(cameraTransform.transform.position, _pushingInteractable.transform.position);

        UIManager.Instance.ShowPromptWindow(false, string.Empty);
    }

    private void StopPushingObject()
    {
        _pushingInteractable.StopInteraction();
        _pushingInteractable = null;

        arms.SetPush(false, false);
    }

    private void TryPushObject(in PushSphereInteractable pushingInteractable)
    {
        var playerForward = cameraTransform.transform.forward.normalized;

        var direction = Vector3.zero;
        direction += playerForward * _currentInput.y;
        direction += cameraTransform.transform.right.normalized * _currentInput.x;

        pushingInteractable.PlayerForward = playerForward;
        pushingInteractable.Push(direction, playerController.CurrentMoveSpeed);

        playerController.SpeedMultiplier = Mathf.Lerp(speedMultRange.x, speedMultRange.y,
            Mathf.InverseLerp(ballSizeRange.y, ballSizeRange.x, pushingInteractable.Size));
    }

    //Callback Functions
    //====================================================================================================================//

    private void OnInputChanged(int x, int y)
    {
        _currentInput.x = x;
        _currentInput.y = y;
    }

    private void ForwardChanged(float deg)
    {
        if (!PushingObject || !(_pushingInteractable is PushSphereInteractable pushingInteractable))
            return;

        var distance = Mathf.Min(pushingInteractable.Size * pushObjectDistanceMult, maxDistance);


        var expectedPosition = cameraTransform.transform.position + (cameraTransform.transform.forward.normalized * distance);

        var dist = Vector3.ProjectOnPlane(expectedPosition - pushingInteractable.transform.position, Vector3.up);
        var mag = dist.magnitude;

        pushingInteractable.Push(dist.normalized, mag * mag * mag);

        playerController.SpeedMultiplier = Mathf.Lerp(speedMultRange.x, speedMultRange.y,
            Mathf.InverseLerp(ballSizeRange.y, ballSizeRange.x, pushingInteractable.Size));
    }

    //====================================================================================================================//
}
