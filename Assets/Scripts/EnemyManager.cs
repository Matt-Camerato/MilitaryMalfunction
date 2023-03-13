using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    [Header("Enemies To Spawn")]
    [SerializeField] private List<GameObject> enemyPrefabs = new List<GameObject>();

    public int EnemiesKilled = 0;
    public int EnemiesRemaining = 0;

    private void Awake() => Instance = this;

    public void SpawnEnemy()
    {

    }
}
