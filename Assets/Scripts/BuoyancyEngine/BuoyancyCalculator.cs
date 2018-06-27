using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// for Unity Job System:
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.Jobs;

public struct BuoyancyResult
{
	public Vector3 origin0;
    public Vector3 force0;
    public Vector3 origin1;
    public Vector3 force1;
}


public class BuoyancyCalculator : MonoBehaviour {

    public int USE_RESULTANT_LIMIT = 100;
	private const bool USE_RESULTANT = false;

	[HideInInspector]
	BuoyancyResult[] managedResults;
	NativeArray<BuoyancyResult> m_results;

    Mesh mesh;
	NativeArray<Vector3> m_vertices;
    NativeArray<int> m_triangles;
	int triCount;

	public int triangleCount
	{
		get{
			if(mesh == null) 
				Debug.Log(gameObject.name+"[WARNING]: get triangle Count from null mesh");
			return triCount;
		}
	}

	public BuoyancyResult[] resultArray
	{
		get{
			return managedResults;
		}
	}

	public bool calculateTorques{
		get; private set;
	}

	public Vector2[] torqueResultArray
	{
		get{
			return managedtorqueResults;
		}
	}

	NativeArray<Vector2> m_torqueResults;
	Vector2[] managedtorqueResults;

	JobHandle handle;

    CalculateBuoyancyJob cbjob;
    SumTorqueJob stjob;


	// public OceanHeightMap ocm;
	// HeightMapDescriptor descriptor
	// {
	// 	get
	// 	{
	// 		return ocm.descriptor;
	// 	}
	// }
	// NativeArray<float> heightMap
	// {
	// 	get
	// 	{
	// 		return ocm.m_heightMap;
	// 	}
	// }
	// JobHandle gheightHandle
	// {
	// 	get
	// 	{
	// 		return ocm.gHeightHandle;
	// 	}
	// }


	// Use this for initialization
	void Start () 
	{
		MeshCollider meshCollider = GetComponent<MeshCollider>();
		MeshFilter meshFilter = GetComponent<MeshFilter>();
	
		if(meshCollider != null) 
			mesh = meshCollider.sharedMesh;
		else if(meshFilter != null) 
			mesh = meshFilter.sharedMesh;
		else
		{
			Debug.Log(gameObject.name + "[WARNING]: cannot get mesh");
		}

		triCount = mesh.triangles.GetLength(0) / 3;


		m_vertices = new NativeArray<Vector3>(mesh.vertices, Allocator.Persistent);
		m_triangles = new NativeArray<int>(mesh.triangles, Allocator.Persistent);
        m_results = new NativeArray<BuoyancyResult>(triCount, Allocator.Persistent);

		managedResults = new BuoyancyResult[triCount];

		// if(triCount > USE_RESULTANT_LIMIT && USE_RESULTANT)
		// 	calculateTorques = true;
		// else
			calculateTorques = false;

		if(calculateTorques)
		{
            m_torqueResults = new NativeArray<Vector2>(triCount, Allocator.Persistent);
            managedtorqueResults = new Vector2[triCount];
        }
        
	}
	
	// Update is called once per frame
	void Update () {
		cbjob = new CalculateBuoyancyJob()
		{
			vertices = m_vertices,
			triangles = m_triangles,
			localToWorldMatrix = transform.localToWorldMatrix,
            results = m_results,
			rho = OceanManager.Instance.oceanDensity,
			gravity = 9.81f,

			descriptor = OceanManager.Instance.descriptor,
			heightMap = OceanManager.Instance.m_heightMap,
		};

		handle = cbjob.Schedule(triCount, 64, OceanManager.Instance.heightMapHandle);

		if(calculateTorques)
		{
			stjob = new SumTorqueJob()
			{
                buoyancyResults = m_results,
				torqueResults = m_torqueResults,
			};
			handle = stjob.Schedule(triCount, 32, handle);
		}
		
	}

	void LateUpdate()
	{
		handle.Complete();
        
	}

	void FixedUpdate()
	{
        //		Debug.Log("in FixedUpdate!");
        m_results.CopyTo(managedResults);

		if(calculateTorques)
		{
            m_torqueResults.CopyTo(managedtorqueResults);
		}
    }


	

    struct CalculateBuoyancyJob : IJobParallelFor
    {
		[ReadOnly]
		public NativeArray<Vector3> vertices;
		[ReadOnly]
		public NativeArray<int> triangles;

		[ReadOnly]
		public Matrix4x4 localToWorldMatrix;

		[ReadOnly]
		public float rho;
		[ReadOnly]
		public float gravity;

		public NativeArray<BuoyancyResult> results;


		// structs for ocean height map
		[ReadOnly]
		public HeightMapDescriptor descriptor;
		[ReadOnly]
		public NativeArray<float> heightMap;
		
