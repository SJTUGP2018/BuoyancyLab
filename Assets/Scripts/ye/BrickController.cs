using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DetectResult{
	public Vector3Int point;

	public bool detectable;
}

public class BrickController : MonoBehaviour {

	public LayerMask layerMask = 1 << 8; // Brick

	public List<GameObject> brickList;

	public int index = 0;

	[HideInInspector]
	public GameObject selectedBrick;

	GameObject testBrick;

	public Transform boatTrans;

	public float maxDistance = 10;

	public Material testMat;

	public Material testMatOk;

	public Material testMatFail;

	Vector3Int setPoint = new Vector3Int(999, 999, 999);

	BrickBehavior bh;

	DetectResult dr;

	MeshRenderer testMR;

	Vector3 pos;

	Vector3 lastPos;

	SteamVR_TrackedObject trackedObj;
	public LineRenderer lr;

	void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

	// Use this for initialization
	void Start () {
		selectedBrick = brickList[0];
		InstantiateTest();
		
		//testBrick = Instantiate(selectedBrick);
		//testBrick.GetComponent<Collider>().enabled = false;
		
	}
	
	// Update is called once per frame
	void Update () {
		dr = BrickDetect(transform);

		lr.SetPosition(0, transform.position);
		lr.SetPosition(1, transform.position + transform.forward * maxDistance);

		SteamVR_Controller.Device device = SteamVR_Controller.Input((int)trackedObj.index);

		dr = BrickDetect(transform);

		if (device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
        {
			Vector2 pad = device.GetAxis();
            Vector3 cross = Vector3.Cross(new Vector2(1, 0), pad);
            float angle = Vector2.Angle(new Vector2(1, 0), pad);
            float ang = cross.z > 0 ? -angle : angle;
            Debug.Log(ang);
            if (ang > 45 && ang < 135)
            {
				Debug.Log("down");
				ChangeBrickDirection(ChangeType.pitchDown);
                //obj.transform.Translate(Vector3.back * Time.deltaTime * 5);
            }
            else if (ang < -45 && ang> -135)
            {
				Debug.Log("up");
				ChangeBrickDirection(ChangeType.pitchUp);
                //obj.transform.Translate(Vector3.forward * Time.deltaTime * 5);
            }
            else if ((ang < 180 && ang> 135) || (ang < -135 && ang > -180))
            {
				Debug.Log("left");
				ChangeBrickDirection(ChangeType.yawUp);
                //obj.transform.Rotate(Vector3.up * Time.deltaTime * -25);
            }
            else if ((ang > 0 && ang< 45) || (ang > -45 && ang< 0)){
				Debug.Log("right");
				ChangeBrickDirection(ChangeType.yawDown);
		    }
		}

		if(device.GetPressDown(SteamVR_Controller.ButtonMask.Grip)){
			ChangeBrickType();
			InstantiateTest();
		}

		if(device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)){
			TryAndSetBrick();
		}

		if(device.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu)){
			DeleteDetect(transform);
		}

		if(dr.detectable == true){		
			testBrick.transform.position = dr.point;
		}
		else{
			testBrick.transform.position = transform.position + transform.forward * maxDistance;
		}

		if(lastPos != testBrick.transform.position){
			testMR.material = testMat;
		}

		if(Input.GetKeyDown(KeyCode.Space)){
			Debug.Log("here");
			TryAndSetBrick();
		}

		if(Input.GetKeyDown(KeyCode.Z)){
			
			ChangeBrickType();
			InstantiateTest();
		}

		if(Input.GetKeyDown(KeyCode.J)){
			ChangeBrickDirection(ChangeType.yawUp);
		}
		else if(Input.GetKeyDown(KeyCode.L)){
			ChangeBrickDirection(ChangeType.yawDown);
		}
		else if(Input.GetKeyDown(KeyCode.I)){
			ChangeBrickDirection(ChangeType.pitchUp);
		}
		else if(Input.GetKeyDown(KeyCode.K)){
			ChangeBrickDirection(ChangeType.pitchDown);
		}
		

		lastPos = testBrick.transform.position;
	}

	void InstantiateTest(){
		if(testBrick){
			Destroy(testBrick);
		}
		else{
			
		}
		testBrick = Instantiate(selectedBrick);
		testBrick.GetComponent<MeshRenderer>().material = testMat;
		testBrick.GetComponent<Collider>().enabled = false;
		testBrick.transform.position = transform.position + transform.forward * maxDistance;
		bh = testBrick.GetComponent<BrickBehavior>();
		testMR = testBrick.GetComponent<MeshRenderer>();
	}

	void TryAndSetBrick(){
		if(bh){
			if(bh.CheckSettable() && dr.detectable == true){
				testMR.material = testMatOk;
				GameObject newBrick = Instantiate(selectedBrick, dr.point, testBrick.transform.rotation, boatTrans);
				
			}
			else{
				testMR.material = testMatFail;
				//testBrick.GetComponent<MeshRenderer>().material = testMat;
			}
		}
		//testMR.material = testMat;
	}


	void ChangeBrickType(){
		if(index < brickList.Count -1){
			index ++;
		}
		else{
			index = 0;
		}
		selectedBrick = brickList[index];
	}

	void ChangeBrickDirection(ChangeType ct){
		if(bh){
			bh.RotateBrick(ct);
		}
	}


	DetectResult BrickDetect(Transform trans){
		DetectResult detectResult = new DetectResult();
		RaycastHit hit;
		if(Physics.Raycast(trans.position, trans.forward, out hit, Mathf.Infinity, layerMask)){
			//Debug.Log("point: " + hit.point);
			//Debug.Log("normal" + hit.normal);

			if(hit.normal.x != (int)(hit.normal.x) || hit.normal.y != (int)(hit.normal.y) || hit.normal.z != (int)(hit.normal.z)){
				detectResult.point = new Vector3Int(0, 0, 0);
				detectResult.detectable = false;
				return detectResult;
			}
			else{
				setPoint = Hitpoint2Grid(hit.point, hit.normal);
				Debug.DrawLine(transform.position, setPoint, Color.red);
				detectResult.point = setPoint;
				detectResult.detectable = true;
				return detectResult;
			}
			
			//return true;
		}
		else{
			detectResult.point = new Vector3Int(0, 0, 0);
			detectResult.detectable = false;
			return detectResult;
			//return false;
		}
	}

	void DeleteDetect(Transform trans){
		RaycastHit hit;
		if(Physics.Raycast(trans.position, trans.forward, out hit, Mathf.Infinity, layerMask)){
			Debug.Log("point: " + hit.point);
			Debug.Log("normal" + hit.normal);

			Destroy(hit.collider.gameObject);
			//return true;
		}
		else{
		}
	}

	Vector3Int Hitpoint2Grid(Vector3 point, Vector3 direction){	
		Vector3 newPoint = point + direction;
		newPoint = (newPoint + point) / 2.0f;
		Vector3Int result = new Vector3Int(Mathf.RoundToInt(newPoint.x),Mathf.RoundToInt(newPoint.y),Mathf.RoundToInt(newPoint.z));
		return result;
	}
}
