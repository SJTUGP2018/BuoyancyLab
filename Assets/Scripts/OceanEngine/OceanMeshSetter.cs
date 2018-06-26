using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OceanMeshSetter : MonoBehaviour {

	public OceanHeightMap heightMap;
	MeshFilter meshFilter;

	// Use this for initialization
	void Start () {
		meshFilter = GetComponent<MeshFilter>();
	}
	
	// Update is called once per frame
	void Update () {
		if(heightMap.oceanMesh != null)
		{
            meshFilter.sharedMesh = OceanManager.Instance.oceanMesh;
		}
		
	}
}
