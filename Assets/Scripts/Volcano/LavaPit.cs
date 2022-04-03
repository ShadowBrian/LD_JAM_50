using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class LavaPit : MonoBehaviour
{
    [SerializeField]
    private VolcanoController volcanoController;

    [SerializeField]
    private float sinkSpeed;

    private List<Transform> _sinkingObjects;
    private float _currentY;

    //Unity Functions
    //====================================================================================================================//
    
    // Start is called before the first frame update
    private void Start()
    {
        _sinkingObjects = new List<Transform>();
        _currentY = transform.position.y;
    }

    // Update is called once per frame
    private void Update()
    {
        if (_sinkingObjects.Count == 0)
            return;

        for (int i = _sinkingObjects.Count - 1; i >= 0; i--)
        {
            var sinkingObject = _sinkingObjects[i];
            
            if(sinkingObject == null)
                continue;
            
            sinkingObject.position += Vector3.down * sinkSpeed * Time.deltaTime;

            if (sinkingObject.position.y < _currentY - 10)
                RemoveSinkingTransform(sinkingObject);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (_sinkingObjects.Contains(other.transform))
            return;

        other.attachedRigidbody.isKinematic = true;
        _sinkingObjects.Add(other.transform);

        var foodValue = other.gameObject.GetComponentsInChildren<Interactable>().Sum(x => x.foodValue);

        //TODO Need a way of determining the value
        volcanoController.ProvideFood(foodValue);
        AudioController.PlaySound(AudioController.SOUND.DISSOLVE, 0.4f);
    }

    //====================================================================================================================//

    private void RemoveSinkingTransform(in Transform target)
    {
        if (_sinkingObjects.Contains(target) == false)
            throw new Exception();
        
        _sinkingObjects.Remove(target);
        Destroy(target.gameObject);
    }

    //====================================================================================================================//
    
    
}
