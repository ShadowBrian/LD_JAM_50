using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DislodgeInteractable : Interactable
{
    //public override string Action => "to Dislodge";
    
    public bool IsDislodged => false;
    
    /*[SerializeField]
    private Vector2Int dislodgeCountRange;
    private int _dislodgeCount;
    [SerializeField]
    private float dislodgeRotation;*/

    //Unity Functions
    //====================================================================================================================//
    
    protected override void Start()
    {
        Rigidbody.isKinematic = true;
        //_dislodgeCount = Random.Range(dislodgeCountRange.x, dislodgeCountRange.y + 1);
        
        
        base.Start();
    }

    //Interactable Functions
    //====================================================================================================================//


    
    public override void StartInteraction()
    {
        Rigidbody.isKinematic = false;
        base.StartInteraction();
    }

    public override void StopInteraction()
    {
        base.StopInteraction();
    }

    //====================================================================================================================//
    
}
