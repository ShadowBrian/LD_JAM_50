using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class FaceTransformOnCircle : MonoBehaviour
{
    [SerializeField]
    private Transform targetTransform;

    [SerializeField]
    private float circleRadius;
    [SerializeField]
    private Transform circleCenterTransformRef;

    //Unity Function
    //====================================================================================================================//
    
    // Start is called before the first frame update
    private void Start()
    {
        Assert.IsNotNull(targetTransform);
        Assert.IsNotNull(circleCenterTransformRef);
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        transform.position = circleCenterTransformRef.position +
                             Vector3.ProjectOnPlane((targetTransform.position - circleCenterTransformRef.position).normalized,
                                 Vector3.up).normalized * 
                             circleRadius;
    }

    //====================================================================================================================//
}
