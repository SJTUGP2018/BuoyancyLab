using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// for Unity Job System:
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.Jobs;

public class OceanManager : Singleton<OceanManager> 
{
    // guarantee this will be always a singleton only - can't use the constructor!
    protected OceanManager() { } 
	
	public OceanGenerator generator;

	public float oceanDensity = 1027f;

	public HeightMapDescriptor descriptor
	{
		get
		{
			return generator.descriptor;
		}
	}
	public JobHandle heightMapHandle
	{
		get
		{
			return generator.heightMapHandle;
		}
	}
    /// 1D array for storing heights
    public NativeArray<float> m_heightMap
	{
		get
		{
			return generator.m_heightMap;
		}
	}
    public Mesh oceanMesh
	{
		get
		{
			return generator.oceanMesh;
		}
	}

	void Awake()
	{
		DontDestroyOnLoad(this.gameObject);
	}

    void Update () {
		generator.UpdateOcean(Time.time);
	}
	
	void LateUpdate () {
		generator.LateUpdateOcean(Time.time);
	}

}
