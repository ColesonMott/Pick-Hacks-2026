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
    public float navMeshSampleRadius = 30f;
    public float verticalOffset = 5f;

    private int currentCars = 0;

    void Start()
    {
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
        if (BuildingManager.buildingEntrances == null ||
            BuildingManager.buildingEntrances.Count < 2)
            return;

        Transform startBuilding =
            BuildingManager.buildingEntrances[
                Random.Range(0, BuildingManager.buildingEntrances.Count)];

        Transform endBuilding =
            BuildingManager.buildingEntrances[
                Random.Range(0, BuildingManager.buildingEntrances.Count)];

        if (startBuilding == endBuilding)
            return;

        Vector3 searchPos = startBuilding.position + Vector3.up * verticalOffset;

        if (!NavMesh.SamplePosition(searchPos,
            out NavMeshHit hit,
            navMeshSampleRadius,
            NavMesh.AllAreas))
            return;

        GameObject car = Instantiate(carPrefab);

        NavMeshAgent agent = car.GetComponent<NavMeshAgent>();
        CarAI ai = car.GetComponent<CarAI>();

        if (agent == null || ai == null)
        {
            Destroy(car);
            return;
        }

        // Disable agent before moving
        agent.enabled = false;

        car.transform.position = hit.position;
        car.transform.rotation = Quaternion.identity;

        // Enable agent
        agent.enabled = true;

        // Hard bind to NavMesh
        agent.Warp(hit.position);

        if (!agent.isOnNavMesh)
        {
            Destroy(car);
            return;
        }

        // Ensure movement
        agent.isStopped = false;

        // Set building destination
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