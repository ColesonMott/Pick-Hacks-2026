using TMPro;
using UnityEngine;

public class HUDClock : MonoBehaviour
{
    public TMP_Text clockText;
    public bool use24Hour = true;

    void Awake()
    {
        if (clockText == null)
            clockText = GetComponent<TMP_Text>();
    }

    void Update()
    {
        if (clockText == null) return;

        // If SimTime isn't present yet, fall back to system time (optional)
        var now = (SimTime.Instance != null) ? SimTime.Instance.Now : System.DateTime.Now;

        clockText.text = use24Hour
            ? now.ToString("HH:mm:ss")
            : now.ToString("hh:mm:ss tt");
    }
}