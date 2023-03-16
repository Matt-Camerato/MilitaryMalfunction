using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    public int currentWave;

    [Header("Wave Settings")]
    [SerializeField] private int timeBetweenWaves = 15;

    [Header("UI Elements")]
    [SerializeField] private TMP_Text waveHeaderText;
    [SerializeField] private TMP_Text waveInfoText;

    private bool betweenWaves = true;
    private float waveCooldown;

    private string saveData;

    private void Awake()
    {
        //save singleton reference
        Instance = this;

        //load current wave from save data
        saveData = PlayerPrefs.GetString("SaveData");
        string[] s = saveData.Split('_');
        currentWave = int.Parse(s[1]);
    }

    private void Start()
    {
        //set initial wave header and info text
        waveCooldown = timeBetweenWaves;
        waveHeaderText.text = "Get Ready...";
        waveInfoText.text = "Next wave will begin in " + waveCooldown;
    }

    private void Update()
    {
        //check if intro sequence is over
        if(!IntroManager.Instance.doneIntro) return;

        //check if paused
        if(HUDManager.Instance.isPaused) return;

        //check if player is dead
        if(PlayerController.Instance.isDead) return;

        //handle cooldown between waves
        if(betweenWaves) DoWaveCooldown();
        else
        {
            //update enemies remaining counter
            waveInfoText.text = EnemyManager.Instance.EnemiesRemaining + " enemies remaining";

            //end wave once enemies are all dead
            if(EnemyManager.Instance.EnemiesRemaining == 0) EndWave();
        }
    }

    private void DoWaveCooldown()
    {
        if(waveCooldown > 0)
        {
            //update wave header and info text
            waveHeaderText.text = "Get Ready...";
            waveInfoText.text = "Next wave will begin in " + Mathf.Ceil(waveCooldown);
            waveCooldown -= Time.deltaTime;
        }
        else StartWave();
    }

    private void StartWave()
    {
        betweenWaves = false;

        //update wave number text
        waveHeaderText.text = "Wave " + currentWave;

        //spawn enemies
        EnemyManager.Instance.SpawnEnemies();
    }

    private void EndWave()
    {
        betweenWaves = true;
        waveCooldown = 15;
        currentWave++;

        //update player's save data with new wave number
        saveData = PlayerPrefs.GetString("SaveData");
        string[] s = saveData.Split('_');
        string newSave = s[0] + '_' + currentWave;
        
        //also update number of each enemy type killed
        for(int i = 0; i < EnemyManager.Instance.EnemiesKilled.Count; i++)
        {
            newSave = newSave + '_' + EnemyManager.Instance.EnemiesKilled[i].ToString();
        }
        
        PlayerPrefs.SetString("SaveData", newSave);
        saveData = newSave;
    }
}
