using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePosition_Destroy : MonoBehaviour {
	Vector3 Obj_pos;
	// Use this for initialization
	void Start () {
		Obj_pos = AppleManager._Instance.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		TestDebugBookPosition ();
	}
	public void TestDebugBookPosition()
	{
		Debug.Log("Position:" + Obj_pos + "!" );
	}
}