        public void Execute(int index)
        {
			// if(index % 3 != 0)  return;
			
			
			Vector3 p1 = vertices[triangles[3 * index]];
			Vector3 p2 = vertices[triangles[3 * index + 1]];
			Vector3 p3 = vertices[triangles[3 * index + 2]];

            // local space -> world space
            Vector4 p14 = p1; p14.w = 1f;
            Vector4 p24 = p2; p24.w = 1f;
            Vector4 p34 = p3; p34.w = 1f;

			p1 = localToWorldMatrix * p14;
			p2 = localToWorldMatrix * p24;
			p3 = localToWorldMatrix * p34;

			// extract height, assumes water is constant at the plane y = 0
			// TODO: add a method to extract real depth when the water is moving
			float h1 = GetHeight(p1);
			float h2 = GetHeight(p2);
			float h3 = GetHeight(p3);

			int underCount = 0;
			if(h1 <= 0) underCount++;
			if(h2 <= 0) underCount++;
			if(h3 <= 0) underCount++;

            var result = results[index];

			// Debug.Log(underCount);

			switch(underCount)
			{
				case 0:
                    result.force0 = Vector3.zero;
					result.force1 = Vector3.zero;
					break;
				case 3:
				{
					// F = -pho * g * hcenter * n
					FillResult0(ref p1, ref p2, ref p3, ref result);
					result.force1 = Vector3.zero;
					break;
				}
				case 2:
				{
					// we have only one point above the water, find it
					Vector3 H, M, L;
					float hH, hM, hL;
					if(h1 > 0)
					{
                        H = p1; L = p2; M = p3;
						hH = h1; hL = h2; hM = h3;
					}  
					else if(h2 > 0) 
					{
						H = p2; L = p3; M = p1;
                        hH = h2; hL = h3; hM = h1;
					}
					else
					{
						H = p3; L = p1; M = p2;
                        hH = h3; hL = h1; hM = h2;
					}

					float tM = -hM / (hH - hM);
					Vector3 MIm = tM * (H - M);
					Vector3 Im = M + MIm;

					float tL = -hL / (hH - hL);
					Vector3 LIl = tL * (H - L);
					Vector3 Il = L + LIl;

					FillResult0(ref M, ref Im, ref Il, ref result);
					FillResult1(ref M, ref Il, ref L, ref result);

					break;
				}
				case 1:
				{
					// we have two points above the water
					Vector3 H, M, L;
					float hH, hM, hL;
					if(h1 <= 0)
					{
						L = p1; H = p2; M = p3;
                        hL = h1; hH = h2; hM = h3;
					}
					else if(h2 <= 0)
					{
						L = p2; H = p3; M = p1;
						hL = h2; hH = h3; hM = h1;
					}
					else
					{
						L = p3; H = p1; M = p2;
						hL = h3; hH = h1; hM = h2;
					}

					float tM = -hL / (hM - hL);
					Vector3 LJm = tM * (M - L);
					Vector3 Jm = L + LJm;

					float tH = -hL / (hH - hL);
					Vector3 LJh = tH * (H - L);
					Vector3 Jh = L + LJh;

					FillResult0(ref L, ref Jh, ref Jm, ref result);
					result.force1 = Vector3.zero;

					break;
				}
			}


			results[index] = result;
        }


		// assume the height map is placed at (0, 0, 0)
		// and the height map is repeated
		float GetHeight(Vector3 worldPos)
		{
			float scaledX = worldPos.x / descriptor.realSize;
			float scaledZ = worldPos.z / descriptor.realSize;

			float u = scaledX + 0.5f;
			float v = scaledZ + 0.5f;

			u = u - Mathf.Floor(u);
			v = v - Mathf.Floor(v);
			
			// uv space to height map space
			int col = (int)Mathf.Round(u * (descriptor.pixelSize - 1));
			int row_from_bottom = (int)Mathf.Round(v * (descriptor.pixelSize - 1));

			int row = (descriptor.pixelSize - 1) - row_from_bottom;

			float y =  heightMap[row * descriptor.pixelSize + col];
			// Debug.Log(y);


			return worldPos.y - y;
		}

		void FillResult0(ref Vector3 p1, ref Vector3 p2, ref Vector3 p3, ref BuoyancyResult result)
		{
            Vector3 center = (1f / 3f) * (p1 + p2 + p3);
            Vector3 normal = Vector3.Cross(p2 - p1, p3 - p1);

			float area = normal.magnitude / 2f;
            normal.Normalize();

            result.origin0 = center;
            result.force0 = -rho * gravity * -GetHeight(center) * area * normal;
			// Debug.Log(string.Format("area={0}", area));
		}

        void FillResult1(ref Vector3 p1, ref Vector3 p2, ref Vector3 p3, ref BuoyancyResult result)
        {
            Vector3 center = (1f / 3f) * (p1 + p2 + p3);
            Vector3 normal = Vector3.Cross(p2 - p1, p3 - p1);

            float area = normal.magnitude / 2f;
            normal.Normalize();

            result.origin1 = center;
            result.force1 = -rho * gravity * -GetHeight(center) * area * normal;
        }


    }

	struct SumTorqueJob : IJobParallelFor
	{
		[ReadOnly]
        public NativeArray<BuoyancyResult> buoyancyResults;
		[WriteOnly]
		public NativeArray<Vector2> torqueResults;

		public void Execute(int index)
		{
			Vector2 torqueSum = Vector2.zero;

            // cancel out horizontal forces
			float force0Y = buoyancyResults[index].force0.y;
			torqueSum.x -= force0Y * buoyancyResults[index].origin0.z;
			torqueSum.y += force0Y * buoyancyResults[index].origin0.x;

            float force1Y = buoyancyResults[index].force1.y;
            torqueSum.x -= force1Y * buoyancyResults[index].origin1.z;
            torqueSum.y += force1Y * buoyancyResults[index].origin1.x;

            // Vector3 finalForce0 = buoyancyResults[index].force0;
			// finalForce0.x = finalForce0.z = 0f;

            // torqueSum += Vector3.Cross(buoyancyResults[index].origin0, finalForce0);

            
        	// // cancel out horizontal forces
            // Vector3 finalForce1 = buoyancyResults[index].force1;
            // finalForce1.x = finalForce1.z = 0f;

            // torqueSum += Vector3.Cross(buoyancyResults[index].origin1, finalForce1);

			torqueResults[index] = torqueSum;
		}
	} 

	private void OnDisable()
	{

	}

    private void OnDestroy()
    {
        m_results.Dispose();
		m_vertices.Dispose();
		m_triangles.Dispose();

		if(calculateTorques)
			m_torqueResults.Dispose();
    }
}
