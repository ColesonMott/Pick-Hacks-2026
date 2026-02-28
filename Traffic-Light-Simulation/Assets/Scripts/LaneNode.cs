using UnityEngine;
using System.Collections.Generic;

public class LaneNode : MonoBehaviour
{
    public enum NodeType
    {
        Start,
        End
    }

    public NodeType nodeType;

    public List<LaneNode> nextNodes = new List<LaneNode>();
}