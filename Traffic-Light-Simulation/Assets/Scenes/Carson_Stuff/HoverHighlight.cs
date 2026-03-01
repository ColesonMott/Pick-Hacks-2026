using UnityEngine;

public class HoverHighlight : MonoBehaviour
{
    Renderer[] rends;

    [Header("Highlight")]
    public Color highlightColor = Color.yellow;
    public float emissionStrength = 2f;

    void Awake()
    {
        rends = GetComponentsInChildren<Renderer>();
    }

    public void SetHighlight(bool state)
    {
        for (int i = 0; i < rends.Length; i++)
        {
            var mat = rends[i].material;

            if (state)
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", highlightColor * emissionStrength);
            }
            else
            {
                mat.DisableKeyword("_EMISSION");
            }
        }
    }
}