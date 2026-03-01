using UnityEngine;
using System.Collections.Generic;

public class CarSpawner : MonoBehaviour
{
    [Header("Car Settings")]
    public CarAI carPrefab;
    public int initialCarCount = 10;
    public float laneOffsetAmount = 1.5f; // half lane width

    [Header("Lane Nodes")]
    public List<LaneNode> allLaneNodes = new List<LaneNode>();

    private List<LaneNode> startNodes = new List<LaneNode>();

    void Awake()
    {
        // Automatically find all LaneNodes in the scene
        allLaneNodes.Clear();
        allLaneNodes.AddRange(FindObjectsOfType<LaneNode>());

        // Filter only Start nodes
        startNodes.Clear();
        foreach (var node in allLaneNodes)
        {
            if (node.nodeType == LaneNode.NodeType.Start)
            {
                // Calculate laneIndex automatically from position
                node.CalculateLaneIndex();
                startNodes.Add(node);
            }
        }

        Debug.Log("Start nodes found: " + startNodes.Count);

        // Spawn initial cars
        for (int i = 0; i < initialCarCount; i++)
        {
            SpawnCar();
        }
    }

    public void SpawnCar()
    {
        if (startNodes.Count == 0)
        {
            Debug.LogWarning("No Start nodes found!");
            return;
        }

        LaneNode startNode = startNodes[Random.Range(0, startNodes.Count)];

        // Spawn position slightly above ground
        Vector3 spawnPos = startNode.transform.position + Vector3.up * 0.5f;

        // Apply lane offset
        Vector3 offset = startNode.transform.right * laneOffsetAmount * startNode.laneIndex;
        spawnPos += offset;

        // Instantiate the car
        CarAI car = Instantiate(carPrefab, spawnPos, Quaternion.identity);

        // Make car face the next node or road
        Vector3 forward = (startNode.nextNodes.Count > 0)
            ? (startNode.nextNodes[0].transform.position - startNode.transform.position).normalized
            : startNode.transform.forward;

        // Apply rotation offset if model is not Z+ forward
        Quaternion rotationOffset = Quaternion.Euler(0, 90f, 0); // adjust if needed
        if (forward != Vector3.zero)
            car.transform.rotation = Quaternion.LookRotation(forward, Vector3.up) * rotationOffset;

        // Initialize CarAI with start node
        car.Initialize(startNode);
    }
}