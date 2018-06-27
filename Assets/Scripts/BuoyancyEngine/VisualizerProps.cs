using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NormalForceVisualizer",
                 menuName = "Force Visualizer/Props")
]
public class ForceVisualizerProps : ScriptableObject {

    public GameObject arrowBody;
    public GameObject arrowHead;

    public float gravityReferenceLength = 5f;
    public float diameterFactor = 0.1f;
    public float arrowHeadFactor = 0.2f;
    public Vector3 arrowHeadComponentFactor =
        (Vector3.up + Vector3.forward + Vector3.right);
}
