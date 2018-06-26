using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuoyancyVisualizer : MonoBehaviour {
	/// Assume using unit cylinder as body.
	public GameObject arrowBody;
    public float gravityReferenceLength = 1f;
	public float diameterFactor = 0.1f;

    List<BuoyancyCalculator> calculators;
    List<BuoyancyResult[]> buoyancyResultArrays;

	public Transform[] arrowBodies;

    Rigidbody rb;

	int totalTriCount = 0;

	bool initialized = false;


	float rbGravity;
	

	public void InitVisualizer(List<BuoyancyCalculator> _calculators, 
							   List<BuoyancyResult[]> _buoyancyResultArrays,
							   Rigidbody _rb)
	{
		calculators = _calculators;
		buoyancyResultArrays = _buoyancyResultArrays;
		rb = _rb;

        rbGravity = (rb.mass * Physics.gravity.magnitude);

		totalTriCount = 0;
		foreach(BuoyancyCalculator cal in calculators)
		{
			totalTriCount += cal.triangleCount;
		}

		arrowBodies = new Transform[2 * totalTriCount];

		// create force arrow prefabs
		for(int i = 0; i < 2 * totalTriCount; ++i)
		{
			GameObject arrowBodyGO = Instantiate(arrowBody, transform.position, 
				Quaternion.identity);
			arrowBodyGO.SetActive(false);
			arrowBodies[i] = arrowBodyGO.transform;
		}


		initialized = true;
	}

	
	// Update is called once per frame
	void Update () {
		if(!initialized) 
			return;

		int curArrowIndex = 0;

		for(int i = 0; i < buoyancyResultArrays.Count; ++i)
		{	
			var resultArray = buoyancyResultArrays[i];
			for(int j = 0; j < resultArray.GetLength(0); ++j)
			{
				if(resultArray[j].force0 != Vector3.zero)
					EnableArrow(curArrowIndex, resultArray[j].origin0, resultArray[j].force0);
				else
					DisableArrow(curArrowIndex);

				curArrowIndex++;

                if (resultArray[j].force1 != Vector3.zero)
                    EnableArrow(curArrowIndex, resultArray[j].origin1, resultArray[j].force1);
                else
                    DisableArrow(curArrowIndex);

                curArrowIndex++;
			}
		}

	}

	void EnableArrow(int index, Vector3 origin, Vector3 force)
	{
		Transform arrowBody = arrowBodies[index];

		Vector3 forceDir = force.normalized;
		float forceLen = force.magnitude / rbGravity * gravityReferenceLength;

		arrowBody.position = origin;
		arrowBody.localScale = new Vector3(
			diameterFactor, 
			force.magnitude / rbGravity * gravityReferenceLength,
			diameterFactor
		);

		arrowBody.up = -force.normalized;

		arrowBody.gameObject.SetActive(true);


	}

	void DisableArrow(int index)
	{
        Transform arrowBody = arrowBodies[index];
		arrowBody.gameObject.SetActive(false);
	}
}
