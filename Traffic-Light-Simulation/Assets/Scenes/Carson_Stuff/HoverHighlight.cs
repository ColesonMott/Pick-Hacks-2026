using UnityEngine;

public class HoverHighlight : MonoBehaviour
{
    Renderer[] rends;

    public Color highlightColor = Color.yellow;
    public float emissionStrength = 2f;

    void Awake()
    {
        rends = GetComponentsInChildren<Renderer>();
    }

    public void SetHighlight(bool on)
    {
        foreach (var r in rends)
        {
            var mat = r.material;
            if (on)
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