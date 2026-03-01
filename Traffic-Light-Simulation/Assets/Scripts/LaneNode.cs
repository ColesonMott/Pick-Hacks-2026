using System.Collections.Generic;
using UnityEngine;

public class LaneNode : MonoBehaviour
{
    public enum NodeType { Start, End }
    public NodeType nodeType;

    [Header("Lane Info")]
    public bool isForward = true;    
    public List<LaneNode> nextNodes = new List<LaneNode>();

    [HideInInspector]
    public int laneIndex = 0; // will be calculated automatically

    /// <summary>
    /// Auto-calculate laneIndex based on position relative to parent road center
    /// </summary>
    public void CalculateLaneIndex(float laneSeparation = 1.5f)
            {
                if (transform.parent == null)
                {
                    laneIndex = 0;
                    return;
                }

                // Parent is the road, assume lanes go along Z axis
                Vector3 localPos = transform.localPosition;

                // Right lane = positive X, left lane = negative X
                laneIndex = localPos.x >= 0 ? 0 : 1;

                // Optional: visually show laneIndex
                Renderer rend = GetComponent<Renderer>();
                if (rend != null)
                    rend.material.color = laneIndex == 0 ? Color.green : Color.blue;
            }
}