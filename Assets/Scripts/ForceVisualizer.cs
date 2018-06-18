using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

struct ForceOriginPair
{
	public Vector3 force;
	public Vector3 origin;

	public ForceOriginPair(Vector3 _force, Vector3 _origin)
	{
		force = _force;
		origin = _origin;
	}
} 

public class ForceVisualizer : MonoBehaviour {
    public float forceLengthMultiplier = 0.001f;
	public Material lineMaterial;
	public float lineWidth = 0.1f;

	List<LineRenderer> lineRenderers;

	// Use this for initialization
	void Start () {
		InitForceVisualization();
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    void InitForceVisualization()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        int triCount = mesh.triangles.GetLength(0) / 3;

        lineRenderers = new List<LineRenderer>( 2 * triCount);

		for(int i = 0; i < 2 * triCount; ++i)
		{
            GameObject lineGO = new GameObject("Force Line " + i.ToString());
            lineGO.transform.parent = transform;

            LineRenderer lr = lineGO.AddComponent<LineRenderer>();
            lr.material = lineMaterial;
            lr.widthMultiplier = lineWidth;
            lr.shadowCastingMode = ShadowCastingMode.Off;
            lr.receiveShadows = false;

			lr.positionCount = 2;

			lr.enabled = false;
			lineRenderers.Add(lr);
		}
    }

    void VisualizeForce(List<ForceOriginPair> forceOriginPairs)
    {
		if(!this.enabled)	return;

        for(int i = 0; i < forceOriginPairs.Count; ++i)
		{
			LineRenderer lr = lineRenderers[i];
			lr.SetPosition(0, forceOriginPairs[i].origin);
            lr.SetPosition(1, forceOriginPairs[i].origin + 
				forceOriginPairs[i].force * forceLengthMultiplier);
			
			lr.enabled = true;
		}

		for(int i = forceOriginPairs.Count; i < lineRenderers.Count; ++i)
		{
			lineRenderers[i].enabled = false;
		}
    }

}
