using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class CarSpawner : MonoBehaviour
{
    public GameObject carPrefab;

    [Header("Traffic Settings")]
    public int maxCars = 40;
    public float minSpawnTime = 0.5f;
    public float maxSpawnTime = 2f;

    [Header("Spawn Settings")]
    public float navMeshSearchRadius = 20f;
    public float verticalSampleOffset = 5f;
    public float spawnClearRadius = 2f;

    private int currentCars = 0;

    void Start()
    {
        Debug.Log("NavMesh triangles: " + NavMesh.CalculateTriangulation().vertices.Length);
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (currentCars < maxCars)
                SpawnCar();

            yield return new WaitForSeconds(Random.Range(minSpawnTime, maxSpawnTime));
        }
    }

void SpawnCar()
{
    if (BuildingManager.buildingEntrances.Count < 2)
        return;

    Transform startBuilding = BuildingManager.buildingEntrances[
        Random.Range(0, BuildingManager.buildingEntrances.Count)
    ];

    Transform endBuilding = BuildingManager.buildingEntrances[
        Random.Range(0, BuildingManager.buildingEntrances.Count)
    ];

    if (startBuilding == endBuilding)
        return;

    // 🔥 KEY FIX: Sample from ABOVE building
    Vector3 searchPosition = startBuilding.position + Vector3.up * 10f;

    if (!UnityEngine.AI.NavMesh.SamplePosition(
        searchPosition,
        out UnityEngine.AI.NavMeshHit hit,
        50f,                         // large radius
        UnityEngine.AI.NavMesh.AllAreas))
    {
        Debug.LogWarning("No NavMesh found near building " + startBuilding.name);
        return;
    }

    // Prevent overlapping cars
    Collider[] overlaps = Physics.OverlapSphere(hit.position, 2f);
    foreach (Collider col in overlaps)
    {
        if (col.GetComponent<CarAI>() != null)
            return;
    }

    // 🔥 SAFE INSTANTIATE
    GameObject car = Instantiate(carPrefab);

    UnityEngine.AI.NavMeshAgent agent = car.GetComponent<UnityEngine.AI.NavMeshAgent>();

    agent.enabled = false;
    car.transform.position = hit.position + Vector3.up * 0.1f;
    agent.enabled = true;

    if (!agent.isOnNavMesh)
    {
        Destroy(car);
        return;
    }

    CarAI ai = car.GetComponent<CarAI>();
    ai.SetBuildingDestination(endBuilding);

    currentCars++;
}

    public void NotifyCarDestroyed()
    {
        currentCars--;
        if (currentCars < 0)
            currentCars = 0;
    }
}