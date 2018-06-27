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

	public Vector3 boatCenter = new Vector3(0 ,0 ,0);

	BrickInfo bi;

	// Use this for initialization
	void Start () {
		originPos = transform.position;
		originRotation = transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void UpdateBoatMass(){
		boatMass = 0;
		foreach(Transform child in transform){
			if(child.tag == "Brick"){
				boatMass += child.GetComponent<BrickInfo>().mass;
			}
		}
	}

	public int childNum(){
		int i = 0;
		foreach(Transform child in transform){
			if(child.gameObject.activeInHierarchy && child.tag == "Brick"){
				i ++; 
			}
		}
		return i;
	}

	public void UpdateboatCenter(){
		boatCenter = new Vector3(0, 0, 0);
		foreach(Transform child in transform){
			if(child.gameObject.activeInHierarchy && child.tag == "Brick"){
				bi = child.GetComponent<BrickInfo>();
				boatCenter += bi.transform.TransformPoint(bi.centerOfMass) * bi.mass;				
			}
		}
		boatCenter = transform.InverseTransformPoint(boatCenter / boatMass);
		//Debug.Log(boatCenter);
	}

}
