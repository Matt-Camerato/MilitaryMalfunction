using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSystem : MonoBehaviour
{
    public static AudioSystem Instance;
    public static float MusicVolume, SFXVolume;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource tankSource;

    [Header("SFX Clips")]
    [SerializeField] private AudioClip button;
    [SerializeField] private AudioClip damage;
    [SerializeField] private AudioClip fire;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void UpdateMusicVolume(float value)
    {
        musicSource.volume = value;
        
        MusicVolume = value;
    }

    public void UpdateSFXVolume(float value)
    {
        sfxSource.volume = value;
        tankSource.volume = value;

        SFXVolume = value;
    }

    public void ButtonSFX() => sfxSource.PlayOneShot(button);
    public void DamageSFX() => sfxSource.PlayOneShot(damage);
    public void FireSFX() => sfxSource.PlayOneShot(fire);

    public void ToggleTankNoise()
    {
        if(tankSource.isPlaying) tankSource.Stop();
        else tankSource.Play();
    }
}
