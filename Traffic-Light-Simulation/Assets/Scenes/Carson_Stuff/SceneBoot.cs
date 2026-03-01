using UnityEngine;

public class StartCameraSwitcher : MonoBehaviour
{
    public Camera startCamera;
    public Camera mainCamera; // your MainCamera

    void Start()
    {
        // Start on intro camera
        startCamera.enabled = true;
        mainCamera.enabled = false;
    }

    // ⭐ THIS is what the button will call
    public void StartGame()
    {
        startCamera.enabled = false;
        mainCamera.enabled = true;

        // Optional: fix audio listener warning
        AudioListener startAL = startCamera.GetComponent<AudioListener>();
        AudioListener mainAL = mainCamera.GetComponent<AudioListener>();

        if (startAL) startAL.enabled = false;
        if (mainAL) mainAL.enabled = true;
    }
}