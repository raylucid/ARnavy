using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
public class scene1 : MonoBehaviour {

    public QRCodeDecodeController qrcodecontroller;
    private string dataText = null;
    const string naverURL = "http://book.naver.com/search/search.nhn?sm=sta_hty.book&sug=&where=nexearch&query=";



    // Use this for initialization
    void Start () {
        VuforiaBehaviour.Instance.enabled = false;
    
        dataText = null;
        Reset();
        qrcodecontroller.onQRScanFinished += onScanFinished;
    }

    // Update is called once per frame
    void Update () {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.LoadLevel(0);
                return;
            }
        }
    }



    
    void onScanFinished(string str)
    {

        dataText = naverURL + str;
        if (dataText != null)
        {
            if (str.Length == 13)
            {
                Application.LoadLevel(0);
            }
        }
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, Screen.width, Screen.height / 10), "Reset"))
        {
            Reset();
        }
        GUI.Box(new Rect(0, Screen.height / 10, Screen.width, Screen.height / 10), "");
        GUI.Label(new Rect(0, Screen.height / 10, Screen.width, Screen.height / 10), dataText);
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
