using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arms : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer leftArmRenderer;
    private Transform _leftArmTransform;
    private Vector3 _leftArmStartPosition;
    
    [SerializeField]
    private SpriteRenderer rightArmRenderer;
    private Transform _rightArmTransform;
    private Vector3 _rightArmStartPosition;

    [SerializeField]
    private float distanceRange;
    private float _currentDistance;
    private float _t;
    private bool _isPushing;

    [SerializeField, Min(0f)]
    private float pushSpeed;
    
    
    //Unity Functions
    //====================================================================================================================//
    
    // Start is called before the first frame update
    private void Start()
    {
        _leftArmTransform = leftArmRenderer.transform;
        _leftArmStartPosition = _leftArmTransform.localPosition;
        
        _rightArmTransform = rightArmRenderer.transform;
        _rightArmStartPosition = _rightArmTransform.localPosition;

        _t = 0f;
        SetDistance(_currentDistance);
    }

    // Update is called once per frame
    private void Update()
    {
        _t = _isPushing
            ? Mathf.Clamp01(_t + Time.deltaTime * pushSpeed)
            : Mathf.Clamp01(_t - Time.deltaTime * pushSpeed);

        if(_isPushing)
            SetDistance(Mathf.Lerp(_currentDistance, distanceRange, _t));
        else
            SetDistance(Mathf.Lerp(0f, _currentDistance, _t));
    }

    //====================================================================================================================//

    public void SetPush(bool isPushing, bool instant)
    {
        _isPushing = isPushing;
        
        if (instant)
            _t = isPushing ? 1f : 0f;
    }
    public void SetPushT(float t)
    {
        _t = Mathf.Clamp01(t);
        SetDistance(Mathf.Lerp(0f, distanceRange, _t));
    }

    private void SetDistance(float distance)
    {
        _rightArmTransform.localPosition = _rightArmStartPosition + Vector3.forward * distance;
        _leftArmTransform.localPosition = _leftArmStartPosition + Vector3.forward * distance;

        _currentDistance = distance;
    }
}
