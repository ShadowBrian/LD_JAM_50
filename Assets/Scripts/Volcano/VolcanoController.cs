using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolcanoController : MonoBehaviour
{
    private static int AtmosphereThickness = Shader.PropertyToID("_AtmosphereThickness");

    //Structs
    //====================================================================================================================//

    [Serializable]
    private struct FaceHungerIndex
    {
        public Sprite FaceSprite;
        public float hungerValue;
    }

    //Properties
    //====================================================================================================================//
    
    
    
    [SerializeField]
    private FaceHungerIndex[] faces;
    [SerializeField]
    private SpriteRenderer faceRenderer;
    private int _faceIndex = -1;
    private bool _overrideFace;

    [SerializeField]
    private Sprite happyFace;
    [SerializeField]
    private Sprite angryFace;

    [SerializeField]
    private Material skyboxMaterial;
    [SerializeField]
    private Vector2 atmosphereRange;

    [SerializeField]
    private float maxHunger;

    [SerializeField]
    private float hungerIncreasePerSecond;
    private float _currentHunger;
    

    //Unity Functions
    //====================================================================================================================//
    
    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        _currentHunger += hungerIncreasePerSecond * Time.deltaTime;

        var hungerValue = _currentHunger / maxHunger;
        
        skyboxMaterial.SetFloat(AtmosphereThickness, Mathf.Lerp(atmosphereRange.x, atmosphereRange.y, hungerValue));

        if (hungerValue >= 1f)
        {
            SetNewFace(angryFace);
        }
        else
        {
            TryUpdateHungerFace(hungerValue);
        }

        
    }

    private void OnDisable()
    {
        skyboxMaterial.SetFloat(AtmosphereThickness, atmosphereRange.x);
    }

    //====================================================================================================================//

    public void ProvideFood(in int amount)
    {
        _overrideFace = true;
        SetNewFace(happyFace);
        this.DelayedCall(2f, () =>
        {
            _overrideFace = false;
            SetNewFace(_faceIndex);
        });

        _currentHunger -= amount;
        if (_currentHunger < 0)
            _currentHunger = 0;
    }

    private void TryUpdateHungerFace(in float hungerValue)
    {
        int newFaceIndex = -1;
        for (int i = 0; i < faces.Length; i++)
        {
            if (hungerValue > faces[i].hungerValue) 
                continue;
            
            newFaceIndex = i;
            break;
        }

        if (_faceIndex == newFaceIndex)
            return;

        SetNewFace(newFaceIndex);
    }

    private void SetNewFace(in int index)
    {
        _faceIndex = index;

        if(_overrideFace == false)
            SetNewFace(faces[index].FaceSprite);
    }

    private void SetNewFace(in Sprite faceSprite)
    {
        faceRenderer.sprite = faceSprite;
    }

    //====================================================================================================================//
    
}
