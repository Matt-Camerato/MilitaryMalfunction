using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemiesKilledPanel : MonoBehaviour
{
    [SerializeField] private List<Sprite> enemyTankSprites = new List<Sprite>();

    [Header("References")]
    [SerializeField] private Image enemyIcon;
    [SerializeField] private TMP_Text killsText;

    public void SetValues(int enemyIndex, int numKilled)
    {
        enemyIcon.sprite = enemyTankSprites[enemyIndex];
        killsText.text = numKilled.ToString();
    }
}
