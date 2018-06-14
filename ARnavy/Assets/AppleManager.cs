using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleManager : MonoBehaviour {

	public static AppleManager _Instance = null;
	void Awake()
	{
		if (_Instance == null) {
			_Instance = this;
		} else if (_Instance != this)
			Destroy (gameObject);

		DontDestroyOnLoad(gameObject);
	}
	// Update is called once per frame
	void Update () {

	}
	/*
	public static AppleManager Instance
	{
		get
		{
			if (_Instance == null)
			{
				_Instance = this;
			} 
			return _Instance;
		}
	}
	*/
	public void TestDebugPosition()
	{
		Debug.Log("Position:" + gameObject.transform.position + "!" );
	}

	public void OnApplicationQuit()
	{
		_Instance = null;
	}
}
