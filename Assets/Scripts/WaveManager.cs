using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    public int currentWave;

    [Header("Wave Settings")]
    [SerializeField] private int timeBetweenWaves = 15;

    [Header("Enemy Settings")]
    [SerializeField] private List<GameObject> enemyPrefabs = new List<GameObject>();

    

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
        


    }

    private IEnumerator WaveCooldown()
    {
        int timeRemaining = timeBetweenWaves;
        while(timeRemaining > 0)
        {
            //update wave countdown display


            yield return new WaitForSeconds(1);
        }
    }

    private void StartWave()
    {
        
    }
}
