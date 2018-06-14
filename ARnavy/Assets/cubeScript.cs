using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cubeScript : MonoBehaviour {

    private string strPos = null;
	// Use this for initialization
	void Start () {
        strPos = InitUI.destPos;
        Debug.Log("cubeScript strPos : " + strPos);
        if (strPos.StartsWith("(") && strPos.EndsWith(")"))
        {
            strPos = strPos.Substring(1, strPos.Length - 2);
        }

        // split the items
        string[] sArray = strPos.Split(',');

        // store as a Vector3
        Vector3 result = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]));

        result.y = 2.5f;

        transform.position = result;
        Debug.Log("cubeScript pos : " + transform.position);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
