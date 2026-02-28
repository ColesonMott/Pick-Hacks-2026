using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CarSpawner : MonoBehaviour
{
    public List<Transform> trafficNodes;
    public GameObject carPrefab;

    public float minSpawnTime = 0.5f;
    public float maxSpawnTime = 2f;

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpawnCar();
            yield return new WaitForSeconds(Random.Range(minSpawnTime, maxSpawnTime));
        }
    }

    void SpawnCar()
    {
        if (trafficNodes.Count < 2)
            return;

        Transform spawn = trafficNodes[Random.Range(0, trafficNodes.Count)];

        Transform destination;
        do
        {
            destination = trafficNodes[Random.Range(0, trafficNodes.Count)];
        }
        while (destination == spawn);

        GameObject car = Instantiate(carPrefab, spawn.position, Quaternion.identity);

        CarAI ai = car.GetComponent<CarAI>();
        ai.SetDestination(destination.position);
    }
}