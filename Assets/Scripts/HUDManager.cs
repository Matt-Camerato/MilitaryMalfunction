using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance;

    [Header("UI Elements")]
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private Image healthBar;
    [SerializeField] private Image armorBar;
    [SerializeField] private Image reloadBar;
    [SerializeField] private GameObject reloadBarFull;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Slider musicSlider, sfxSlider;

    [SerializeField] private CanvasGroup screenFade;

    [HideInInspector] public bool isPaused;

    private string playerName;

    private void Awake() => Instance = this;

    private void Start()
    {
        //load player name from save data and update HUD
        string saveData = PlayerPrefs.GetString("SaveData");
        string[] s = saveData.Split('_');
        playerName = s[0];
        playerNameText.text = "Sgt. " + playerName;

        //update slider values based on static values in AudioSystem
        musicSlider.value = AudioSystem.MusicVolume;
        sfxSlider.value = AudioSystem.SFXVolume;

        //cue screen fade-in
        StartCoroutine(DoScreenFade(1, 0, 3));
    }

    public void Update()
    {
        //check if intro sequence is over
        if(!IntroManager.Instance.doneIntro) return;

        //check if player is dead
        if(PlayerController.Instance.isDead) return;

        //toggle settings by pressing the escape key
        if(Input.GetKeyDown(KeyCode.Escape)) ToggleSettings();
    }

    //called by player controller to update HUD bar fill values
    public void UpdateHUDBars(float healthFill, float armorFill, float reloadFill)
    {
        //update bar fill values
        healthBar.fillAmount = healthFill;
        armorBar.fillAmount = armorFill;
        reloadBar.fillAmount = reloadFill;

        //turn on full reload bar when fully reloaded
        if(reloadFill <= 0) reloadBarFull.SetActive(true);
        else reloadBarFull.SetActive(false);
    }

    //used by each button to cue SFX
    public void ButtonPressed() => AudioSystem.Instance?.ButtonSFX();
    
    //public methods for UI elements
    public void OnMusicSliderChanged(float value) => AudioSystem.Instance?.UpdateMusicVolume(value);
    public void OnSFXSliderChanged(float value) => AudioSystem.Instance?.UpdateSFXVolume(value);
    public void PlayAgainButton() => StartCoroutine(PlayAgain());
    public void SaveAndQuitButton() => StartCoroutine(Quit());
    public void QuitButton()
    {
        //reset save data first
        PlayerPrefs.SetString("SaveData", "null");
        StartCoroutine(Quit());
    }

    //fades screen to black, then reset's player save and reloads scene
    private IEnumerator PlayAgain()
    {
        yield return DoScreenFade(0, 1, 3);
        
        //reset save data
        string saveData = playerName + "_1";
        PlayerPrefs.SetString("SaveData", saveData);

        //reload scene
        SceneManager.LoadScene(1);
    }

    //fades screen to black, then switches scenes
    private IEnumerator Quit()
    {
        yield return DoScreenFade(0, 1, 3);
        SceneManager.LoadScene(0);
    }

    //fades screen from one alpha value to another over given duration
    private IEnumerator DoScreenFade(float start, float end, float duration)
    {
        float a = start;
        while(Mathf.Abs(a - end) < 0.0001f)
        {
            screenFade.alpha = a;
            if(a > end) a -= (Time.deltaTime / duration);
            else a += (Time.deltaTime / duration);
            yield return null;
        }
        screenFade.alpha = end;
    }

    private void ToggleSettings()
    {
        isPaused = !isPaused;
        settingsPanel.SetActive(isPaused);
    }
}
