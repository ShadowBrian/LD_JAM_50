using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DislodgeInteractable : Interactable
{
    public bool IsDislodged => _dislodgeCount <= 0;
    
    [SerializeField]
    private Vector2Int dislodgeCountRange;
    private int _dislodgeCount;
    [SerializeField]
    private float dislodgeRotation;

    //Unity Functions
    //====================================================================================================================//
    
    protected override void Start()
    {
        Rigidbody.isKinematic = true;
        _dislodgeCount = Random.Range(dislodgeCountRange.x, dislodgeCountRange.y + 1);
        
        
        base.Start();
    }

    //Interactable Functions
    //====================================================================================================================//

    public bool TryStartInteraction()
    {
        if (_dislodgeCount > 0)
        {
            if (--_dislodgeCount > 0)
            {
                //TODO Apply Random Rotation
                transform.rotation *= Quaternion.Euler(
                    Random.Range(-1f, 1f) * dislodgeRotation,
                    Random.Range(-1f, 1f) * dislodgeRotation, 
                    Random.Range(-1f, 1f) * dislodgeRotation);
                return false;
            }
            
            Rigidbody.isKinematic = false;
            return false;
        }

        StartInteraction();
        return true;
    }
    
    public override void StartInteraction()
    {
        base.StartInteraction();
    }

    public override void StopInteraction()
    {
        base.StopInteraction();
    }

    //====================================================================================================================//
    
}
