using UnityEngine;
using UnityEngine.UI;

public class TimeScaleSlider : MonoBehaviour
{
    [Header("UI")]
    public Slider timeSlider;
    public Text speedLabel;

    [Header("Settings")]
    public float maxTimeScale = 20f;
    public float baseFixedDelta = 0.02f;

    void Start()
    {
        if (timeSlider == null) return;

        timeSlider.minValue = 0f;
        timeSlider.maxValue = maxTimeScale;
        timeSlider.value = 1f;

        ApplyTimeScale(timeSlider.value);
        timeSlider.onValueChanged.AddListener(ApplyTimeScale);
    }

    void ApplyTimeScale(float value)
    {
        float scale = Mathf.Max(0f, value);

        Time.timeScale = scale;
        Time.fixedDeltaTime = baseFixedDelta * Mathf.Clamp(scale, 0.01f, 100f);

        // No SimTime reference here, so it can’t fail to compile

        if (speedLabel != null)
            speedLabel.text = scale.ToString("0.0") + "x";
    }
}