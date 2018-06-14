using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour {

    public Transform target;
    public float dist = 10.0f;
    public float height = 5.0f;
    public float smoothRotate = 5.0f;
    public Transform tr;
    public bool flag = true;
	void Start () {
        tr = GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void Update () {
        float curYangle = Mathf.LerpAngle(tr.eulerAngles.y, target.eulerAngles.y, smoothRotate * Time.deltaTime);

        Quaternion rot = Quaternion.Euler(0, curYangle, 0);

        tr.position = target.position - (rot * Vector3.forward * dist) + (Vector3.up * height);
        tr.LookAt(target);

	}
}
