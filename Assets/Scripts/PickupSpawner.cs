using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
    [SerializeField] private GameObject pickupPrefab;

    private List<Transform> spawnPoints;
    private Transform lastSpawnPoint = null;

    private GameObject currentPickup = null;

    private void Start()
    {
        //initialize list of spawn points
        spawnPoints = new List<Transform>();
        foreach(Transform child in transform) spawnPoints.Add(child);

        SpawnPickup(); //spawn first pickup
    }

    private void Update()
    {
        //check if there is already a pickup spawned
        if(currentPickup != null) return;

        //if not, spawn a new pickup
        SpawnPickup();
    }

    private void SpawnPickup()
    {
        //get random spawn point
        int index = Random.Range(0, spawnPoints.Count);
        Transform spawnPoint = spawnPoints[index];
        if(lastSpawnPoint != null)
        {
            //make sure it wasn't the last spawn point used
            while(spawnPoint == lastSpawnPoint)
            {
                index = Random.Range(0, spawnPoints.Count);
                spawnPoint = spawnPoints[index];
            }
        }

        //update last spawn point and spawn pickup prefab
        lastSpawnPoint = spawnPoint;
        currentPickup = Instantiate(pickupPrefab, spawnPoint.position, Quaternion.identity);
    }
}
