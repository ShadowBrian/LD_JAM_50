using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class VolcanoController : MonoBehaviour
{
    private static int AtmosphereThickness = Shader.PropertyToID("_AtmosphereThickness");

    //Structs
    //====================================================================================================================//

    [Serializable]
    private struct VolcanoStateData
    {
        public Sprite FaceSprite;
        public float hungerValue;
        public Color smokeColor;
        public float shakeAmplitude;
        public float shakeDuration;
    }

    //Properties
    //====================================================================================================================//

    public static Action OnGameOver;
    public static Action<Sprite> OnNewFace;
    
    
    [FormerlySerializedAs("faces")] [SerializeField]
    private VolcanoStateData[] states;
    [SerializeField]
    private SpriteRenderer faceRenderer;
    private int _stateIndex = -1;
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

    [SerializeField]
    private ParticleSystem smokeParticleSystem;
    private ParticleSystem.MainModule _mainSmokeModule;
    [SerializeField] private VolcanoFinalAnimation volcanoFinalAnimation;


    //Unity Functions
    //====================================================================================================================//
    
    // Start is called before the first frame update
    private void Start()
    {
        _mainSmokeModule = smokeParticleSystem.main;
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
            enabled = false;
            
            volcanoFinalAnimation.StartAnimation();
            OnGameOver?.Invoke();
        }
        else
        {
            TryUpdateVolcanoState(hungerValue);
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
        OnNewFace?.Invoke(happyFace);
        this.DelayedCall(2f, () =>
        {
            _overrideFace = false;
            SetNewVolcanoState(_stateIndex, false);
        });

        _currentHunger = 0;
        /*_currentHunger -= amount;
        if (_currentHunger < 0)
            _currentHunger = 0;*/
    }

    private void TryUpdateVolcanoState(in float hungerValue)
    {
        int newFaceIndex = -1;
        for (int i = 0; i < states.Length; i++)
        {
            if (hungerValue > states[i].hungerValue) 
                continue;
            
            newFaceIndex = i;
            break;
        }

        if (_stateIndex == newFaceIndex)
            return;

        SetNewVolcanoState(newFaceIndex, true);
    }

    private void SetNewVolcanoState(in int newIndex, bool announceFace)
    {
        var stateData = states[newIndex];
        
        if (newIndex > _stateIndex && newIndex != 0)
        {
            CameraShake.Shake(stateData.shakeAmplitude, stateData.shakeDuration);
        }
        
        _stateIndex = newIndex;

        _mainSmokeModule.startColor = stateData.smokeColor;

        if(announceFace)
            OnNewFace?.Invoke(stateData.FaceSprite);
        
        if(_overrideFace == false)
            SetNewFace(stateData.FaceSprite);
    }

    private void SetNewFace(in Sprite faceSprite)
    {
        faceRenderer.sprite = faceSprite;
        
    }

    //====================================================================================================================//
    
}
