using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vuforia;

public class test : MonoBehaviour {

    private WebViewObject webViewObject;
   // public QRCodeDecodeController qrcodecontroller;
    string dataText;
    public GameObject go1;
   // public scene1 myClass;
    const string naverURL = "http://book.naver.com/search/search.nhn?sm=sta_hty.book&sug=&where=nexearch&query=";

    // Use this for initialization
    void Start () {

        onScanFinished();
	}
	
	// Update is called once per frame
	void Update () {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                GameObject webviewOB = GameObject.Find("WebViewObject");
                Destroy(webviewOB);
                Reset();
                return;
            }
        }
        if (Input.GetKey(KeyCode.Escape))
        {
            GameObject webviewOB = GameObject.Find("WebViewObject");
            Destroy(webviewOB);
            Reset();
            return;
        }
    }


    public void OnMouseDown()
    {
        if (gameObject.name == "blackCube")
        {
            VuforiaBehaviour.Instance.enabled = false;
            dataText = "";
            SceneManager.LoadScene("barcode");
        }
    }
    void OnGUI()
    {
      /*  if(GUI.Button(new Rect(0,0,Screen.width ,Screen.height/10),"Reset"))
        {
            Reset();
        }
        GUI.Box(new Rect(0, Screen.height / 10, Screen.width, Screen.height / 10), "");
        GUI.Label(new Rect(0, Screen.height / 10, Screen.width, Screen.height / 10), dataText);*/
    }

    void onScanFinished()
    {
        dataText = gameMgr.Instance.ReturnDataTxt();
        if(dataText != null)
        {
                TurnOnInternet();
        }
        dataText = "";
    }
    public void TurnOnInternet()
    {
        
        webViewObject = (new GameObject("WebViewObject")).AddComponent<WebViewObject>();
        webViewObject.Init((msg) =>
        {
            Debug.Log(string.Format("CallFromJS[{0}]", msg)); 
        });

        webViewObject.LoadURL(dataText);
        webViewObject.SetVisibility(true);
        webViewObject.SetMargins(50, 50, 50, 50);

    }
    public string data()
    {
        return dataText;
    }

    public void Reset()
    {
       
        dataText = "";
    }
}

