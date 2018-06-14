using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dirTarget : MonoBehaviour {

    private Vector3 pos;
	// Use this for initialization
	void Start () {
        pos.x = 0;
        pos.y = 1;
        pos.z = 5;
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void SetPos(Vector3 pos)
    {
        this.pos = pos;
        transform.position = this.pos;
    }
}
