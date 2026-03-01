using UnityEngine;

public class SnapLaneNodesFlat : MonoBehaviour
{
    [ContextMenu("Snap All LaneNodes To Flat Y")]
    public void SnapNodes()
    {
        LaneNode[] nodes = FindObjectsOfType<LaneNode>();
        float roadY = 0f; // or whatever your “road height” should be

        foreach (var node in nodes)
        {
            node.transform.position = new Vector3(node.transform.position.x, roadY, node.transform.position.z);
        }

        Debug.Log($"Snapped {nodes.Length} LaneNodes to flat Y={roadY}");
    }
}