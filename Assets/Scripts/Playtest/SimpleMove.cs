using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMove : MonoBehaviour {

	public float speed = 20f;

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");
		Vector2 input = new Vector2 (horizontal, vertical);

		Vector2 scaledInput = input * speed * Time.deltaTime;

		Vector3 deltaMove = transform.forward * scaledInput.y + transform.right * scaledInput.x;

		transform.position += deltaMove;
	}
}
