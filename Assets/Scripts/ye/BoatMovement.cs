using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatMovement : MonoBehaviour {

	public float speed = 0;

	public List<GameObject> target;

	Vector3 targetPos;

	public bool moveDone = false;

	// Use this for initialization
	void Start () {
		targetPos = target[0].transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if(moveDone == true){
			moveDone = false;
			Move(targetPos);
		}
	}

	void Move(Vector3 targetPos){
		StartCoroutine(Shift(targetPos));
	}

	IEnumerator Shift(Vector3 targetPos){
		float timeSinceStarted = 0f;
    	while (true)
    	{
        	timeSinceStarted += Time.deltaTime;
        	gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, targetPos, timeSinceStarted);

        	// If the object has arrived, stop the coroutine
        	if (gameObject.transform.position == targetPos)
        	{
				moveDone = true;
            	yield break;
        	}

        // Otherwise, continue next frame
        yield return null;
    }
	}

}
