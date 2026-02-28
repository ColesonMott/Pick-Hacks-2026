using UnityEngine;
using System.Collections;

public class SimpleTrafficLight : MonoBehaviour
{
    public MeshRenderer redLightMesh;
    public MeshRenderer yellowLightMesh;
    public MeshRenderer greenLightMesh;

    public Material lightsOnMat;
    public Material lightsOffMat;

    public float greenTime = 5f;
    public float yellowTime = 2f;
    public float redTime = 5f;

    private void Start()
    {
        StartCoroutine(LightCycle());
    }

    private IEnumerator LightCycle()
    {
        while (true)
        {
            // Green
            SetLights(true, false, false);
            yield return new WaitForSeconds(greenTime);

            // Yellow
            SetLights(false, true, false);
            yield return new WaitForSeconds(yellowTime);

            // Red
            SetLights(false, false, true);
            yield return new WaitForSeconds(redTime);
        }
    }

    private void SetLights(bool greenOn, bool yellowOn, bool redOn)
    {
        greenLightMesh.material = greenOn ? lightsOnMat : lightsOffMat;
        yellowLightMesh.material = yellowOn ? lightsOnMat : lightsOffMat;
        redLightMesh.material = redOn ? lightsOnMat : lightsOffMat;
    }
}