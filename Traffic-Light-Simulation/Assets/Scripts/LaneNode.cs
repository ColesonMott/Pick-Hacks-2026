using UnityEngine;
using System.Collections.Generic;

public class LaneNode : MonoBehaviour
{
    public enum NodeType
    {
        Start,
        End
    }

    public enum TurnType
    {
        Straight,
        Left,
        Right
    }

    [Header("Node Type")]
    public NodeType nodeType;

    [Header("Connections")]
    public List<LaneNode> nextNodes = new List<LaneNode>();


}