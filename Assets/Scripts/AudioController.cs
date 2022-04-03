using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
{
    //Enums
    //====================================================================================================================//
    
    public enum SOUND
    {
        UI_Press,
        DISSOLVE,
        POP,
        RUMBLE,
        BREAK
    }
    
    [Serializable]
    public struct SoundData
    {
        public SOUND type;
        public AudioClip audioClip;
    }
    
    //Properties
    //====================================================================================================================//

    private const string VOLUME = "volume";
    
    private static AudioController Instance;

    [SerializeField]
    private AudioSource sfxAudioSource;
    [SerializeField]
    private AudioMixer masterMixer;

    [SerializeField]
    private AnimationCurve volumeCurve;

    [SerializeField]
    private SoundData[] soundDatas;
    private Dictionary<SOUND, AudioClip> _sounds;


    //Unity Functions
    //====================================================================================================================//

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        if (soundDatas == null || soundDatas.Length == 0)
            return;
        
        _sounds = new Dictionary<SOUND, AudioClip>();
        foreach (var soundData in soundDatas)
        {
            if (_sounds.ContainsKey(soundData.type))
                throw new Exception();
            
            _sounds.Add(soundData.type, soundData.audioClip);
        }
    }

    //====================================================================================================================//

    public static void PlaySound(in SOUND soundType, in float volume = 1f)
    {
        Instance.Play(soundType, volume);
    }

    public static void SetVolume(in float volume)
    {
        Instance.SetVolume(volume);
    }

    //====================================================================================================================//
    
    private void Play(SOUND soundType, float volume)
    {
        sfxAudioSource.PlayOneShot(_sounds[soundType], volume);
    }

    private void SetVolume(float volumeValue)
    {
        masterMixer.SetFloat(VOLUME, volumeCurve.Evaluate(volumeValue));
    }

    //====================================================================================================================//
    
    
}
