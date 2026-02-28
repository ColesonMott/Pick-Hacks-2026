// using UnityEngine;
// using System.Collections.Generic;

// public class CarDetector : MonoBehaviour
// {
//     [Header("Direction This Detector Represents")]
//     public string direction; 
//     // Example: "North", "South", etc.

//     private HashSet<CarAI> carsInZone = new HashSet<CarAI>();

//     [Header("Live Counts (Read Only)")]
//     [SerializeField] private int leftCount;
//     [SerializeField] private int straightCount;
//     [SerializeField] private int rightCount;

//     public int LeftCount => leftCount;
//     public int StraightCount => straightCount;
//     public int RightCount => rightCount;

//     private void OnTriggerEnter(Collider other)
//     {
//         if (!other.CompareTag("Car")) return;

//         CarAI car = other.GetComponent<CarAI>();
//         if (car == null) return;

//         if (carsInZone.Contains(car)) return;

//         carsInZone.Add(car);
//         RecalculateCounts();
//     }

//     private void OnTriggerExit(Collider other)
//     {
//         if (!other.CompareTag("Car")) return;

//         CarAI car = other.GetComponent<CarAI>();
//         if (car == null) return;

//         if (carsInZone.Remove(car))
//         {
//             RecalculateCounts();
//         }
//     }

//     private void Update()
//     {
//         // Safety cleanup in case cars are destroyed
//         carsInZone.RemoveWhere(car => car == null);
//         RecalculateCounts();
//     }

//     private void RecalculateCounts()
//     {
//         leftCount = 0;
//         straightCount = 0;
//         rightCount = 0;

//         foreach (CarAI car in carsInZone)
//         {
//             switch (car.intendedTurn)
//             {
//                 case TurnDirection.Left:
//                     leftCount++;
//                     break;

//                 case TurnDirection.Right:
//                     rightCount++;
//                     break;

//                 default:
//                     straightCount++;
//                     break;
//             }
//         }
//     }

//     private void OnDrawGizmos()
//     {
//         BoxCollider box = GetComponent<BoxCollider>();
//         if (box == null) return;

//         Gizmos.color = Color.cyan;
//         Gizmos.matrix = transform.localToWorldMatrix;
//         Gizmos.DrawWireCube(box.center, box.size);
//     }
// }