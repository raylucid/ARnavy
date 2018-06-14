using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Appleexit : MonoBehaviour {
	uint exitCountValue = 0;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.Escape)) {
			exitCountValue++; //1인상태
			if (!IsInvoking ("disable_DoubleClick"))
				Invoke ("disable_DoubleCilck", 0.3f);
			
		}
		if(exitCountValue ==2){
			CancelInvoke ("disable_DoubleClick");
			Application.Quit ();
		}
	}

	void disable_DoubleClick(){
		exitCountValue = 0;
		SceneManager.LoadScene ("init");
	}

}
