using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "rigidbodySetting", menuName = "BoatFile")]
public class BoatRigidbodySO : ScriptableObject {

	//public float mass;

	public float drag;

	public float angularDrag;

	public bool useGravity;

}
