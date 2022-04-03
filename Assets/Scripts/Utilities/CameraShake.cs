using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraShake : MonoBehaviour
{
    private static CameraShake Instance { get; set; }

    [SerializeField]
    private CinemachineVirtualCamera cinemachineVirtualCamera;

    private CinemachineBasicMultiChannelPerlin _multiChannelPerlin;

    private float _amplitude;
    private float _shakeTimer;
    private float _shakeTime;

    [Header("Debugging"), SerializeField]
    private float testAmplitude = 1f;
    [SerializeField]
    private float testTime = 0.5f;

    //Unity Functions
    //====================================================================================================================//

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (_shakeTimer > 0)
        {
            _shakeTimer -= Time.deltaTime;

            //_multiChannelPerlin.m_AmplitudeGain = ;
            SetCameraAmplitude(Mathf.Lerp(0f, _amplitude, _shakeTimer / _shakeTime));
        }
        else
        {
            //_multiChannelPerlin.m_AmplitudeGain = 0;
            SetCameraAmplitude(0);
        }
        
    }

    //====================================================================================================================//

    public static void SetActiveCamera(CinemachineVirtualCamera virtualCamera)
    {
        Instance.cinemachineVirtualCamera = virtualCamera;
        Instance._multiChannelPerlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }
    public static void Shake(float amplitude, float time)
    {
        if (Instance == null)
            return;
        
        Instance.ShakeCamera(amplitude, time);
    }
    
    private void ShakeCamera(float amplitude, float time)
    {
        AudioController.PlaySound(AudioController.SOUND.RUMBLE, 1f);
        
        if(_multiChannelPerlin == null)
            _multiChannelPerlin = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        _amplitude = amplitude;
        _shakeTime = _shakeTimer = time;

        SetCameraAmplitude(_amplitude);
    }

    private void SetCameraAmplitude(in float amplitude)
    {
        if (_multiChannelPerlin == null)
            return;
        
        _multiChannelPerlin.m_AmplitudeGain = amplitude;
    }

    //====================================================================================================================//

#if UNITY_EDITOR
    [ContextMenu("TestShake")]
    private void TestShake()
    {
        if (EditorApplication.isPlaying == false)
            return;
        
        ShakeCamera(testAmplitude, testTime);
    }
#endif
}
