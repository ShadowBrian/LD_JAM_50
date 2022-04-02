using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerCarryInteract : MonoBehaviour
{
    [SerializeField]
    private Transform cameraTransform;

    [SerializeField]
    private Vector3 carryPositionOffset;

    [SerializeField]
    private LayerMask raycastMask;

    [SerializeField]
    private float castLength = 5f;

    [SerializeField]
    private KeyCode interactButton = KeyCode.Space;

    private bool CarryingObject => _carryingInteractable != null;
    private Interactable _carryingInteractable;
    
    private bool LookingAtInteractable => CarryingObject == false && _lookingAtInteractable != null;
    private Interactable _lookingAtInteractable;
    private Dictionary<Transform, Interactable> _lookAtHistoryDict;

    //Unity Functions
    //====================================================================================================================//
    
    // Start is called before the first frame update
    private void Start()
    {
        Assert.IsNotNull(cameraTransform);
        _lookAtHistoryDict = new Dictionary<Transform, Interactable>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (CarryingObject)
        {
            if (Input.GetKeyDown(interactButton))
                DropObject();
            
            return;
        }

        if (LookingAtInteractable && Input.GetKeyDown(interactButton))
        {
            PickupObject();
            return;
        }

        _lookingAtInteractable = LookForInteractable();
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

    private void PickupObject()
    {
        _carryingInteractable = _lookingAtInteractable;
        _lookingAtInteractable = null;

        _carryingInteractable.StartInteraction();
        
        var carryTransform = _carryingInteractable.transform;
        carryTransform.SetParent(transform, false);
        carryTransform.localPosition = carryPositionOffset;
    }

    private void DropObject()
    {
        _carryingInteractable.StopInteraction();
        
        _carryingInteractable = null;
    }

    //Unity Editor
    //====================================================================================================================//

    #region Unity Editor

#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireSphere(carryPositionOffset, 0.5f);
    }

#endif

    #endregion //Unity Editor
    //====================================================================================================================//
    
}
