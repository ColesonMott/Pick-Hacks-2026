using UnityEngine;
using UnityEditor;

public class AutoTagBuildings
{
    [MenuItem("Tools/Tag All Buildings In Scene")]
    static void TagBuildingsInScene()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        int count = 0;

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.ToLower().Contains("building"))
            {
                obj.tag = "Building";
                count++;
            }
        }

        Debug.Log("Tagged " + count + " buildings in scene.");
    }
}