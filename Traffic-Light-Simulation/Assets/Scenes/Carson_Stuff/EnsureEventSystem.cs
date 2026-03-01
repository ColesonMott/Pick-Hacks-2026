using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;   // only if you use the new Input System
using UnityEngine.UI;               // safe to include

public class EnsureEventSystem : MonoBehaviour
{
    void Awake()
    {
        if (FindObjectOfType<EventSystem>() != null)
            return;

        var go = new GameObject("EventSystem");
        go.AddComponent<EventSystem>();

        // If you are using the OLD input system, add this:
        go.AddComponent<StandaloneInputModule>();

        // If you are using the NEW input system, use this instead:
        // go.AddComponent<InputSystemUIInputModule>();

        DontDestroyOnLoad(go);
    }
}