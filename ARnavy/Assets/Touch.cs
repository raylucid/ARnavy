using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Touch : MonoBehaviour {

	public GameObject hamstor;
	private Touch tempTouchs;
	private Vector3 touchedPos;
	private bool touchOn;
	private Vector3 dirToTouch;

	// Use this for initialization
	void Start () {
		touchOn = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.touchCount == 0) {
			//애니메이션 지정 ~ 돌아가면서 행동시작
		} else if (Input.touchCount == 1) {
			touchShow ();   //테스트용으로 터치된 곳에 동적으로 오브젝트를 생성해주고 그쪽을 바라 볼 수 있도록
		} else if (Input.touchCount == 2) {
			touchZoom ();  // 줌기능을 추가
		} else {
			return;
		}
	}

	void touchShow()
	{
		//tempTouchs = Input.GetTouch (0);  
		if( Input.GetTouch(0).phase == TouchPhase.Began )  //알아보기 Touch.phase
		{
			touchOn = true;
			touchedPos = Camera.main.ScreenToWorldPoint (new Vector3(Input.GetTouch(0).position.x,Input.GetTouch(0).position.y,-Camera.main.transform.position.z));
	
			dirToTouch = touchedPos - hamstor.transform.position; // 바라보는 방향벡터를 구하고
			Vector3 look = Vector3.Slerp(hamstor.transform.forward, dirToTouch.normalized , Time.deltaTime * 2.0f);//보간을 이용해 look벡ㅓ를 구ㅁ
			hamstor.transform.rotation = Quaternion.LookRotation(look,Vector3.up); //look를 잉해 쿼터니언 회전
			
		} 
		else if(Input.GetTouch(0).phase == TouchPhase.Moved)
		{	
			
		}

	}

	void touchZoom()
	{
		if (Input.touchCount == 2 && Input.GetTouch (0).phase == TouchPhase.Moved && Input.GetTouch (1).phase == TouchPhase.Moved) 
		{
			float touchDelta = 0.0F;
			float distance = 0.0F;
			Vector2 prevDist = new Vector2 (0, 0);
			Vector2 curDist = new Vector2 (0, 0);

			curDist = Input.GetTouch (0).position - Input.GetTouch (1).position;
			prevDist = ((Input.GetTouch (0).position - Input.GetTouch(0).deltaPosition) - (Input.GetTouch (1).position - Input.GetTouch(1).deltaPosition));

			touchDelta = curDist.magnitude - prevDist.magnitude;
			distance= touchDelta;
			if (distance > 100)
				distance = 100;
			else if (distance < 20)
				distance = 20;

			if (distance < 100 && distance > 20)
			{
				if ((touchDelta < 0)) {
					Camera.main.fieldOfView = Mathf.Lerp (Camera.main.fieldOfView, distance, Time.deltaTime * 5);
				}

				if ((touchDelta > 0)) {
					Camera.main.fieldOfView = Mathf.Lerp (Camera.main.fieldOfView, distance, Time.deltaTime * 5);
				}
			}
		}
	}

	void MoveLimit() 
	{ 
		Vector3 temp;
		temp.x = Mathf.Clamp( transform.position.x , -1 , 1 );
		temp.y = Mathf.Clamp( transform.position.y , 5 ,  7 );
		temp.z = Mathf.Clamp( transform.position.z , 10 , 12);

		transform.position = temp;	
	}


}
