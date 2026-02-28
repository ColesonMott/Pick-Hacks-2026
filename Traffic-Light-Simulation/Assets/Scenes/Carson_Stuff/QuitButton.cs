using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class QuitButton : MonoBehaviour
{
    // This is the function the UI button will call
    public void QuitGame()
    {
        Debug.Log("Quit button pressed");

#if UNITY_EDITOR
        EditorApplication.isPlaying = false; // Stops play mode in Unity
#else
        Application.Quit(); // Closes built game
#endif
    }
}
