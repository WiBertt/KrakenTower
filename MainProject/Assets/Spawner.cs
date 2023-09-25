using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject objectToSpawn;
    [SerializeField] private float spawnCoolDown;
    [SerializeField] private int maxSpawnAmt;
    [SerializeField] private UnityEvent spawnEvent;

    private int spawnCount;

    private void Start()
    {
        InvokeRepeating("Spawn", 0, spawnCoolDown);
    }

    private void Spawn()
    {
        if (spawnCount >= maxSpawnAmt)
            return;

        Instantiate(objectToSpawn, transform.position, Quaternion.identity);
        spawnCount++;
        spawnEvent?.Invoke();
    }
}
