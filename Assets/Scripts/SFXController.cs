using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SFXController : MonoBehaviour
{
    [Header("Slider")]
    public Slider SFXSlider;

    [Header("SFX")]
    public AudioClip buttonPress;
    public AudioClip damage;
    public AudioClip shoot;

    [Header("TankAudioSource")]
    public AudioSource tankNoise;

    public void updateSFXVolume()
    {
        GetComponent<AudioSource>().volume = SFXSlider.value;
        if (tankNoise != null)
        {
            tankNoise.volume = SFXSlider.value;
        }
        HUDController.SFXSlider = SFXSlider.value;
    }

    public void ButtonPress()
    {
        GetComponent<AudioSource>().clip = buttonPress;
        GetComponent<AudioSource>().Play();
    }

    public void Damage()
    {
        GetComponent<AudioSource>().clip = damage;
        GetComponent<AudioSource>().Play();
    }

    public void Shoot()
    {
        GetComponent<AudioSource>().clip = shoot;
        GetComponent<AudioSource>().Play();
    }

    public void stopTankNoise()
    {
        tankNoise.loop = false;
    }
}
