using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// for Unity Job System:
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.Jobs;

public abstract class OceanGenerator : ScriptableObject {

	public HeightMapDescriptor editorDescriptor;
	protected HeightMapDescriptor m_descriptor;

	public HeightMapDescriptor descriptor
	{
		get
		{
			return m_descriptor;
		}
	}


    // 1D array for storing heights
    public NativeArray<float> m_heightMap;
	public JobHandle heightMapHandle;


    public Mesh oceanMesh;

	public abstract void UpdateOcean(float _time);
	public abstract void LateUpdateOcean(float _time);
	public abstract void DestroyOcean();

	
	// Use this for initialization
	protected virtual void OnEnable () 
	{
		m_descriptor = editorDescriptor;
		m_heightMap = new NativeArray<float>(m_descriptor.pixelSize * m_descriptor.pixelSize,
            Allocator.Persistent);
	}

	/// OnDisable should dispose any native allocated memory
	protected virtual void OnDisable()
	{
		DestroyOcean();
	}

}
