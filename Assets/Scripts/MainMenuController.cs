using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [Header("Music Player")]
    public AudioSource MusicPlayer;

    private TMP_InputField nameInputField;
    private Image gameFade;

    private bool fading = false;
    private bool doneStartFade = false;

    private void Start()
    {
        //on game start up, load save data from user operating system
        PlayerData savedData = SaveSystem.LoadPlayerData();
        
        //check if save data actually exists
        if (savedData == null)
        {
            //IF SAVE DATA DOESN'T EXIST

            //disable continue game button
            transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetComponent<Button>().interactable = false;
        }
        else
        {
            //IF SAVE DATA DOES EXIST

            //load data to static variables
            WaveController.PLAYER_NAME = savedData._playerName;
            WaveController.waveNum = savedData._waveNum;
            //first clear current list of killed enemies, then add range of killed enemies from save data
            WaveController.killedEnemies.Clear();
            WaveController.killedEnemies.AddRange(savedData._killedEnemies);
        }
        

        //setup private variables
        nameInputField = transform.GetChild(0).GetChild(0).GetChild(2).GetChild(0).GetChild(2).GetComponent<TMP_InputField>();
        gameFade = transform.GetChild(1).GetComponent<Image>();
        

        //set sliders to static variables (makes them same across scenes)
        //in future, will also have to update volumes here manually
        //STILL NEED TO FIX SLIDERS SO THAT THEY CALL A FUNCTION ON VALUE CHANGED THAT CHANGES THE STATIC SLIDER VARIABLES ACCORDINGLY
        transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<Slider>().value = HUDController.MusicSlider;
        transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(1).GetComponent<Slider>().value = HUDController.SFXSlider;
    }

    private void Update()
    {
        if (doneStartFade)
        {
            if (fading)
            {
                MusicPlayer.GetComponent<MusicController>().fadingOff = true;
                //check fade value
                if (gameFade.color.a < 1)
                {
                    //if not fully faded
                    gameFade.color = new Color(0, 0, 0, gameFade.color.a + (0.5f * Time.deltaTime));
                }
                else
                {
                    //once fully faded, load next scene
                    SceneManager.LoadScene(1);
                }
            }
        }
        else
        {
            gameFade.color = new Color(0, 0, 0, gameFade.color.a - (0.5f * Time.deltaTime));
            if(gameFade.color.a <= 0)
            {
                gameFade.raycastTarget = false;
                doneStartFade = true;
            }
        }
    }

    public void NameInput()
    {
        //turns on/off main menu 
        transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(!transform.GetChild(0).GetChild(0).GetChild(0).gameObject.activeSelf);
        //turns on/off nameInput 
        transform.GetChild(0).GetChild(0).GetChild(2).gameObject.SetActive(!transform.GetChild(0).GetChild(0).GetChild(2).gameObject.activeSelf);
    }

    public void onNameInputUpdate()
    {
        //this makes sure player name has a non-space character in it in order to be usable
        bool usableName = false;
        for (int i = 0; i < nameInputField.text.Length; i++)
        {
            if (nameInputField.text[i] != ' ')
            {
                usableName = true;
            }
        }

        //this makes sure first character isn't a space in order for name to be usable
        if(nameInputField.text.Length != 0)
        {
            if(nameInputField.text[0] == ' ')
            {
                usableName = false;
            }
        }

        //after name checks are done, this enables button to begin game if name is usable, otherwise it disables the button
        if (usableName)
        {
            transform.GetChild(0).GetChild(0).GetChild(2).GetChild(0).GetChild(4).GetComponent<Button>().interactable = true;
        }
        else
        {
            transform.GetChild(0).GetChild(0).GetChild(2).GetChild(0).GetChild(4).GetComponent<Button>().interactable = false;
        }
    }

    public void NewGame()
    {
        //make sure name is set and other static variables are reset... 
        WaveController.PLAYER_NAME = nameInputField.text;
        WaveController.waveNum = 0;
        WaveController.killedEnemies.Clear();

        //...then create new save file on user's operating system...
        SaveSystem.SavePlayerData();

        //...and lastly load game scene (after fade)
        fading = true;
        gameFade.raycastTarget = true;
    }

    public void ContinueGame()
    {
        //if continuing game, save data is already loaded (in Start() method)

        //so just begin game (after fade)
        fading = true;
        gameFade.raycastTarget = true;
    }

    public void Settings()
    {
        //turns on/off settings menu
        transform.GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(!transform.GetChild(0).GetChild(0).GetChild(1).gameObject.activeSelf);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
