using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PropPanelVisualizer : MonoBehaviour {

	public GameObject propPanelPrefab;
	public float offsetFactor = 0.6f;

	Rigidbody rb;
	float offsetLength = 1f;

	PropPanelManager manager;
	GameObject propPanelGO;
	Transform propPanel;

	float volume;
	float density;
	Vector3 velocity;
	Vector3 angularVelocity;

	BuoyancyApplier applier;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
		applier = GetComponent<BuoyancyApplier>();

        var rbBounds = new Bounds();
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider coll in colliders)
        {
            rbBounds.Encapsulate(coll.bounds);
        }

		float maxLength = 0f;
		maxLength = Mathf.Max(rbBounds.extents.x, rbBounds.extents.y, rbBounds.extents.z);
		Debug.Log(maxLength);
		offsetLength = maxLength;

		propPanelGO = Instantiate(propPanelPrefab, transform.position, transform.rotation, transform);
        // propPanelGO.SetActive(false);
		propPanel = propPanelGO.transform;
		manager = propPanelGO.GetComponent<PropPanelManager>();

        CalculateVolumeAndDensity();
	}


	void CalculateVolumeAndDensity()
	{
        // very dirty way to calculate volume
        rb = GetComponent<Rigidbody>();
        float oldMass = rb.mass;
        rb.SetDensity(1f);
        volume = rb.mass;
		rb.mass = oldMass;

		density = rb.mass / volume;
	}
	

	void OnEnable()
	{
        CalculateVolumeAndDensity();
		if(propPanelGO != null)
        	propPanelGO.SetActive(true);
	}

	void OnDisable()
	{
        if (propPanelGO != null)
            propPanelGO.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () 
	{
        // TODO: align panel with view
		propPanel.position = rb.worldCenterOfMass + Vector3.up * offsetLength * offsetFactor;

		if(Vector3.Distance(propPanel.position, Camera.main.transform.position) > 1f)
		{
            propPanel.LookAt(Camera.main.transform);
            propPanel.forward = -propPanel.forward;
		}
		

		// set properties
        manager.massText.text = string.Format("{0:F2}", rb.mass);
		manager.volumeText.text = string.Format("{0:F2}", volume);
		manager.densityText.text = string.Format("{0:F2}", density);

        manager.velocityText.text = rb.velocity.ToString();
        manager.angularVelocityText.text = rb.angularVelocity.ToString();

		if(applier)
		{
			float waterDisplacementVolume = applier.resultantForceY / OceanManager.Instance.oceanDensity
					/ 9.81f;
            manager.waterDisplacementText.text = string.Format("{0:F2}", waterDisplacementVolume);
		}
	}
}
