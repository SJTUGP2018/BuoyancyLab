using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// for Unity Job System:
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.Jobs;

struct BuoyancyResult
{
	public Vector3 origin0;
    public Vector3 force0;
    public Vector3 origin1;
    public Vector3 force1;
}

public class BuoyancyParallelFor : MonoBehaviour {

	public const int USE_RESULTANT_LIMIT = 100;

	BuoyancyResult[] managedResults;
	NativeArray<BuoyancyResult> m_results;

    Mesh mesh;
	NativeArray<Vector3> m_vertices;
    NativeArray<int> m_triangles;
	int triCount;

	JobHandle handle;

    CalculateBuoyancyJob cbjob;

	Rigidbody rb;
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();

		mesh = GetComponent<MeshFilter>().mesh;
		triCount = mesh.triangles.GetLength(0) / 3;


		m_vertices = new NativeArray<Vector3>(mesh.vertices, Allocator.Persistent);
		m_triangles = new NativeArray<int>(mesh.triangles, Allocator.Persistent);
        m_results = new NativeArray<BuoyancyResult>(triCount, Allocator.Persistent);

		managedResults = new BuoyancyResult[triCount];
	}
	
	// Update is called once per frame
	void Update () {
		cbjob = new CalculateBuoyancyJob()
		{
			vertices = m_vertices,
			triangles = m_triangles,
			localToWorldMatrix = transform.localToWorldMatrix,
            results = m_results,
			rho = 1027f,
			gravity = 9.81f
		};

		handle = cbjob.Schedule(triCount, 64);
		
	}

	void LateUpdate()
	{
		handle.Complete();
	}

	void FixedUpdate()
	{
//		Debug.Log("in FixedUpdate!");
        m_results.CopyTo(managedResults);

		if(managedResults.Length <= USE_RESULTANT_LIMIT)
		{
            ApplySeparateForces();
		}
		else
		{
			ApplyResultantForce();
		}
	}

	void ApplySeparateForces()
	{
        foreach (var result in managedResults)
        {
            if (result.force0 != Vector3.zero)
            {
                // cancel out horizontal forces
                Vector3 finalForce0 = new Vector3(0f, result.force0.y, 0f);
                rb.AddForceAtPosition(finalForce0, result.origin0);
                // Debug.DrawLine(result.origin0, result.origin0 + result.force0);
            }
            if (result.force1 != Vector3.zero)
            {
                Vector3 finalForce1 = new Vector3(0f, result.force1.y, 0f);
                rb.AddForceAtPosition(finalForce1, result.origin1);
                // Debug.DrawLine(result.origin1, result.origin1 + result.force1);
            }
        }
	}

	void ApplyResultantForce()
	{
        Vector3 resultantForce = Vector3.zero;
        Vector3 torqueSum = Vector3.zero;

        foreach (var result in managedResults)
        {
            if (result.force0 != Vector3.zero)
            {
                // cancel out horizontal forces
                Vector3 finalForce0 = new Vector3(0f, result.force0.y, 0f);

                resultantForce += finalForce0;
                torqueSum += Vector3.Cross(result.origin0, finalForce0);

                //rb.AddForceAtPosition(finalForce0, result.origin0);
                // Debug.DrawLine(result.origin0, result.origin0 + result.force0);
            }
            if (result.force1 != Vector3.zero)
            {
                Vector3 finalForce1 = new Vector3(0f, result.force1.y, 0f);

                resultantForce += finalForce1;
                torqueSum += Vector3.Cross(result.origin1, finalForce1);
                //rb.AddForceAtPosition(finalForce1, result.origin1);
                // Debug.DrawLine(result.origin1, result.origin1 + result.force1);
            }
        }

        // R * F = sigma(Ri * Fi)
        if (resultantForce == Vector3.zero)
        {
            // resultant Force is zero!
            Debug.Log("resultant Force is zero: " + gameObject.name);
            // rb.AddTorque(torqueSum);
        }
        else
        {
            // ax(by-bx) + ay(bz-bx) + az(bz-by) = cx + cy + cz
            // Dot(a, bp) = s
            // bp = (by-bz, bz-bx, bx-by)
            // a = (ax, ay, az)
            // s = - (cx + cy + cz)  (right-handed -> left-handed)

            // Dot(a, n) = d
            // d = s / |x|
            // n = bp / |bp|
            Vector3 resultantOrigin = rb.worldCenterOfMass;
            Vector3 f = resultantForce;
            Vector3 c = torqueSum;
            Vector3 bp = new Vector3(f.y - f.z, f.z - f.x, f.x - f.y);
            if (bp != Vector3.zero)
            {
                float s = (c.x + c.y + c.z);
                float d = s / bp.magnitude;
                Vector3 n = bp.normalized;

                // P -> point on plane
                Vector3 P = n * d;

                Vector3 C = rb.worldCenterOfMass;
                Vector3 CP = P - C;
                float dist = -Vector3.Dot(CP, n);
                Vector3 O = C + (-n) * dist;

                // Debug.Log(string.Format("{0} == {1}", Vector3.Dot(O, bp), s));
                // Debug.Log(string.Format("{0} == {1}", Vector3.Cross(O, f), c));


                // resultantOrigin = P;
                resultantOrigin = O;

                //                Debug.Log(string.Format("{0} == {1}", Vector3.Cross(resultantOrigin, f), c));
            }
            else
            {
                Debug.Log("bp == Vector3.zero, c is " + torqueSum.ToString());
            }

            rb.AddForceAtPosition(resultantForce, resultantOrigin);
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

		float GetHeight(Vector3 worldPos)
		{
			return worldPos.y;
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

    private void OnDestroy()
    {
        m_results.Dispose();
		m_vertices.Dispose();
		m_triangles.Dispose();
    }
}
