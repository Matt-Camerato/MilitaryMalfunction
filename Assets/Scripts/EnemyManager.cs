using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    [Header("Enemy Info")]
    public int EnemiesKilled = 0;
    public int EnemiesRemaining = 0;

    [SerializeField] private List<GameObject> enemyPrefabs = new List<GameObject>();
    [SerializeField] private List<Transform> spawnPositions = new List<Transform>();

    private bool noMoreTypes = false;

    //5 main spawn variants used for spawning current and new enemy types
    private Vector2[] spawnVariants = { 
        new Vector2(2, 0),
        new Vector2(3, 0),
        new Vector2(3, 1),
        new Vector2(1, 2),
        new Vector2(2, 3)
    };

    private void Awake() => Instance = this;

    //determines which enemies to spawn based on current wave number
    public void SpawnEnemies()
    {
        //first determine current and next enemy types
        GameObject currentEnemy, newEnemy;

        //calculate current enemy index (for rounds 1-5, this is 0; rounds 6-10, this is 1; etc.)
        int currentEnemyType = Mathf.FloorToInt((WaveManager.Instance.currentWave - 1) / 5);

        //if current enemy doesn't exist at index, choose a random enemy type
        if(currentEnemyType >= enemyPrefabs.Count) currentEnemy = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
        else currentEnemy = enemyPrefabs[currentEnemyType];

        //if nwe enemy doesn't exist at index, choose a random enemy type
        if(currentEnemyType + 1 >= enemyPrefabs.Count) newEnemy = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
        else newEnemy = enemyPrefabs[currentEnemyType + 1];
        
        //determine spawn variant to find number of current and new enemies to spawn
        int spawnVariantIndex = (WaveManager.Instance.currentWave - 1) % 5;
        Vector2 spawnVariant = spawnVariants[spawnVariantIndex];

        //spawn current and new enemies
        for(int i = 0; i < spawnVariant.x; i++) SpawnEnemy(currentEnemy);
        for(int j = 0; j < spawnVariant.y; j++) SpawnEnemy(newEnemy);

        //spawn extra enemies (round 1-5 has 0 extra, 6-10 has 1 extra, etc.)
        for(int k = 0; k < currentEnemyType; k++)
        {
            //choose a random enemy type out of all previously spawned enemy types
            int randomEnemyIndex = Random.Range(0, currentEnemyType);
            SpawnEnemy(enemyPrefabs[randomEnemyIndex]);
        }
    }

    //spawns enemy of given type
    private void SpawnEnemy(GameObject enemy)
    {
        //loop until spawn position is found that player cannot currently see
        Transform spawnPos = null;
        while(spawnPos == null)
        {
            spawnPos = spawnPositions[Random.Range(0, spawnPositions.Count)];
            if(Physics2D.OverlapCircle(spawnPos.position, 20, LayerMask.GetMask("Player"))) spawnPos = null;
        }

        //spawn enemy prefab at spawn position
        Instantiate(enemy, spawnPos.position, Quaternion.identity);

        //increment enemies remaining counter
        EnemiesRemaining++;
    }
}
