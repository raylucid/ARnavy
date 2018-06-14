using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterMoving : MonoBehaviour {
	public float moveSpeed = 10f;

	// Update is called once per frame
	void Update () {
		float h = Input.GetAxis ("Horizontal");
		float v = Input.GetAxis ("Vertical");
		transform.Translate(Vector3.forward * v * moveSpeed * Time.deltaTime);
		transform.Rotate(Vector3.up * h * moveSpeed * 10f * Time.deltaTime);
	}
}
