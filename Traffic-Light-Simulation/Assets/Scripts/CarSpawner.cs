using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CarSpawner : MonoBehaviour
{
    public GameObject carPrefab;

    [Header("Spawn Points")]
    public List<Transform> spawnPoints;

    [Header("Traffic Settings")]
    public int maxCars = 40;
    public float minSpawnTime = 0.5f;
    public float maxSpawnTime = 2f;

    private int currentCars = 0;

    void Start()
    {
        Debug.Log("NavMesh triangles: " + UnityEngine.AI.NavMesh.CalculateTriangulation().vertices.Length);
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
    if (spawnPoints.Count == 0 || carPrefab == null)
        return;

    Transform spawn = spawnPoints[
        Random.Range(0, spawnPoints.Count)
    ];

    Vector3 spawnPos = spawn.position;

    UnityEngine.AI.NavMeshHit hit;

    // Force snap to NavMesh within 100 units
    if (!UnityEngine.AI.NavMesh.SamplePosition(spawnPos, out hit, 100f, UnityEngine.AI.NavMesh.AllAreas))
    {
        Debug.LogWarning("No NavMesh found near spawn.");
        return;
    }

    spawnPos = hit.position;

    GameObject car = Instantiate(
        carPrefab,
        spawnPos,
        spawn.rotation
    );

    UnityEngine.AI.NavMeshAgent agent = car.GetComponent<UnityEngine.AI.NavMeshAgent>();

    if (!agent.isOnNavMesh)
    {
        agent.Warp(spawnPos);
    }

    CarAI ai = car.GetComponent<CarAI>();
    ai.Initialize(FindNearestStartNode(spawnPos), this);

    currentCars++;
}

    LaneNode FindNearestStartNode(Vector3 position)
    {
        LaneNode[] nodes = FindObjectsOfType<LaneNode>();

        float closest = Mathf.Infinity;
        LaneNode best = null;

        foreach (LaneNode node in nodes)
        {
            if (node.nodeType != LaneNode.NodeType.Start)
                continue;

            float dist = Vector3.Distance(position, node.transform.position);

            if (dist < closest)
            {
                closest = dist;
                best = node;
            }
        }

        return best;
    }

    public void NotifyCarDestroyed()
    {
        currentCars--;
        if (currentCars < 0)
            currentCars = 0;
    }
}