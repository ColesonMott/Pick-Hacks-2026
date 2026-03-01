using UnityEngine;
using UnityEngine.AI;

public class WreckSpawner : MonoBehaviour
{
    [Header("Wreck Spawning")]
    public GameObject wreckPrefab;

    [Tooltip("Chance per second to spawn a random wreck.")]
    public float randomChancePerSecond = 0.01f;

    private void Update()
    {
        // Manual spawn for testing
        if (Input.GetKeyDown(KeyCode.L))
        {
            SpawnWreck();
        }

        // Random automatic spawning
        if (Random.value < randomChancePerSecond * Time.deltaTime)
        {
            SpawnWreck();
        }
    }

    private void SpawnWreck()
    {
        if (wreckPrefab == null)
        {
            Debug.LogWarning("[WreckSpawner] No wreckPrefab assigned.");
            return;
        }

        NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();

        if (triangulation.vertices == null || triangulation.vertices.Length == 0)
        {
            Debug.LogWarning("[WreckSpawner] NavMesh has no vertices; cannot spawn wreck.");
            return;
        }

        // Pick a random point on the NavMesh
        Vector3 randomVertex = triangulation.vertices[
            Random.Range(0, triangulation.vertices.Length)
        ];

        // Snap to nearest valid NavMesh position
        Vector3 spawnPos = randomVertex;
        if (NavMesh.SamplePosition(randomVertex, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            spawnPos = hit.position;
        }

        GameObject wreck = Instantiate(wreckPrefab, spawnPos, Quaternion.identity);

        // Automatically dispatch an ambulance, if we have a spawner in the scene
        if (AmbulanceSpawner.Instance != null)
        {
            AmbulanceSpawner.Instance.DispatchAmbulanceTo(wreck.transform);
        }
        else
        {
            Debug.LogWarning("[WreckSpawner] No AmbulanceSpawner.Instance in scene; wreck spawned but no ambulance dispatched.");
        }
    }
}