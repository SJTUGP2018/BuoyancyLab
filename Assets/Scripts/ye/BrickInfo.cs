using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickInfo : MonoBehaviour {

	public float mass;

	[HideInInspector]
	public Material mat;

	public List<BrickSO> brickSOs;

	//public BrickSO currentSO;

	int index = 0;

	public Vector3 centerOfMass;

	// Use this for initialization
	void Start () {
		//mass = brickSOs[index].mass;
		mat = brickSOs[index].mat;
		//mass = mass * transform.lossyScale.x * transform.lossyScale.y * transform.lossyScale.z;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	// public void UpdateCenterOfMass(){
	// 	centerOfMass ;
	// }

	public void ChangeBrickInfo(){
		if(index < brickSOs.Count -1){
			index ++;
		}
		else{
			index = 0;
		}
		//Debug.Log(index);
		mass = brickSOs[index].mass;
		mass = mass * transform.lossyScale.x * transform.lossyScale.y * transform.lossyScale.z;
		mat = brickSOs[index].mat;
	}
}
