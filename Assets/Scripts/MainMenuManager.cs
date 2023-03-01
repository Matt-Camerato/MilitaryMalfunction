using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Animator))]
public class MainMenuManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button continueGameButton;
    [SerializeField] private Button nameInputContinueButton;
    [SerializeField] private TMP_InputField nameInputField;

    private Animator anim;
    private bool hasSaveInfo = false;

    private void Start()
    {
        anim = GetComponent<Animator>();

        //see if player has save info or not and set interactablity of continue button
        hasSaveInfo = !PlayerPrefs.GetString("SaveData", "null").Equals("null");
        continueGameButton.interactable = hasSaveInfo;

        //NOTE: update settings slider values based on static values in AudioSystem
    }

    //used by each button to cue SFX
    public void ButtonPressed() => AudioSystem.Instance.ButtonSFX();

    //called at the end of each screen fade to transition to the game scene
    public void StartGame()
    {
        //NOTE: put scene transition here
    }

    //home button methods
    public void NewGameButton() => anim.SetTrigger("HomeToNameInput");
    public void ContinueGameButton() => anim.SetTrigger("HomeToFade");
    public void SettingsButton() => anim.SetTrigger("HomeToSettings");

    //name input methods
    public void OnNameInputChanged(string name)
    {
        //reset continue button interactability
        nameInputContinueButton.interactable = false;

        //check if name input is valid
        //NOTE: this is now mostly being done through the inspector

        //invalid if string is empty
        if(string.IsNullOrEmpty(name)) return;

        //this makes sure first character isn't a space
        char[] chars = name.ToCharArray();
        if(chars[0] == ' ') return;

        //update interactability of continue button
        nameInputContinueButton.interactable = true;
    }
    public void NameInputReturnButton() => anim.SetTrigger("NameInputToHome");
    public void NameInputContinueButton()
    {
        //initialize save data with player's name and wave number starting at 1
        string saveData = nameInputField.text + "_1";
        PlayerPrefs.SetString("SaveData", saveData);

        //fade out before transitioning to game scene
        anim.SetTrigger("NameInputToFade");
    }

    //settings methods
    public void OnMusicSliderChanged(float value) => AudioSystem.Instance.UpdateMusicVolume(value);
    public void OnSFXSliderChanged(float value) => AudioSystem.Instance.UpdateSFXVolume(value);
    public void SettingsReturnButton() => anim.SetTrigger("SettingsToHome");
}