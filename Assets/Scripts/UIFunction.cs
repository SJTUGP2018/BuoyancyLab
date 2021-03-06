﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFunction : MonoBehaviour {

	public BoatRigidbodySO brSO;
	public GameObject boat;

	public TestObjSO testSO;

	BoatInfo boatInfo;

	BuoyancyVisualizer bv;

	GravityVisualizer gv;

	Rigidbody testRB;

	Vector3 originObjPos;

	Quaternion originObjRot;

	public GameObject testObj;

	public GameObject suc;

	public GameObject fail;

	public float goalTime = 5;

	public float failHeight = -2f;

	float timeStart;

	// Use this for initialization
	void Start () {
		boatInfo = boat.GetComponent<BoatInfo>();
		bv = boat.GetComponent<BuoyancyVisualizer>();
		gv = boat.GetComponent<GravityVisualizer>();
		
		if(testObj){
			originObjPos = testObj.transform.position;
			originObjRot = testObj.transform.rotation;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SaveAndCalc(){
		boatInfo.UpdateBoatMass();
		boat.AddComponent<Rigidbody>();
		boat.AddComponent<BuoyancyApplier>();
		Rigidbody rb = boat.GetComponent<Rigidbody>();
		rb.mass = boatInfo.boatMass;
		rb.drag = brSO.drag;
		rb.angularDrag = brSO.angularDrag;
		rb.useGravity = brSO.useGravity;
		boatInfo.UpdateboatCenter();
		rb.centerOfMass = boatInfo.boatCenter;
		Debug.Log(rb.centerOfMass);
		//Debug.Log(rb.centerOfMass);
		//Debug.DrawLine(boat.transform.position, rb.worldCenterOfMass, Color.yellow);
		if(bv){
			bv.enabled = true;
		}
		if(gv){
			gv.enabled = true;
		}

	}

	public void Reset(){
		if(bv){
			bv.enabled = false;
		}
		if(gv){
			gv.enabled = false;
		}
		boat.transform.position = boatInfo.originPos;
		boat.transform.rotation = boatInfo.originRotation;
		Destroy(boat.GetComponent<BuoyancyApplier>());
		Destroy(boat.GetComponent<Rigidbody>());	
	}

	void ReleaseTest(GameObject testObject){
		testRB = testObject.GetComponent<Rigidbody>();
		testRB.mass = testSO.mass;
		testRB.drag = testSO.drag;
		testRB.angularDrag = testSO.angularDrag;
		testRB.useGravity = true;
	}

	void ResetTest(GameObject testObject){
		testRB = testObject.GetComponent<Rigidbody>();
		//testRB.mass = testSO.mass;
		//testRB.drag = testSO.drag;
		//testRB.angularDrag = testSO.angularDrag;
		testRB.useGravity = false;
		testObj.transform.position = originObjPos;
		testObj.transform.rotation = originObjRot;
	}

	public void Test(){
		timeStart = Time.timeSinceLevelLoad;
		SaveAndCalc();
		ReleaseTest(testObj);
		StartCoroutine(TestIE());
	}

	public void TestReset(){
		Reset();
		ResetTest(testObj);
		StopAllCoroutines();
		suc.SetActive(false);
		fail.SetActive(false);
	}

	IEnumerator TestIE(){
		while(Time.timeSinceLevelLoad - timeStart < goalTime){
			if(testObj.transform.position.y < failHeight){
				fail.SetActive(true);
				yield return null;
				//StopAllCoroutines();
			}
			else{
				yield return null;
			}
		}
		if(fail.activeInHierarchy == false){
			suc.SetActive(true);
		}
		
	}
	
}
