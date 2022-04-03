using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BreakableInteractable : Interactable
{
    public override string ActionVerb => "Press";
    public override string Action => "to Break";
    
    [SerializeField]
    private Interactable[] connectedInteractables;
    
    [SerializeField]
    private Vector2Int breakCountRange;
    private int _breakCount;
    
    [SerializeField]
    private float dislodgeshake;

    //Unity Functions
    //====================================================================================================================//
    private void Awake()
    {
        foreach (var interactable in connectedInteractables)
        {
            interactable.transform.SetParent(transform, true);
            interactable.Rigidbody.isKinematic = true;
            interactable.Collider.enabled = false;
        }
    }

    protected override void Start()
    {
        Rigidbody.isKinematic = true;
        _breakCount = Random.Range(breakCountRange.x, breakCountRange.y + 1);
        
        
        base.Start();
    }

    //Interactable Functions
    //====================================================================================================================//

    public void TryStartInteraction()
    {
        if (_breakCount <= 0) 
            return;
        
        if (--_breakCount > 0)
        {
            //TODO Apply Shake
            StartCoroutine(ShakeCoroutine(0.25f, 0.3f));
            return;
        }
            
        StartInteraction();
    }
    
    public override void StartInteraction()
    {
        //TODO Enable all children
        //TODO Separate Children
        Collider.enabled = false;

        foreach (var interactable in connectedInteractables)
        {
            interactable.transform.SetParent(transform.parent, true);
            interactable.Rigidbody.isKinematic = false;
            interactable.Collider.enabled = true;

            interactable.Rigidbody.velocity = Random.insideUnitSphere.normalized * 10f;
        }
        //TODO Destroy Self
        Destroy(gameObject);
    }

    public override void StopInteraction()
    {

    }

    private IEnumerator ShakeCoroutine(float amount, float time)
    {
        var originalPosition = transform.position;
        
        for (float t = 0; t < time; t += Time.deltaTime)
        {
            var random = Random.insideUnitCircle * Random.Range(0f, amount);
            
            transform.position = originalPosition + new Vector3(random.x, 0f, random.y);
            yield return null;
        }

        transform.position = originalPosition;
    }

    //====================================================================================================================//
}
