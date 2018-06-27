using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CastResult{
	public GameObject resultObj;
	public bool castable;
}

struct savedRigidbody{
	public float mass;
	public float drag;
	public float angularDrag;
	public bool useGravity;
}

public class BupyancyTestController : MonoBehaviour {
	public LayerMask layerMask = 1 << 9; // TestThing

	SteamVR_TrackedObject trackedObj;

	public float distance = 3;

	float showDistance = 10;

	CastResult res;

	savedRigidbody srb;

	bool setLock = false;

	Rigidbody rb;

	public LineRenderer lr;

	void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

	// Use this for initialization
	void Start () {

		
		//testBrick = Instantiate(selectedBrick);
		//testBrick.GetComponent<Collider>().enabled = false;
		
	}
	
	// Update is called once per frame
	void Update () {

		lr.SetPosition(0, transform.position);
		lr.SetPosition(1, transform.position + transform.forward * showDistance);

		SteamVR_Controller.Device device = SteamVR_Controller.Input((int)trackedObj.index);

		if (device.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
        {
			Vector2 pad = device.GetAxis();
            Vector3 cross = Vector3.Cross(new Vector2(1, 0), pad);
            float angle = Vector2.Angle(new Vector2(1, 0), pad);
            float ang = cross.z > 0 ? -angle : angle;
            Debug.Log(ang);

			if (ang > 0 && ang < 180)
            {
				Debug.Log("down");

				if(res.resultObj != null){
					res.resultObj.transform.Translate(-transform.forward * Time.deltaTime * 5, Space.World);
				}

                
            }
            else if (ang < 0 && ang > -180)
            {
				Debug.Log("up");
				if(res.resultObj != null){
					res.resultObj.transform.Translate(transform.forward * Time.deltaTime * 5, Space.World);
				}
				
                //obj.transform.Translate(Vector3.forward * Time.deltaTime * 5);
            }


            // if (ang > 45 && ang < 135)
            // {
			// 	Debug.Log("down");

            //     //obj.transform.Translate(Vector3.back * Time.deltaTime * 5);
            // }
            // else if (ang < -45 && ang> -135)
            // {
			// 	Debug.Log("up");
				
            //     //obj.transform.Translate(Vector3.forward * Time.deltaTime * 5);
            // }
            // else if ((ang < 180 && ang> 135) || (ang < -135 && ang > -180))
            // {
			// 	Debug.Log("left");
				
            //     //obj.transform.Rotate(Vector3.up * Time.deltaTime * -25);
            // }
            // else if ((ang > 0 && ang< 45) || (ang > -45 && ang< 0)){
			// 	Debug.Log("right");
				
		    // }
		}

		if(device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)){
			if(setLock == false){
				res = testCast();
				if(res.castable != false){
					res.resultObj.transform.position = transform.position + transform.forward * distance;
					res.resultObj.transform.SetParent(transform);
					Destroy(res.resultObj.GetComponent<BuoyancyApplier>());

					rb = res.resultObj.GetComponent<Rigidbody>();

					srb.mass = rb.mass;
					srb.drag = rb.drag;
					srb.angularDrag = rb.angularDrag;
					srb.useGravity = rb.useGravity;

					Destroy(res.resultObj.GetComponent<Rigidbody>());

					setLock = true;
				}
			}
			else{
				if(res.resultObj != null){
		 			res.resultObj.transform.parent = null;

					res.resultObj.AddComponent<Rigidbody>();

					rb = res.resultObj.GetComponent<Rigidbody>();

					rb.mass = srb.mass ;
					rb.drag = srb.drag ;
					rb.angularDrag = srb.angularDrag;
					rb.useGravity = srb.useGravity;

					res.resultObj.AddComponent<BuoyancyApplier>();

					setLock = false;
		 		}
			}

		}

		// if(device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger)){
		// 	if(res.resultObj != null){
		// 		res.resultObj.transform.parent = null;
		// 	}
		// 	// res = testCast();
		// 	// if(res.castable != false){
		// 	// 	res.resultObj.transform.position = transform.position + transform.forward * distance;
		// 	// }
		// }

	}

	CastResult testCast(){
		CastResult cr = new CastResult();
		RaycastHit hit;
		if(Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, layerMask)){
			cr.resultObj = hit.transform.gameObject;
			cr.castable = true;
		}
		else{
			cr.resultObj = null;
			cr.castable = false;
		}
		return cr;
	}

		
}
