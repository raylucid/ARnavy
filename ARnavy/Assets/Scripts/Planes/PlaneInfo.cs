using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneInfo : MonoBehaviour {
    public int planeType; //0 -> 바닥, 1 -> 플레이어 위치, 2-> 목적지, 3-> 장애물, 4 -> 최단경로, 5-> 서가
    private int idx = -1;
    void Start()
    {
     
    }

    // Update is called once per frame
    void Update()
    {

    }

    //안씀
    //public void OnMouseDown()
    //{

    //    if (Input.GetKeyDown(KeyCode.B))
    //    {
    //        //Debug.Log("hel");
    //        SetColor(Color.black);
    //        planeType = 3;

    //    }
    //    else if (Input.GetKeyDown(KeyCode.D))
    //    {
    //        SetColor(Color.red);
    //        planeType = 2;

    //    }
    //}
    //바닥색깔바꾸기



    public void SetColor(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;
    }
    //바닥인덱스설정
    public void SetIndex(int idx)
    {
        this.idx = idx;
    }
}
