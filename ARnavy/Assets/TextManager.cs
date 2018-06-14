using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextManager : MonoBehaviour {
	public static TextManager instance;
	public Text FinishText;
	// Use this for initialization
	void Start () {
		if (!instance)
			instance = this;
	}
	public void ShowText()
	{
		FinishText.text = "Book is here!!";
	}
	// Update is called once per frame
	void Update () {
		
	}
}
