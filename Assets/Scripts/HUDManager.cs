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
    [SerializeField] private CanvasGroup screenFade;

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

    public void Update()
    {

    }
}
