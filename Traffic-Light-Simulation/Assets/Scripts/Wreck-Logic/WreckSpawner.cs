using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class WreckSpawner : MonoBehaviour
{
    public GameObject wreckPrefab;

    public float randomChancePerSecond = 0.01f;

    private int laneAArea;

    void Start()
    {
        laneAArea = NavMesh.GetAreaFromName("LaneA");

        if (laneAArea == -1);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            SpawnWreck();
        }

        if (Random.value < randomChancePerSecond * Time.deltaTime)
        {
            SpawnWreck();
        }
    }

    void SpawnWreck()
{
    NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();

    if (triangulation.vertices.Length == 0)
    {
        return;
    }

    Vector3 randomVertex = triangulation.vertices[
        Random.Range(0, triangulation.vertices.Length)
    ];

    Instantiate(wreckPrefab, randomVertex, Quaternion.identity);

}
}