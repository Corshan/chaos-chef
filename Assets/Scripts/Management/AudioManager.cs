using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Singleton { get; private set; }
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private float _minVolume, _maxVolume;
    public float MinVolume => _minVolume;
    public float MaxVolume => _maxVolume;

    void Awake()
    {
        if (Singleton != null && Singleton != this) Destroy(this);
        else
        {
            Singleton = this;
            DontDestroyOnLoad(this);
        }
    }

    public void ChangeMusicVolume(float volume)
    {
        _audioMixer.SetFloat("MusicVolume", volume);
    }

    public void ChangeSFXVolume(float volume)
    {
        _audioMixer.SetFloat("SfxVolume", volume);
    }
    
    public float MusicVolume()
    {
        _audioMixer.GetFloat("MusicVolume", out float volume);
        return volume;
    }

    public float SfxVolume()
    {
        _audioMixer.GetFloat("SfxVolume", out float volume);
        return volume;
    }
}
