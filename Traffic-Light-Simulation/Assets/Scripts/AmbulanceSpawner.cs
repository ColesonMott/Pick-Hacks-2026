using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AmbulanceSpawner : MonoBehaviour
{
    public static AmbulanceSpawner Instance { get; private set; }

    [Header("Ambulance Setup")]
    [Tooltip("Prefab that has AmbulanceAI + NavMeshAgent on it.")]
    public GameObject ambulancePrefab;

    [Tooltip("Where new ambulances will spawn (usually at the hospital).")]
    public Transform spawnPoint;

    [Tooltip("Base/location the ambulance considers 'home'. Optional but nice.")]
    public Transform baseLocation;

    private readonly List<AmbulanceAI> spawnedAmbulances = new List<AmbulanceAI>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[AmbulanceSpawner] Multiple spawners found, destroying the extra one.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    /// <summary>
    /// Spawns a new ambulance at the spawn point and returns its AI.
    /// </summary>
    public AmbulanceAI SpawnAmbulance()
    {
        if (ambulancePrefab == null || spawnPoint == null)
        {
            Debug.LogError("[AmbulanceSpawner] Prefab or spawnPoint not assigned.");
            return null;
        }

        GameObject obj = Instantiate(
            ambulancePrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        AmbulanceAI ai = obj.GetComponent<AmbulanceAI>();
        if (ai == null)
        {
            Debug.LogError("[AmbulanceSpawner] Spawned ambulance has no AmbulanceAI component.");
            return null;
        }

        if (baseLocation != null)
        {
            ai.baseLocation = baseLocation;
        }

        spawnedAmbulances.Add(ai);
        return ai;
    }

    /// <summary>
    /// Spawns an ambulance and sends it to the wreck.
    /// </summary>
    public void DispatchAmbulanceTo(Transform wreckTransform)
    {
        if (wreckTransform == null)
            return;

        AmbulanceAI ambulance = SpawnAmbulance();
        if (ambulance == null)
            return;

        ambulance.DispatchToAccident(wreckTransform);
    }
}