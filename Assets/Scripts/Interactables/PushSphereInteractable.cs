using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class PushSphereInteractable : Interactable
{
    public const float RESIZE_MULT = 0.05f;
    public float Size { get; private set; }
    public int AttachedObjects { get; private set; }

    private static PlayerController _playerController;

    private SphereCollider SphereCollider
    {
        get
        {
            if(_sphereCollider)
                _sphereCollider = Collider as SphereCollider;

            return _sphereCollider;
        }
    }
    private SphereCollider _sphereCollider;

    public Vector3 PlayerForward { get; set; }


    //Unity Functions
    //====================================================================================================================//
    
    protected override void Start()
    {
        if (!_playerController)
            _playerController = FindObjectOfType<PlayerController>();
        
        base.Start();

        Rigidbody.isKinematic = Interacting == false;
        _sphereCollider = Collider as SphereCollider;

        Size = SphereCollider.radius;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (Interacting == false)
            return;
        
        var otherInteractable = collision.gameObject.GetComponent<Interactable>();
        if (otherInteractable == null)
            return;

        TryCollectInteractable(otherInteractable);
    }

    //====================================================================================================================//
    
    public override void StartInteraction()
    {
        Interacting = true;
        Rigidbody.isKinematic = false;
    }
    
    public void StartInteraction(Interactable forceAdd)
    {
        Interacting = true;
        Rigidbody.isKinematic = false;
        
        TryCollectInteractable(forceAdd, false);
    }

    public override void StopInteraction()
    {
        Interacting = false;
        Rigidbody.isKinematic = false;
    }

    //====================================================================================================================//
    
    
    public void Push(in Vector3 direction, in float speed)
    {
        const float LENGTH = 4f;
        if (Interacting == false)
            return;

        var circ = Mathf.PI * (_sphereCollider.radius * 2f);
        var circPerSec = circ / (speed * Time.deltaTime);
        var degPerSec = 360f / circPerSec;
        
        Rigidbody.position += direction * speed * Time.deltaTime;
        
        var crossed = -Vector3.Cross(direction.normalized, Vector3.up);
        transform.Rotate(crossed, degPerSec, Space.World);

#if UNITY_EDITOR
        var currentPosition = transform.position;
        Debug.DrawRay(currentPosition, LENGTH * direction.normalized, Color.blue);
        Debug.DrawRay(currentPosition, LENGTH * Vector3.up, Color.green);
        Debug.DrawRay(currentPosition, LENGTH * crossed, Color.red);
#endif

    }

    private void TryCollectInteractable(in Interactable interactable, bool reposition = true)
    {
        
        switch (interactable)
        {
            case BreakableInteractable _:
                return;
            /*case DislodgeInteractable dislodgeInteractable:
                if (dislodgeInteractable.IsDislodged == false)
                    return;
                interactable.StartInteraction();
                break;*/
            case PushSphereInteractable pushSphere:
                //interactable.StartInteraction();
                pushSphere.Rigidbody.isKinematic = true;
                pushSphere.Collider.enabled = false;
                pushSphere.enabled = false;
                break;
            case Interactable _:
                interactable.StartInteraction();
                break;
        }
        
        var interactableTransform = interactable.transform;
        interactableTransform.SetParent(transform, reposition);

        if (reposition == false)
        {
            interactableTransform.localPosition = Vector3.zero;
            return;
        }

        interactableTransform.position = transform.position +
                                         ((interactableTransform.position - transform.position).normalized *
                                          Size * 1.2f);

        SphereCollider.radius = Size *= 1f + RESIZE_MULT;

        AttachedObjects++;
        /*//Push the ball away to remain in view
        var distanceToPlayer = Vector3.Distance(_playerController.transform.position, transform.position);
        Push(PlayerForward, distanceToPlayer * RESIZE_MULT);*/
        AudioController.PlaySound(AudioController.SOUND.POP, 0.7f);
    }

    //====================================================================================================================//

}
