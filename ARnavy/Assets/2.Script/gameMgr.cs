using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vuforia;

public class gameMgr : MonoBehaviour {

    private string score = null;
    private bool isPaused = false;

    private static gameMgr instance = null;

    public static gameMgr Instance
    {
        get
        {

            return instance;
        }
    }

    public string Score
    {
        get
        {
            return dataText;

        }
        set
        {
            dataText = value;
        }
    }

    void Awake()
    {
        if (instance)
        {
            DestroyImmediate(Instance);
           // DestroyImmediate(gameObject);
            
            return;
        }
        instance = this;
        Reset();
        DontDestroyOnLoad(gameObject);
    }

    public QRCodeDecodeController qrcodecontroller;
    public string dataText = null;
    const string naverURL = "http://book.naver.com/search/search.nhn?sm=sta_hty.book&sug=&where=nexearch&query=";
    private WebViewObject webViewObject;



    // Use this for initialization
    void Start()
    {
        VuforiaBehaviour.Instance.enabled = false;
        DontDestroyOnLoad(gameObject);
        dataText = null;
        Reset();
        qrcodecontroller.onQRScanFinished += onScanFinished;
    }

    // Update is called once per frame
    void Update()
    {
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

    }




    void onScanFinished(string str)
    {

        dataText = naverURL+str;
        if (dataText != null)
        {
            if (str.Length == 13)
            {
                SceneManager.LoadScene("test");
            }
        }
    }

    void OnGUI()
    {
      /*  if (GUI.Button(new Rect(0, 0, Screen.width, Screen.height / 10), "Reset"))
        {
            Reset();
        }
        GUI.Box(new Rect(0, Screen.height / 10, Screen.width, Screen.height / 10), "");
        GUI.Label(new Rect(0, Screen.height / 10, Screen.width, Screen.height / 10), dataText);*/
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
    public void Reset()
    {
        qrcodecontroller.Reset();
        dataText = "";
    }

    public string ReturnDataTxt()
    {
        return dataText;
    }
}
