using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatInfo : MonoBehaviour {

	public float boatMass = 0;

	public int limit = 10;

	[HideInInspector]
	public Vector3 originPos;

	[HideInInspector]
	public Quaternion originRotation;

	// Use this for initialization
	void Start () {
		originPos = transform.position;
		originRotation = transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void UpdateBoatMass(){
		foreach(Transform child in transform){
			boatMass += child.GetComponent<BrickInfo>().mass;
		}
	}

	public int childNum(){
		return transform.childCount;
	}

}
