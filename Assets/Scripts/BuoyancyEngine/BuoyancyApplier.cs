using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BuoyancyApplier : MonoBehaviour {

    public int USE_RESULTANT_LIMIT = 100;

	List<BuoyancyCalculator> calculators;
	List<BuoyancyResult[]> buoyancyResultArrays;

	Rigidbody rb;


	bool initialied = false;
	// Use this for initialization
	void Start () {
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
		buoyancyResultArrays = new List<BuoyancyResult[]>(calculators.Count);
		// calculators are initialized
		for(int i = 0; i < calculators.Count; ++i)
		{
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

		foreach(BuoyancyResult[] resultArray in buoyancyResultArrays)
		{
			if(resultArray.GetLength(0) <= USE_RESULTANT_LIMIT)
			{
                ApplySeparateForces(resultArray);
			}
			else
			{
				ApplyResultantForce(resultArray);
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


}
