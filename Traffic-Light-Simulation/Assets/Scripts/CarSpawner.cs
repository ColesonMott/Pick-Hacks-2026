using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CarSpawner : MonoBehaviour
{
    public GameObject carPrefab;
    public int maxCars = 30;
    public float minSpawnTime = 0.5f;
    public float maxSpawnTime = 2f;

    private int currentCars = 0;
    private List<LaneNode> spawnNodes = new List<LaneNode>();

void Start()
{
    LaneNode[] allNodes = FindObjectsOfType<LaneNode>();

    spawnNodes.Clear();

    foreach (LaneNode node in allNodes)
    {
        if (node.nodeType == LaneNode.NodeType.Start)
        {
            spawnNodes.Add(node);
        }
    }

    Debug.Log("Total LaneNodes: " + allNodes.Length);
    Debug.Log("SpawnNodes found: " + spawnNodes.Count);

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
        if (spawnNodes.Count == 0 || carPrefab == null)
            return;

        LaneNode spawnNode = spawnNodes[
            Random.Range(0, spawnNodes.Count)
        ];

        GameObject car = Instantiate(
            carPrefab,
            spawnNode.transform.position,
            spawnNode.transform.rotation
        );

        CarAI ai = car.GetComponent<CarAI>();
        ai.Initialize(spawnNode, this);

        currentCars++;
    }

    public void NotifyCarDestroyed()
    {
        currentCars--;
        if (currentCars < 0)
            currentCars = 0;
    }
}