using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class CarSpawner : MonoBehaviour
{
    public GameObject carPrefab;

    [Header("Traffic Settings")]
    public int maxCars = 40;
    public float minSpawnTime = 0.5f;
    public float maxSpawnTime = 2f;

    [Header("Spawn Settings")]
    public float spawnRadius = 80f;
    public float spawnClearRadius = 3f;

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

    Vector3 searchPosition = startBuilding.position + Vector3.up * 5f;

    if (!NavMesh.SamplePosition(searchPosition, out NavMeshHit hit, 25f, NavMesh.AllAreas))
    {
        Debug.LogWarning("No NavMesh found near building: " + startBuilding.name);
        return;
    }

    GameObject car = Instantiate(carPrefab);

    NavMeshAgent agent = car.GetComponent<NavMeshAgent>();

    // Disable BEFORE moving
    agent.enabled = false;

    // Snap EXACTLY to NavMesh
    car.transform.position = hit.position;

    // Enable AFTER placement
    agent.enabled = true;

    // NOW warp to guarantee binding
    agent.Warp(hit.position);

    if (!agent.isOnNavMesh)
    {
        Debug.LogWarning("Agent failed to bind to NavMesh.");
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