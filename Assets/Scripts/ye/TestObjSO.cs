using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "testRigidbodySetting", menuName = "testFile")]
public class TestObjSO : ScriptableObject {

	public float mass;

	public float drag;

	public float angularDrag;

	//public bool useGravity;
}
