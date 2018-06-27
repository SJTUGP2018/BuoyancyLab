using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BuoyancyApplier : MonoBehaviour {

    public int USE_RESULTANT_LIMIT = 100;

	List<BuoyancyCalculator> calculators;
	List<BuoyancyResult[]> buoyancyResultArrays;
    bool[] hasTorqueSumArray;

	Rigidbody rb;



	bool initialied = false;
	// Use this for initialization
	void OnEnable () {
		initialied = false;
		rb = GetComponent<Rigidbody>();
		StartCoroutine(InitApplierIE());
	}

	IEnumerator InitApplierIE()
	{
        BuoyancyCalculator[] calArray = GetComponentsInChildren<BuoyancyCalculator>();
		calculators = new List<BuoyancyCalculator>(calArray);
		// wait for each calculator to init
		yield return new WaitForEndOfFrame();

        hasTorqueSumArray = new bool[calculators.Count];
		buoyancyResultArrays = new List<BuoyancyResult[]>(calculators.Count);
		// calculators are initialized
		for(int i = 0; i < calculators.Count; ++i)
		{
            hasTorqueSumArray[i] = calculators[i].calculateTorques;
			buoyancyResultArrays.Add(calculators[i].resultArray);
		}
        initialied = true;

        BuoyancyVisualizer visualizer = GetComponent<BuoyancyVisualizer>();
        if(visualizer)
        {
            visualizer.InitVisualizer(calculators, buoyancyResultArrays, rb);
        }
	}

	void FixedUpdate()
	{
		if(!initialied) return;

		// foreach(BuoyancyResult[] resultArray in buoyancyResultArrays)
		// {
		// 	if(resultArray.GetLength(0) <= USE_RESULTANT_LIMIT)
		// 	{
        //         ApplySeparateForces(resultArray);
		// 	}
		// 	else
		// 	{
		// 		ApplyResultantForce(resultArray);
		// 	}
		// }
        for(int i = 0; i < buoyancyResultArrays.Count; ++i)
        {
            if(!hasTorqueSumArray[i])
            {   
                if(buoyancyResultArrays[i].GetLength(0) < USE_RESULTANT_LIMIT)
                    ApplySeparateForces(buoyancyResultArrays[i]);
                else
                    ApplyResultantForce(buoyancyResultArrays[i]);
            }
            else
            {
                ApplyResultantForceWithTorques(
                    buoyancyResultArrays[i], 
                    calculators[i].torqueResultArray
                );
            }
        }


	}

    void ApplySeparateForces(BuoyancyResult[] managedResults)
    {
        foreach (var result in managedResults)
        {
            if (result.force0 != Vector3.zero)
            {
                // cancel out horizontal forces
                Vector3 finalForce0 = new Vector3(0f, result.force0.y, 0f);
                rb.AddForceAtPosition(finalForce0, result.origin0);
                //Debug.DrawLine(result.origin0, result.origin0 + result.force0);
            }
            if (result.force1 != Vector3.zero)
            {
                Vector3 finalForce1 = new Vector3(0f, result.force1.y, 0f);
                rb.AddForceAtPosition(finalForce1, result.origin1);
                //Debug.DrawLine(result.origin1, result.origin1 + result.force1);
            }
        }
    }

    void ApplyResultantForce(BuoyancyResult[] managedResults)
    {
        float resultantForceY = 0f;
        //Vector3 torqueSum = Vector3.zero;
        Vector2 torqueSumXZ = Vector2.zero;

        foreach (var result in managedResults)
        {
            if (result.force0 != Vector3.zero)
            {
                // cancel out horizontal forces
                // Vector3 finalForce0 = new Vector3(0f, result.force0.y, 0f);

                resultantForceY += result.force0.y;
                // torqueSum += Vector3.Cross(result.origin0, finalForce0);

                float force0Y = result.force0.y;
                torqueSumXZ.x -= force0Y * result.origin0.z;
                torqueSumXZ.y += force0Y * result.origin0.x;

            }
            if (result.force1 != Vector3.zero)
            {
                // Vector3 finalForce1 = new Vector3(0f, result.force1.y, 0f);

                resultantForceY += result.force1.y;
                // torqueSum += Vector3.Cross(result.origin1, finalForce1);


                float force1Y = result.force1.y;
                torqueSumXZ.x -= force1Y * result.origin1.z;
                torqueSumXZ.y += force1Y * result.origin1.x;
            }
        }
        Vector3 resultantForce = new Vector3(0f, resultantForceY, 0f);
        Vector3 torqueSum = new Vector3(torqueSumXZ.x, 0f, torqueSumXZ.y);
        // R * F = sigma(Ri * Fi)
        if (resultantForce == Vector3.zero)
        {
            // resultant Force is zero!
            Debug.Log("resultant Force is zero: " + gameObject.name);
            rb.AddTorque(torqueSum);
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

        void ApplyResultantForceWithTorques(BuoyancyResult[] managedResults, Vector2[] torqueResults)
    {
        float resultantForceY = 0f;
        Vector2 torqueSumXZ = Vector3.zero;

        for(int i = 0; i < managedResults.Length; ++i)
        {
            resultantForceY += managedResults[i].force0.y;
            resultantForceY += managedResults[i].force1.y;

            torqueSumXZ += torqueResults[i];
        }

        Vector3 torqueSum = new Vector3(torqueSumXZ.x, 0f, torqueSumXZ.y);

        // foreach (var result in managedResults)
        // {
        //     if (result.force0 != Vector3.zero)
        //     {
        //         // cancel out horizontal forces
        //         Vector3 finalForce0 = new Vector3(0f, result.force0.y, 0f);

        //         resultantForce += finalForce0;
        //         torqueSum += Vector3.Cross(result.origin0, finalForce0);

        //         //rb.AddForceAtPosition(finalForce0, result.origin0);
        //         // Debug.DrawLine(result.origin0, result.origin0 + result.force0);
        //     }
        //     if (result.force1 != Vector3.zero)
        //     {
        //         Vector3 finalForce1 = new Vector3(0f, result.force1.y, 0f);

        //         resultantForce += finalForce1;
        //         torqueSum += Vector3.Cross(result.origin1, finalForce1);
        //         //rb.AddForceAtPosition(finalForce1, result.origin1);
        //         // Debug.DrawLine(result.origin1, result.origin1 + result.force1);
        //     }
        // }

        // R * F = sigma(Ri * Fi)
        if (Mathf.Approximately(0f, resultantForceY))
        {
            // resultant Force is zero!
            Debug.Log("resultant Force is zero: " + gameObject.name);
            rb.AddTorque(torqueSum);
        }
        else
        {
            Vector3 resultantForce = new Vector3(0f, resultantForceY, 0f);
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


}
