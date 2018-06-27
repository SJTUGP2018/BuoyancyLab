using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFunction : MonoBehaviour {

	public BoatRigidbodySO brSO;
	public GameObject boat;

	BoatInfo boatInfo;

	// Use this for initialization
	void Start () {
		boatInfo = boat.GetComponent<BoatInfo>();
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
	}

	public void Reset(){
		boat.transform.position = boatInfo.originPos;
		boat.transform.rotation = boatInfo.originRotation;
		Destroy(boat.GetComponent<BuoyancyApplier>());
		Destroy(boat.GetComponent<Rigidbody>());
	}

}
