using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeathScreenManager : MonoBehaviour
{
    [SerializeField] private TMP_Text headerText;
    [SerializeField] private TMP_Text subheaderText;
    [SerializeField] private TMP_Text killCounterText;
    [SerializeField] private RectTransform killCounterContent;
    [SerializeField] private GameObject enemiesKilledPrefab;

    //called when player dies and this script is enabled
    private void Start()
    {
        //get save data
        string saveData = PlayerPrefs.GetString("SaveData");
        string[] s = saveData.Split('_');

        //update header and subheader text with player's name and wave number
        headerText.text = "† Sgt. " + s[0] + " †";
        subheaderText.text = "K.I.A. - Wave " + s[1];

        //update size of kill counter content area
        int width = Mathf.Max(305, 50 + (115 * EnemyManager.Instance.EnemiesKilled.Count) 
            + (15 * EnemyManager.Instance.EnemiesKilled.Count - 1));
        killCounterContent.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);

        int totalKills = 0;
        //loop through values in save data representing each enemy type
        for(int i = 0; i < EnemyManager.Instance.EnemiesKilled.Count; i++)
        {
            //get # of kills
            int numKilled = EnemyManager.Instance.EnemiesKilled[i];

            //increment total kills
            totalKills += numKilled;

            //instantiate UI prefab and set values to display kills of each enemy type
            GameObject obj = Instantiate(enemiesKilledPrefab, killCounterContent);
            obj.GetComponent<EnemiesKilledPanel>().SetValues(i, numKilled);
        }
        
        //set total kills text
        killCounterText.text = "Total Confirmed Kills: " + totalKills;
    }
}
