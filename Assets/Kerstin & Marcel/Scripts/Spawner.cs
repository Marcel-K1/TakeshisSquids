using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Made by Marcel

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private int ballAmount = 5000;

    [SerializeField]
    GameObject ballPrefab;

    [SerializeField]
    TimerController timer;

    [SerializeField]
    Transform spawnPoint1;    
    [SerializeField]
    Transform spawnPoint2;    
    [SerializeField]
    Transform spawnPoint3;    
    [SerializeField]
    Transform spawnPoint4;

    [SerializeField]
    private float spawnDelay = 1f;

    void Start()
    {
        StartCoroutine(SpawnBalls(ballAmount));
    }

    private void Update()
    {
        //Setting up the different spawndelays
        if (timer.RoundTime < 130f)
        {
            spawnDelay = 0.5f;
        }
        else if (timer.RoundTime < 100f)
        {
            spawnDelay = 0.25f;
        }
        else if (timer.RoundTime < 30f)
        {
            spawnDelay = 0.1f;
        }

    }

    IEnumerator SpawnBalls(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject ball1 = Instantiate(ballPrefab, new Vector3 (spawnPoint1.position.x + Random.Range(-8f,8f), spawnPoint1.position.y, spawnPoint1.position.z), Quaternion.identity);
            GameObject ball2 = Instantiate(ballPrefab, new Vector3(spawnPoint2.position.x + Random.Range(-8f, 8f), spawnPoint2.position.y, spawnPoint2.position.z), Quaternion.identity);
            GameObject ball3 = Instantiate(ballPrefab, new Vector3(spawnPoint3.position.x, spawnPoint3.position.y, spawnPoint3.position.z +Random.Range(-8f, 8f)), Quaternion.identity);
            GameObject ball4 = Instantiate(ballPrefab, new Vector3(spawnPoint4.position.x, spawnPoint4.position.y, spawnPoint4.position.z + Random.Range(-8f, 8f)), Quaternion.identity);
            
            yield return new WaitForSeconds(spawnDelay);
        }
    }
}
