using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraMoving : MonoBehaviour {
	public Transform target;
	public float offsetX = 0f;
	public float offsetY = 0f;
	public float offsetZ = 0f;
	Vector3 cameraPosition;
	// Use this for initialization
	void Start () {
		cameraPosition = target.transform.position;
	}

	// Update is called once per frame
	void Update () {
	//	transform.rotation = target.transform.rotation;
		cameraPosition.x = target.transform.position.x + offsetX;
		cameraPosition.y = target.transform.position.y + offsetY;
		cameraPosition.z = target.transform.position.z + offsetZ;

		transform.position = cameraPosition;
		//transform.rotation = Quaternion.LookRotation ((target.position - transform.position).normalized);
		//transform.Translate (Vector3.forward *5f* Time.deltaTime);
	}
}
