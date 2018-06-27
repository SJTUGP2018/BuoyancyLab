using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickInfo : MonoBehaviour {

	public float mass;

	// Use this for initialization
	void Start () {
		mass = mass * transform.lossyScale.x * transform.lossyScale.y * transform.lossyScale.z;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
