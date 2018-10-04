using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuoyancyVisualizer : MonoBehaviour {
	/// Assume using unit cylinder as body.
	public GameObject arrowBody;
	public GameObject arrowHead;

    public float gravityReferenceLength = 5f;
	public float diameterFactor = 0.1f;
	public float arrowHeadFactor = 0.2f;
	public Vector3 arrowHeadComponentFactor = 
		(Vector3.up + Vector3.forward + Vector3.right);

    List<BuoyancyCalculator> calculators;
    List<BuoyancyResult[]> buoyancyResultArrays;

	[HideInInspector]
	public Transform[] arrowBodies;
	[HideInInspector]
	public Transform[] arrowHeads;

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
		arrowHeads = new Transform[2 * totalTriCount];

		Vector3 arrowHeadScaleVec = new Vector3(arrowHeadFactor, arrowHeadFactor, arrowHeadFactor);
		arrowHeadScaleVec = Vector3.Scale(arrowHeadScaleVec, arrowHeadComponentFactor);

		// create force arrow prefabs
		for(int i = 0; i < 2 * totalTriCount; ++i)
		{
			GameObject arrowBodyGO = Instantiate(arrowBody, transform.position, 
				Quaternion.identity, transform);
			arrowBodyGO.SetActive(false);
			arrowBodies[i] = arrowBodyGO.transform;

            GameObject arrowHeadGO = Instantiate(arrowHead, transform.position,
                Quaternion.identity, transform);
            arrowHeadGO.SetActive(false);

			Transform arrowHeadTrans = arrowHeadGO.transform;
			arrowHeadTrans.localScale = Vector3.Scale(arrowHeadTrans.localScale,
                arrowHeadScaleVec);

            arrowHeads[i] = arrowHeadGO.transform;
			
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
//		float forceLen = force.magnitude / rbGravity * gravityReferenceLength;

		arrowBody.position = origin - forceDir * arrowHeadFactor;
		arrowBody.localScale = new Vector3(
			diameterFactor, 
			force.magnitude / rbGravity * gravityReferenceLength,
			diameterFactor
		);

		arrowBody.up = -force.normalized;

		arrowBody.gameObject.SetActive(true);

		Transform arrowHead = arrowHeads[index];
		arrowHead.position = origin;
		arrowHead.up = forceDir;
		arrowHead.gameObject.SetActive(true);

	}

	void DisableArrow(int index)
	{
        Transform arrowBody = arrowBodies[index];
		arrowBody.gameObject.SetActive(false);
		Transform arrowHead = arrowHeads[index];
		arrowHead.gameObject.SetActive(false);
	}


	void OnDisable()
	{
		if(arrowBodies != null)
		{
			foreach(Transform body in arrowBodies)
			{
				body.gameObject.SetActive(false);
			}
		}

        if (arrowHeads != null)
        {
            foreach (Transform head in arrowHeads)
            {
                head.gameObject.SetActive(false);
            }
        }
	}
	
}
