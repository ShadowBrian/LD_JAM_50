using System;
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
    private PlayerController playerController;
    [SerializeField]
    private Transform cameraTransform;

    [SerializeField]
    private LayerMask raycastMask;

    [SerializeField]
    private float castLength = 5f;

    [SerializeField]
    private KeyCode interactButton = KeyCode.Space;

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
        CameraLook.OnForwardChanged += ForwardChanged;
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
            if (Input.GetKey(interactButton) == false)
            {
                StopPushingObject();
                return;
            }

            if (_pushingInteractable is PushSphereInteractable pushingInteractable)
                TryPushObject(pushingInteractable);
            
        }

        UIManager.Instance.ShowPromptWindow(LookingAtInteractable,
            LookingAtInteractable
                ? $"{_lookingAtInteractable.ActionVerb} {interactButton} {_lookingAtInteractable.Action}"
                : string.Empty);
        
        if (LookingAtInteractable && Input.GetKeyDown(interactButton))
        {
            StartPushingObject();
            return;
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
                break;
            }
            //--------------------------------------------------------------------------------------------------------//
            case BreakableInteractable breakableInteractable:
            {
                breakableInteractable.TryStartInteraction();
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
                break;
            }
            //--------------------------------------------------------------------------------------------------------//
            default:
                throw new Exception();
        }

        _interactableDistance = Vector3.Distance(transform.position, _pushingInteractable.transform.position);
        
        UIManager.Instance.ShowPromptWindow(false, string.Empty);
    }

    private void StopPushingObject()
    {
        _pushingInteractable.StopInteraction();
        _pushingInteractable = null;
    }

    private void TryPushObject(in PushSphereInteractable pushingInteractable)
    {
        var direction = Vector3.zero;
        direction += playerController.transform.forward.normalized * _currentInput.y;
        direction += playerController.transform.right.normalized * _currentInput.x;
        
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
        if (PushingObject && _pushingInteractable is PushSphereInteractable pushingInteractable)
        {
            var expectedPosition = transform.position + (transform.forward.normalized * _interactableDistance);
            
            var dist = (expectedPosition - pushingInteractable.transform.position);
            var mag = dist.magnitude;
            
            pushingInteractable.Push(dist.normalized, mag * mag * mag);

            playerController.SpeedMultiplier = Mathf.Lerp(speedMultRange.x, speedMultRange.y,
                Mathf.InverseLerp(ballSizeRange.y, ballSizeRange.x, pushingInteractable.Size));
        }
    }

    //====================================================================================================================//
}
