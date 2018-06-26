using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum BrickType
{
	Cube, Prism
}

public enum ChangeType{
	yawUp,
	yawDown,
	pitchUp,
	pitchDown
}

public static class HelperFunc{

	static public Vector3 GetDirVector(ChangeType ct){
		switch(ct){
			case ChangeType.yawUp:
				return new Vector3(0, 90, 0);
			case ChangeType.yawDown:
				return new Vector3(0, -90, 0);
			case ChangeType.pitchUp:
				return new Vector3(90, 0, 0);
			case ChangeType.pitchDown:
				return new Vector3(-90, 0, 0);
			default:
				return new Vector3(0, 0, 0);
		}

	}

	static public bool Equal(float a, int b){
		if((a - b) > -0.0001 && (a-b) < 0.0001 ){
			return true;
		}
		else{
			return false;
		}
	}

}

public class BrickBehavior : MonoBehaviour{

	public float checkDistance;

	public BrickType bt = BrickType.Cube;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

		// if(Input.GetKeyDown(KeyCode.LeftArrow)){
		// 	RotateBrick(ChangeType.yawUp);
		// }
		// else if(Input.GetKeyDown(KeyCode.RightArrow)){
		// 	RotateBrick(ChangeType.yawDown);
		// }
		// else if(Input.GetKeyDown(KeyCode.UpArrow)){
		// 	RotateBrick(ChangeType.pitchUp);
		// }
		// else if(Input.GetKeyDown(KeyCode.DownArrow)){
		// 	RotateBrick(ChangeType.pitchDown);
		// }
		// else if(Input.GetKeyDown(KeyCode.T)){
		// 	Debug.Log(CheckSettable());

		// }
		// else{
		// 	Debug.Log("invalid");
		// }
	}

	Vector3 GetDirVectorWithNum(int i){
		switch(i){
			case 0:
				// x
				return transform.right;
			case 1:
				// y
				return transform.up;
			case 2:
				// z
				return transform.forward;
			case 3:
				// -x
				return -transform.right;
			case 4:	
				// -y
				return -transform.up;
			case 5:
				// -z
				return -transform.forward;
			default:
				// 0
				return new Vector3(0, 0, 0);
		}
	}

	public void RotateBrick(ChangeType ct){
		transform.Rotate(HelperFunc.GetDirVector(ct));
	}

	// void RotateBrick(Vector3 rotation){
	// 	transform.Rotate(rotation);
	// }

	public bool CheckSettable(){
		switch(bt){
			case BrickType.Cube:
				// y, -y, x, -x, z, -x 
				for(int index = 0; index < 6; index++){
					if(!CheckDirectionSettable(GetDirVectorWithNum(index), checkDistance)){
						return false;
					}
				}
				for(int index = 0; index < 6; index++){
					if(CheckDirectionHaveThing(GetDirVectorWithNum(index), checkDistance)){
						return true;
					}
				}			
				return false;
			case BrickType.Prism:
				// -y
				if(!CheckDirectionSettable(GetDirVectorWithNum(4), checkDistance)){
					return false;
				}
				// -z
				if(!CheckDirectionSettable(GetDirVectorWithNum(5), checkDistance)){
					return false;
				}
				for(int index = 1; index <= 2; index++){
					if(CheckDirectionHaveThing(GetDirVectorWithNum(index), checkDistance)){
						return false;
					}
				}
				return true;
			default:
				return false;
		}
	}

	bool CheckDirectionSettable(Vector3 direction, float checkDistance){
		RaycastHit hit;
		Vector3 originPos = transform.position;
		if(bt == BrickType.Prism){
			originPos += -transform.up * 0.00001f;
			originPos += -transform.forward * 0.00001f;
		}
		//Debug.Log(originPos + " " + direction+ " " + checkDistance);
		if(Physics.Raycast(originPos, direction ,out hit, checkDistance)){

			Debug.Log("here " + hit.transform.gameObject.name + hit.normal);
			if(Vector3.Normalize(hit.normal) == -Vector3.Normalize(direction)){		
				return true;
			}
			else{
				return false;
			}
		}
		else{
			return true;
		}
	}

	bool CheckDirectionHaveThing(Vector3 direction, float checkDistance){
		RaycastHit hit;
		Vector3 originPos = transform.position;
		if(bt == BrickType.Prism){
			originPos += -transform.up * 0.00001f;
			originPos += -transform.forward * 0.00001f;
		}
		//Debug.Log("check thing" + originPos + " " + direction+ " " + checkDistance);
		if(Physics.Raycast(originPos, direction ,out hit, checkDistance)){
			if(hit.transform.gameObject.tag == gameObject.tag){
				return true;
			}
			else{
				return false;
			}
		}
		else{
			return false;
		}
	}

}
