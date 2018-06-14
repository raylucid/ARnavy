using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveDestination : MonoBehaviour {
	public GameObject goal;
	public Animator ani;
	public bool idle;
	public bool walking;
	// Use this for initialization
	void Start () {
		ani = GetComponent<Animator>();	
		NavMeshAgent agent = GetComponent<NavMeshAgent>();
		agent.destination = goal.transform.position; 
	}
	
	// Update is called once per frame
	void Update () {
		float tDistance = Vector3.Distance (transform.position, goal.transform.position);
		if (tDistance > 2f) {
				ani.SetBool ("IsRun", true);
				ani.SetBool ("IsWaiting", false);
		}else {
			ani.SetBool ("IsRun", false);
			ani.SetBool ("IsJump", true);
			//		TextManager.instance.ShowText ();
		}
	}
}
