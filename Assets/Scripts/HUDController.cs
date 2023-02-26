using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public static class ExtensionMethods
{
    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}

public class HUDController : MonoBehaviour
{
    [Header("PlayerController")]
    public PlayerController player;

    [Header("Music & SFX")]
    public AudioSource musicSource;
    public AudioSource SFXSource;

    [Header("Static Slider Values")]
    static public float MusicSlider = 0.75f;
    static public float SFXSlider = 0.75f;

    private Image fireCooldown;
    private GameObject weaponIcon;
    private Image levelFade;

    void Start()
    {
        //setup private variables
        fireCooldown = transform.GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetComponent<Image>();
        weaponIcon = transform.GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).gameObject;
        levelFade = transform.GetChild(6).GetComponent<Image>();

        //set sliders to static variables (makes them same across scenes)
        //in future, will also have to update volumes here manually
        transform.GetChild(4).GetChild(0).GetChild(0).GetChild(1).GetComponent<Slider>().value = MusicSlider;
        transform.GetChild(4).GetChild(0).GetChild(0).GetChild(2).GetComponent<Slider>().value = SFXSlider;
    }

    void Update()
    {
        if (!fadeOut)
        {
            if (!settingsOn)
            {
                if (player.fireCooldown == 0)
                {
                    fireCooldown.color = new Color(1, 1, 1, 1);
                    weaponIcon.SetActive(true);
                }
                else
                {
                    fireCooldown.color = new Color(0.6f, 0.6f, 0.6f, 0.6f);
                    fireCooldown.fillAmount = player.fireCooldown.Remap(player.fireCooldownAmount * 100, 0, 0, 1);
                    weaponIcon.SetActive(false);
                }
            }
        }
        else
        {
            musicSource.GetComponent<MusicController>().fadingOff = true;
            levelFade.raycastTarget = true;
            if (levelFade.color.a < 1)
            {
                //if not fully faded
                levelFade.color = new Color(0, 0, 0, levelFade.color.a + (0.5f * Time.deltaTime));
            }
            else
            {
                //once fully faded, load fadeOutScene
                SceneManager.LoadScene(fadeOutScene);
            }
        }


    }

    [Header("Are Settings On?")]
    public bool settingsOn = false;

    [Header("FadingOut?")]
    public bool fadeOut = false;
    private int fadeOutScene;

    public void Settings()
    {
        //this tells the necessary game components to pause
        settingsOn = !settingsOn;
        //while this turns on and off the actual on-screen settings menu
        transform.GetChild(4).gameObject.SetActive(settingsOn);
    }

    public void MainMenu()
    {
        //reset enemies remaining to 0
        WaveController.enemiesRemaining = 0;

        //check if quit during wave cooldown, and if so, fixes waveNum for next time user plays
        if(WaveController.waveCooldown != 0)
        {
            WaveController.waveNum++;
            SaveSystem.SavePlayerData();
        }

        //then exits to main menu (after fade)
        fadeOutScene = 0;
        fadeOut = true;
    }

    public void NewGame()
    {
        //make sure all static variables are reset (other than name, which is already correctly set)...

        //waveNum is being set to 1 so that player doesn't have to rewatch intro sequence (considering they have 100% already seen it if they are at death screen)
        WaveController.waveNum = 1;
        WaveController.killedEnemies.Clear();

        //...then overwrite save file on user's operating system
        SaveSystem.SavePlayerData();

        //also reset enemies remaining to 0
        WaveController.enemiesRemaining = 0;

        //reloads level scene (after fade)
        fadeOutScene = 1;
        fadeOut = true;
    }
}
