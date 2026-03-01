using System;
using TMPro;
using UnityEngine;

public class HUDClock : MonoBehaviour
{
    public TMP_Text clockText;
    public bool use24Hour = true;

    void Update()
    {
        if (clockText == null) return;

        DateTime now = DateTime.Now;
        clockText.text = use24Hour
            ? now.ToString("HH:mm:ss")
            : now.ToString("hh:mm:ss tt");
    }
}