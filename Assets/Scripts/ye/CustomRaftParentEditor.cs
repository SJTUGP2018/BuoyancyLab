using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(RaftParent))]
public class CustomRaftParentEditor : Editor {

	public override void OnInspectorGUI(){
		DrawDefaultInspector();
		RaftParent raftParent = (RaftParent) target;
		// if(GUILayout.Button("init childList")){
		// 	raftParent.InitChildList();
		// } 
		// if(GUILayout.Button("add child")){
		// 	raftParent.AddChild();
		// } 
		// if(GUILayout.Button("delete child")){
		// 	raftParent.DeleteChild();
		// } 

		// if(GUILayout.Button("sort child")){
		// 	raftParent.SortChildRaft();
		// } 

		// if(GUILayout.Button("collider off")){
		// 	raftParent.ColliderOff();
		// } 

		// if(GUILayout.Button("collider on")){
		// 	raftParent.ColliderOn();
		// } 

		if(GUILayout.Button("Start")){
			Time.timeScale = 1;
		} 

		if(GUILayout.Button("Stop")){
			Time.timeScale = 0;
		} 

		if(GUILayout.Button("add")){
			raftParent.TransactionAdd();
		} 

		if(GUILayout.Button("dalete")){
			raftParent.TransactionDelete();
		}

		

	}

}
