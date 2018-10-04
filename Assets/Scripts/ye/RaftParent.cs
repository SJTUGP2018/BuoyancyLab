using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaftParent : MonoBehaviour {

	public GameObject raftChild;

	public bool timeStop;

	public float number;

	public int rowSize = 5;

	List<GameObject> childList = new List<GameObject>();

	FixedJoint joint;

	public Vector3 moveVector = new Vector3(1, 0, 2); 

	// Use this for initialization
	void Start () {
		if(timeStop){
			Time.timeScale = 0;
		}
		
		initRaftList();
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log(childList.Count);
	}

	public void TransactionAdd(){
		InitChildList();
		ColliderOff();
		AddChild();
		SortChildRaft();
		ColliderOn();
	}

	public void TransactionDelete(){
		DeleteChild();
	}

	public void ColliderOff(){
		foreach(Transform child in transform){
			child.GetComponent<Collider>().enabled = false;
		}
	}

	public void ColliderOn(){
		foreach(Transform child in transform){
			child.GetComponent<Collider>().enabled = true;
		}
	}

	public void AddChild(){
		//Debug.Log(number);
		number++;
		//Debug.Log(number);
		updateChildRaft(childList, number);
		//SortChildRaft();
	}

	public void DeleteChild(){

		if(number > 0){
			number--;
			updateChildRaft(childList, number);
		}
		else{

		}

	}

	public void InitChildList(){
		initRaftList();
	}

	void initRaftList(){
		childList.Clear();
		foreach(Transform child in transform){
			childList.Add(child.gameObject);
		}
		number = childList.Count;		
	}

	void updateChildRaft(List<GameObject> childList, float number){
		float count = childList.Count;
		GameObject operatingObject;
		if(count > number){
			for(int i = 0; i < count - number; i ++){
				operatingObject = childList[childList.Count - 1];
				DestroyImmediate(childList[childList.Count - 1]);
				childList.Remove(operatingObject);
			}
		}
		else{
			for(int i = 0; i < number - count; i ++){				
				operatingObject = Instantiate(raftChild,gameObject.transform);
				childList.Add(operatingObject);
			}
		}
	}

	void SortChildRaftReal(List<GameObject> childList, int rowSize){
		int count = childList.Count;
		int row = (count / rowSize);

		int remainder  = count - rowSize * row;
		Vector3 originPos = gameObject.transform.position;

		childList[0].transform.localPosition = new Vector3(0, 0, 0);

		//childList[0].GetComponent<FixedJoint>().

		for(int p = 1; p < rowSize; p++){
			childList[p].transform.localPosition = new Vector3(moveVector.x * p, 0, 0);

			joint = childList[p].GetComponent<FixedJoint>();

			if(!joint){
				childList[p].AddComponent<FixedJoint>();
				joint = childList[p].GetComponent<FixedJoint>();
			}

			joint.connectedBody = childList[p-1].GetComponent<Rigidbody>();
		}


		for(int i = 1; i < row; i++){
			for(int j = 0; j < rowSize; j ++){
				childList[j + i * rowSize].transform.localPosition = new Vector3(moveVector.x * j, 0, moveVector.z * i);

				joint = childList[j + i * rowSize].GetComponent<FixedJoint>();

				if(!joint){
					childList[j + i * rowSize].AddComponent<FixedJoint>();
					joint = childList[j + i * rowSize].GetComponent<FixedJoint>();
				}
			
				joint.connectedBody = childList[j + (i-1) * rowSize].GetComponent<Rigidbody>();
			}
		}

		for(int s = 0; s < remainder; s++){
			childList[s + row * rowSize].transform.localPosition = new Vector3(moveVector.x * s, 0, moveVector.z * row);

			joint = childList[s + row * rowSize].GetComponent<FixedJoint>();

			if(!joint){
				childList[s + row * rowSize].AddComponent<FixedJoint>();
				joint = childList[s + row * rowSize].GetComponent<FixedJoint>();
			}
			
			joint.connectedBody = childList[s + (row-1) * rowSize].GetComponent<Rigidbody>();
		}

	}

	public void SortChildRaft(){
		SortChildRaftReal(childList, rowSize);
	}

}
