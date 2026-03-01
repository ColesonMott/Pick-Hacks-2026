using System;
using UnityEngine;

public class SimTime : MonoBehaviour
{
    public static SimTime Instance { get; private set; }

    [Tooltip("1 = real-time. 60 = 1 real second = 1 sim minute.")]
    public float timeScale = 1f;

    public bool paused = false;

    private DateTime startTime;
    private double simSecondsElapsed;

    public DateTime Now => startTime.AddSeconds(simSecondsElapsed);

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        // Sim starts at your computer time
        startTime = DateTime.Now;
        simSecondsElapsed = 0;
    }

    void Update()
    {
        if (paused) return;

        // Use deltaTime so sim clock pauses when Time.timeScale = 0
        simSecondsElapsed += Time.deltaTime * timeScale;
    }

    public void SetScale(float newScale) => timeScale = Mathf.Max(0f, newScale);
    public void Pause(bool p) => paused = p;
}