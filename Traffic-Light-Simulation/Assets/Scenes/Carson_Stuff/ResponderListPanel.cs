using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResponderDashboardList : MonoBehaviour
{
    [Header("UI")]
    public Transform listContent;              // VerticalLayoutGroup container
    public Button responderButtonPrefab;       // a UI Button prefab (with TMP child)

    [Header("Refresh")]
    public float refreshEverySeconds = 1f;
    float nextRefresh;

    void Start() => Rebuild();

    void Update()
    {
        if (Time.time >= nextRefresh)
        {
            nextRefresh = Time.time + refreshEverySeconds;
            Rebuild();
        }
    }

    void Rebuild()
    {
        if (listContent == null || responderButtonPrefab == null) return;
        if (ResponderRegistry.Instance == null) return;

        // Clear old buttons
        for (int i = listContent.childCount - 1; i >= 0; i--)
            Destroy(listContent.GetChild(i).gameObject);

        // Build new buttons
        foreach (var v in ResponderRegistry.Instance.responders)
        {
            if (v == null) continue;

            var btn = Instantiate(responderButtonPrefab, listContent);
            var label = btn.GetComponentInChildren<TMP_Text>();
            if (label != null)
                label.text = $"{v.displayName} ({v.type})";

            btn.onClick.AddListener(() =>
            {
                if (TopDownOrbitCam.Instance != null)
                    TopDownOrbitCam.Instance.SetTarget(v.transform);
            });
        }
    }
}