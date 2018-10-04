using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SimpleForceVisualizerNew : MonoBehaviour {

	Rigidbody rb;
	Vector3 lastVelocity;

	//List<LineRenderer> lineRenderers;

	List<GameObject> arrows;

	public GameObject arrowPrefab;

    public float forceLengthMultiplier = 0.001f;
    //public Material lineMaterial;
    public float lineWidth = 0.1f;

	void Start()
	{
		rb = GetComponent<Rigidbody>();
        InitForceVisualization();
	}

    void InitForceVisualization()
    {
        //lineRenderers = new List<LineRenderer>(2);
		arrows = new List<GameObject>(2);

        for (int i = 0; i < 2; ++i)
        {
            //GameObject arrow = new GameObject("Arrow " + i.ToString());

			GameObject arrow = Instantiate(arrowPrefab, gameObject.transform);
            // lineGO.transform.parent = transform;

            // LineRenderer lr = lineGO.AddComponent<LineRenderer>();
            // lr.material = lineMaterial;
            // lr.widthMultiplier = lineWidth;
            // lr.shadowCastingMode = ShadowCastingMode.Off;
            // lr.receiveShadows = false;

            // lr.positionCount = 2;

            // // lr.enabled = false;
            // lineRenderers.Add(lr);

			arrows.Add(arrow);
        }
		arrows[1].transform.localScale = new Vector3(1, -1.0f, 1);
    }

	void FixedUpdate()
	{
		Vector3 deltaVelo = rb.velocity - lastVelocity;
		Vector3 resultantForce = deltaVelo * rb.mass / Time.fixedDeltaTime;

		// Debug.DrawLine(transform.position, transform.position + resultantForce);
		// visualize gravity
		// LineRenderer gravityLr = lineRenderers[0];
		// gravityLr.startColor = Color.red;
		// gravityLr.endColor = Color.red;

		Vector3 centerOfMass = transform.TransformVector(rb.centerOfMass) + transform.position;
		Vector3 gravityForce = Physics.gravity * rb.mass;

		// gravityLr.SetPosition(0, centerOfMass);
		// gravityLr.SetPosition(1, centerOfMass + gravityForce * forceLengthMultiplier);


		GameObject gravityArrow = arrows[0];
		Debug.Log("gravityArrow: " + gravityForce * forceLengthMultiplier);
		gravityArrow.transform.position = centerOfMass;
		gravityArrow.transform.localScale = new Vector3(1, gravityForce.y * forceLengthMultiplier, 1);

		if(gravityArrow.activeInHierarchy == false){
			gravityArrow.SetActive(true);
		}



		// visualize buoyancy
		Vector3 buoyancy = resultantForce - gravityForce;

		// LineRenderer buoyancyLr = lineRenderers[1];
        // buoyancyLr.SetPosition(0, centerOfMass);
        // buoyancyLr.SetPosition(1, centerOfMass + buoyancy * forceLengthMultiplier);

		GameObject buoyancyArrow = arrows[1];
		Debug.Log("boyancyArrow: " + gravityForce * forceLengthMultiplier);
		buoyancyArrow.transform.position = centerOfMass;
		buoyancyArrow.transform.localScale = new Vector3(1, buoyancy.y * forceLengthMultiplier, 1);

		if(buoyancyArrow.activeInHierarchy == false){
			buoyancyArrow.SetActive(true);
		}


		lastVelocity = rb.velocity;
	}

}
