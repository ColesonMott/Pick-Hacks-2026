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

        // Prevent overlapping spawns
        Collider[] overlaps = Physics.OverlapSphere(hit.position, spawnClearRadius);
        foreach (Collider col in overlaps)
        {
            if (col.GetComponent<CarAI>() != null)
                return;
        }

        GameObject car = Instantiate(carPrefab);
        NavMeshAgent agent = car.GetComponent<NavMeshAgent>();

        // Disable agent before moving
        agent.enabled = false;

        car.transform.position = hit.position;

        // Determine correct forward direction from road
        Vector3 laneForward = FindLaneForward(hit.position);
        car.transform.rotation = Quaternion.LookRotation(laneForward, Vector3.up);

        agent.enabled = true;
        agent.Warp(hit.position);

        if (!agent.isOnNavMesh)
        {
            Destroy(car);
            return;
        }

        CarAI ai = car.GetComponent<CarAI>();
        ai.SetBuildingDestination(endBuilding);

        currentCars++;
    }

    // 🔥 Finds nearest road direction to align spawn properly
    Vector3 FindLaneForward(Vector3 position)
    {
        Ray ray = new Ray(position + Vector3.up * 2f, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, 10f))
        {
            Transform road = hit.collider.transform;

            // Use road forward if aligned prefab
            return road.forward.normalized;
        }

        return Vector3.forward; // fallback
    }

    public void NotifyCarDestroyed()
    {
        currentCars--;
        if (currentCars < 0)
            currentCars = 0;
    }
}