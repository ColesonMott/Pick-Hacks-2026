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
    public float navMeshSearchRadius = 50f;
    public float verticalSampleOffset = 10f;
    public float spawnClearRadius = 2f;

    private int currentCars = 0;

    private int laneAArea;
    private int laneBArea;

    void Start()
    {
        Debug.Log("NavMesh triangles: " + NavMesh.CalculateTriangulation().vertices.Length);

        // Cache area indices
        laneAArea = NavMesh.GetAreaFromName("LaneA");
        laneBArea = NavMesh.GetAreaFromName("LaneB");

        if (laneAArea == -1 || laneBArea == -1)
        {
            Debug.LogError("LaneA or LaneB NavMesh area not found! Create them in Navigation Areas.");
        }

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

        Vector3 searchPosition = startBuilding.position + Vector3.up * verticalSampleOffset;

        if (!NavMesh.SamplePosition(
            searchPosition,
            out NavMeshHit hit,
            navMeshSearchRadius,
            NavMesh.AllAreas))
        {
            Debug.LogWarning("No NavMesh found near building " + startBuilding.name);
            return;
        }

        // Prevent overlapping cars
        Collider[] overlaps = Physics.OverlapSphere(hit.position, spawnClearRadius);
        foreach (Collider col in overlaps)
        {
            if (col.GetComponent<CarAI>() != null)
                return;
        }

        GameObject car = Instantiate(carPrefab);

        NavMeshAgent agent = car.GetComponent<NavMeshAgent>();

        agent.enabled = false;
        car.transform.position = hit.position + Vector3.up * 0.1f;
        agent.enabled = true;

        if (!agent.isOnNavMesh)
        {
            Destroy(car);
            return;
        }

        // 🔥 LOCK LANE DIRECTION HERE
        LockAgentToSpawnLane(agent);

        CarAI ai = car.GetComponent<CarAI>();
        ai.SetBuildingDestination(endBuilding);

        currentCars++;
    }

    void LockAgentToSpawnLane(NavMeshAgent agent)
    {
        NavMeshHit areaHit;

        if (NavMesh.SamplePosition(
            agent.transform.position,
            out areaHit,
            2f,
            NavMesh.AllAreas))
        {
            int mask = areaHit.mask;

            if ((mask & (1 << laneAArea)) != 0)
            {
                agent.areaMask = 1 << laneAArea;
                return;
            }

            if ((mask & (1 << laneBArea)) != 0)
            {
                agent.areaMask = 1 << laneBArea;
                return;
            }

            Debug.LogWarning("Spawned on unknown NavMesh area.");
        }
    }

    public void NotifyCarDestroyed()
    {
        currentCars--;
        if (currentCars < 0)
            currentCars = 0;
    }
}