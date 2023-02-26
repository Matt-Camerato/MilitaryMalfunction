using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicController : MonoBehaviour
{
    [Header("Slider")]
    public Slider MusicSlider;

    [Header("Fade Bools")]
    public bool fadingOn = true;
    public bool fadingOff = false;

    public void Update()
    {
        if (fadingOn)
        {
            if (GetComponent<AudioSource>().volume != HUDController.MusicSlider)
            {
                if (GetComponent<AudioSource>().volume < HUDController.MusicSlider)
                {
                    GetComponent<AudioSource>().volume += 0.5f * Time.deltaTime;
                }
            }
            else
            {
                fadingOn = false;
            }
        }

        if (fadingOff)
        {
            if (GetComponent<AudioSource>().volume > 0)
            {
                GetComponent<AudioSource>().volume -= 0.5f * Time.deltaTime;
            }
            else
            {
                fadingOff = false;
            }
        }
    }

    public void updateMusicVolume()
    {
        GetComponent<AudioSource>().volume = MusicSlider.value;
        HUDController.MusicSlider = MusicSlider.value;
    }
}
