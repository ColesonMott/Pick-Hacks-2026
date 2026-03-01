using UnityEngine;

public class StartCameraSwitcher : MonoBehaviour
{
    public Camera startCamera;
    public Camera mainCamera;

    public GameObject startMenuUI; // ⭐ your button/menu

    void Start()
    {
        startCamera.enabled = true;
        mainCamera.enabled = false;
    }

    // Called by the button
    public void StartGame()
    {
        // Switch cameras
        startCamera.enabled = false;
        mainCamera.enabled = true;

        // ⭐ Hide the UI
        if (startMenuUI != null)
            startMenuUI.SetActive(false);

        // Optional: fix audio listener warning
        AudioListener startAL = startCamera.GetComponent<AudioListener>();
        AudioListener mainAL = mainCamera.GetComponent<AudioListener>();

        if (startAL) startAL.enabled = false;
        if (mainAL) mainAL.enabled = true;
    }
}