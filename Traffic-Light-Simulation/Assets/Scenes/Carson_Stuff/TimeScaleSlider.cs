using UnityEngine;
using UnityEngine.UI;

public class TimeScaleSlider : MonoBehaviour
{
    [Header("UI")]
    public Slider timeSlider;
    public Text speedLabel; // optional text display

    [Header("Settings")]
    public float maxTimeScale = 20f;
    public float baseFixedDelta = 0.02f;

    void Start()
    {
        if (timeSlider != null)
        {
            timeSlider.minValue = 0f;
            timeSlider.maxValue = maxTimeScale;
            timeSlider.value = 1f;

            ApplyTimeScale(timeSlider.value);
            timeSlider.onValueChanged.AddListener(ApplyTimeScale);
        }
    }

    void ApplyTimeScale(float value)
    {
        // Prevent tiny unstable physics values
        float scale = Mathf.Max(0f, value);

        Time.timeScale = scale;

        // VERY IMPORTANT for vehicle physics
        Time.fixedDeltaTime = baseFixedDelta * Time.timeScale;

        if (speedLabel != null)
            speedLabel.text = scale.ToString("0.0") + "x";
    }
}