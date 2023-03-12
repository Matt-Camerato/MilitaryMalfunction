using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    [SerializeField] private CanvasGroup screenFade;

    [HideInInspector] public bool isPaused;

    private string playerName;

    private void Awake() => Instance = this;

    private void Start()
    {
        //load player name from save data and update HUD
        string saveData = PlayerPrefs.GetString("SaveData");
        string[] s = saveData.Split('_');
        playerName = "Sgt. " + s[0];
        playerNameText.text = playerName;

        //cue screen fade-in
        StartCoroutine(DoScreenFade(1, 0));
    }

    public void Update()
    {
        //check if intro sequence is over
        if(!IntroManager.Instance.doneIntro) return;

        //toggle settings by pressing the escape key
        if(Input.GetKeyDown(KeyCode.Escape)) ToggleSettings();


    }

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

    //fades screen from one alpha value to another 
    private IEnumerator DoScreenFade(float start, float end)
    {
        float a = start;
        while(a != end)
        {
            screenFade.alpha = a;
            if(a > end) a -= Time.deltaTime;
            else a += Time.deltaTime;
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
