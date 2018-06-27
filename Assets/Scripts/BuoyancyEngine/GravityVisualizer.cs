using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityVisualizer : MonoBehaviour {

	public GameObject arrowBodyPrefab;
	public GameObject arrowHeadPrefab;

	public float length = 1f;
    public float diameterFactor = 0.1f;
    public float arrowHeadFactor = 0.2f;
    public Vector3 arrowHeadComponentFactor =
        (Vector3.up + Vector3.forward + Vector3.right);

	Vector3 arrowHeadScale;

	Rigidbody rb;

//	Vector3 gravityTargetPoint;

	Transform arrowBodyTrans;
	Transform arrowHeadTrans;


	void OnEnable () 
	{
		rb = GetComponent<Rigidbody>();
//		gravityTargetPoint = rb.worldCenterOfMass;


		GameObject arrowBodyGO = Instantiate(arrowBodyPrefab, transform.position, 
			Quaternion.identity, transform);
		arrowBodyTrans = arrowBodyGO.transform;

        Vector3 arrowHeadScaleVec = new Vector3(arrowHeadFactor, arrowHeadFactor, arrowHeadFactor);
        arrowHeadScaleVec = Vector3.Scale(arrowHeadScaleVec, arrowHeadComponentFactor);

        GameObject arrowHeadGO = Instantiate(arrowHeadPrefab, transform.position,
            Quaternion.identity, transform);
        arrowHeadTrans = arrowHeadGO.transform;
        arrowHeadTrans.localScale = Vector3.Scale(arrowHeadTrans.localScale,
                arrowHeadScaleVec);
		
	}

	void Update()
	{
        
        Vector3 gravityStartPoint = rb.worldCenterOfMass;
		arrowBodyTrans.position = gravityStartPoint;
		arrowBodyTrans.localScale = new Vector3(
            diameterFactor,
            length,
            diameterFactor
		);
		arrowBodyTrans.up = - Vector3.up;

		arrowHeadTrans.position = gravityStartPoint - Vector3.up * 
			(length + arrowHeadTrans.localScale.y);
		arrowHeadTrans.up = - Vector3.up;
		

	}

	void OnDisable()
	{
		if(arrowHeadTrans != null) Destroy(arrowHeadTrans.gameObject);
		if(arrowBodyTrans != null) Destroy(arrowBodyTrans.gameObject);
	}


}
