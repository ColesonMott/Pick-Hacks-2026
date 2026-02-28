using UnityEngine;
using System.Collections.Generic;

public class TrafficNode : MonoBehaviour
{
    [Tooltip("Nodes cars are allowed to drive to from this node")]
    public List<TrafficNode> connectedNodes = new List<TrafficNode>();

    [Tooltip("Optional: Mark as spawnable")]
    public bool isSpawnNode = true;
}