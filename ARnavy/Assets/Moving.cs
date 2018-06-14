using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moving : MonoBehaviour {



	public Animator ani;
	public float moveSpeed;
	public bool idle;
	public bool walking;
	public Transform target;
	public GameObject master;
	public Vector3 nPosition;
	public bool turn;
	public float rotate;
	public float time;
	// Use this for initia lization
	void Start () {
		ani = GetComponent<Animator>();	
	//	master = GetComponent<GameObject> ();
		turn = false;
		time = 0f;
	}
	
	// Update is called once per frame
	void Update () {

		/*float h = Input.GetAxis ("Horizontal");
		float v = Input.GetAxis ("Vertical");
		transform.Translate(Vector3.forward * v * moveSpeed * Time.deltaTime);
		transform.Rotate(Vector3.up * h * moveSpeed * 10f * Time.deltaTime);
		*/
		/*if (v == 0) {
			ani.SetBool ("IsRun", false);
		} else if (Input.GetKey (KeyCode.Alpha0)) {
			ani.SetBool ("IsJump", true);
		} else
			ani.SetBool ("IsRun", true);
			*/
		float mDistance = Vector3.Distance(transform.position, master.transform.position);   //움직이는 마스터와 햄스터의 거리를 구함
		float tDistance = Vector3.Distance (transform.position, target.position);			//책과 햄스터의 거리

		if (tDistance > 2f) {
			if (mDistance < 10f) {
				Vector3 vec = (target.position - transform.position).normalized;
				Vector3 fixEuler = Quaternion.LookRotation (vec).eulerAngles; // x축으로 넘어지지 않도록 고정
				fixEuler.x = 0f;
				transform.rotation = Quaternion.Euler (fixEuler);

				transform.Translate (Vector3.forward * moveSpeed * Time.deltaTime);
				ani.SetBool ("IsRun", true);
				ani.SetBool ("IsWaiting", false);
				turn = false;
			} else {
				transform.Translate (Vector3.forward * 0f * Time.deltaTime);  //멈추고 뒤돌아봄,조건을 180도 회전할때가지 천천히 Rotate 
				//ani.SetBool ("IsRun", false);
				if (turn == false) {
					transform.Rotate (0, 180, 0);
				}
				time += Time.deltaTime;
				if (time > 3f) {
					ani.SetBool ("IsWaiting", true);
				}
			turn = true;
				//회전 -- 애니메이션을 만들자
				/*if (turn == false) {
					Vector3 mToRotate = (master.transform.position - transform.position);
					Quaternion Rotation = Quaternion.LookRotation (mToRotate);
					if (Rotation == transform.rotation)
						turn = true;
					else
						transform.Rotate (Vector3.up * moveSpeed * 10f * Time.deltaTime);
				}*/
			}
		} else {
			ani.SetBool ("IsRun", false);
			ani.SetBool ("IsJump", true);
	//		TextManager.instance.ShowText ();
		}
	}
}
