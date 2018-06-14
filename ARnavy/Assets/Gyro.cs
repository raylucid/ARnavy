using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gyro : MonoBehaviour {
	private Gyroscope gyro;
	// Use this for initialization
	void Start () {
		gyro = Input.gyro;
		gyro.enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
		Invoke ("gyroupdate", 0.1f);
	}

	void gyroupdate(){
		Quaternion transquat = Quaternion.identity;
		transquat.w = gyro.attitude.w;
		//x,y축 뒤집음
		transquat.x = -gyro.attitude.x;
		transquat.y = -gyro.attitude.y;
		transquat.z = gyro.attitude.z;
		//transform.eulerAngles = new Vector3(0.0f,transform.rotation.y,0.0f);
		transform.rotation = Quaternion.Euler(90,0,0) * transquat;
	}
}
