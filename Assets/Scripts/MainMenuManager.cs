using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class MainMenuManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button continueGameButton;
    [SerializeField] private Button nameInputContinueButton;
    [SerializeField] private InputField nameInputField;

    private Animator anim;
    private bool hasSaveInfo = false;

    private void Start()
    {
        anim = GetComponent<Animator>();

        //see if player has save info or not and set interactablity of continue button
        hasSaveInfo = !PlayerPrefs.GetString("SaveData", "null").Equals("null");
        continueGameButton.interactable = hasSaveInfo;
    }

    //called at the end of each screen fade to transition to the game scene
    public void StartGame()
    {
        //put scene transition here
    }

    //home button methods
    public void NewGameButton() => anim.SetTrigger("HomeToNameInput");
    public void ContinueGameButton() => anim.SetTrigger("HomeToFade");
    public void SettingsButton() => anim.SetTrigger("HomeToSettings");

    //name input methods
    public void OnNameInputChanged()
    {
        //check if name input is valid

        //update interactability of continue button
        //nameInputContinueButton.interactable = 
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
