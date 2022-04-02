using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class FaceTransform : MonoBehaviour
{
    [SerializeField]
    private Transform targetTransform;
    private Transform transform;

    //Unity Function
    //====================================================================================================================//
    
    // Start is called before the first frame update
    private void Start()
    {
        Assert.IsNotNull(targetTransform);
        transform = gameObject.transform;
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        transform.forward = (targetTransform.position - transform.position).normalized;
    }

    //====================================================================================================================//
    
}
