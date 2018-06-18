using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SimpleForceVisualizer : MonoBehaviour {

	Rigidbody rb;
	Vector3 lastVelocity;

	List<LineRenderer> lineRenderers;

    public float forceLengthMultiplier = 0.001f;
    public Material lineMaterial;
    public float lineWidth = 0.1f;

	void Start()
	{
		rb = GetComponent<Rigidbody>();
        InitForceVisualization();
	}

    void InitForceVisualization()
    {
        lineRenderers = new List<LineRenderer>(2);

        for (int i = 0; i < 2; ++i)
        {
            GameObject lineGO = new GameObject("Force Line " + i.ToString());
            lineGO.transform.parent = transform;

            LineRenderer lr = lineGO.AddComponent<LineRenderer>();
            lr.material = lineMaterial;
            lr.widthMultiplier = lineWidth;
            lr.shadowCastingMode = ShadowCastingMode.Off;
            lr.receiveShadows = false;

            lr.positionCount = 2;

            // lr.enabled = false;
            lineRenderers.Add(lr);
        }
    }

	void FixedUpdate()
	{
		Vector3 deltaVelo = rb.velocity - lastVelocity;
		Vector3 resultantForce = deltaVelo * rb.mass / Time.fixedDeltaTime;

		// Debug.DrawLine(transform.position, transform.position + resultantForce);
		// visualize gravity
		LineRenderer gravityLr = lineRenderers[0];
		gravityLr.startColor = Color.red;
		gravityLr.endColor = Color.red;

		Vector3 centerOfMass = transform.TransformVector(rb.centerOfMass) + transform.position;
		Vector3 gravityForce = Physics.gravity * rb.mass;

		gravityLr.SetPosition(0, centerOfMass);
		gravityLr.SetPosition(1, centerOfMass + gravityForce * forceLengthMultiplier);

		// visualize buoyancy
		Vector3 buoyancy = resultantForce - gravityForce;

		LineRenderer buoyancyLr = lineRenderers[1];
        buoyancyLr.SetPosition(0, centerOfMass);
        buoyancyLr.SetPosition(1, centerOfMass + buoyancy * forceLengthMultiplier);


		lastVelocity = rb.velocity;
	}

}
