using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class onclick : MonoBehaviour {

    private WebViewObject webViewObject;
    public GameObject ob1, ob2, ob3;
    private test code;
    string barcode = "";
    string strUrl = "";
    const string naverURL = "http://book.naver.com/search/search.nhn?sm=sta_hty.book&sug=&where=nexearch&query=";
    // Use this for initialization
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
     
    }

    public void OnMouseDown()
    {
        if (gameObject.name == "blackCube")
        {
            ob1.SetActive(false);
            Instantiate(ob2, new Vector3(0, 0, 0), Quaternion.Euler(new Vector3(0, 0, 0)));
            ob2.SetActive(true);
            ob3.SetActive(true);
            code.Reset();
        }
    } 

}
