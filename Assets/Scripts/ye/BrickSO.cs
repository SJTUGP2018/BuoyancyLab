using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "brickSO", menuName = "BrickFile")]
public class BrickSO : ScriptableObject {
	public float mass;

	public Material mat;
}
