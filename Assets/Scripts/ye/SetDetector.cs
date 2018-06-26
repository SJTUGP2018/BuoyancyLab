using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetDetector : MonoBehaviour {

	public LayerMask layerMask = 1 << 8;

	Vector3Int settable = new Vector3Int(999, 999, 999);

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		BrickDetect(transform);
		Debug.Log(settable);
	}

	bool BrickDetect(Transform trans){
		RaycastHit hit;
		if(Physics.Raycast(trans.position, trans.forward, out hit, Mathf.Infinity, layerMask)){
			Debug.Log("point: " + hit.point);
			Debug.Log("normal" + hit.normal);
			settable = Hitpoint2Grid(hit.point, hit.normal);
			Debug.DrawLine(transform.position, settable, Color.red);
			return true;
		}
		else{
			return false;
		}
	}

	Vector3Int Hitpoint2Grid(Vector3 point, Vector3 direction){
		
		Vector3 newPoint = point + direction;
		newPoint = (newPoint + point) / 2.0f;
		Vector3Int result = new Vector3Int(Mathf.RoundToInt(newPoint.x),Mathf.RoundToInt(newPoint.y),Mathf.RoundToInt(newPoint.z));
		return result;
	}


}
