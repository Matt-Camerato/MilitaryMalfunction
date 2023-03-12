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

    [Header("Enemy Settings")]
    [SerializeField] private List<GameObject> enemyPrefabs = new List<GameObject>();

    [Header("UI Elements")]
    [SerializeField] private TMP_Text waveHeaderText;
    [SerializeField] private TMP_Text waveInfoText;

    private bool betweenWaves = true;
    private float waveCooldown;

    private void Awake()
    {
        //save singleton reference
        Instance = this;

        //load current wave from save data
        string saveData = PlayerPrefs.GetString("SaveData");
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
            //wait for enemies to all die
            //end wave once enemies are all dead
        }

    }

    private void DoWaveCooldown()
    {
        if(waveCooldown > 0)
        {
            //update wave info text
            waveInfoText.text = "Next wave will begin in " + Mathf.Ceil(waveCooldown);
            waveCooldown -= Time.deltaTime;
        }
        else StartWave();
    }

    private void StartWave()
    {
        betweenWaves = false;

        //spawn enemies

    }

    private void EndWave()
    {
        betweenWaves = true;
        currentWave++;
    }
}
