using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using UnityEngine.SceneManagement;

public class InitUI : MonoBehaviour {

    // Use this for initialization
    public AspectRatioFitter fit;
    private AndroidJavaObject activityContext = null;
    private AndroidJavaClass ajc = null;

    public static string destPos = null;
    
    void Start () {
        float ratio = (float)Screen.width / (float)Screen.height;
        fit.aspectRatio = ratio;

//        tank = GameObject.Find("csTank").GetComponent<csTank>();

        ThreadStart th = new ThreadStart(work);
        Thread t = new Thread(th);
        t.Start();

        using (AndroidJavaClass activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            Debug.Log("kslee : activityClass");
            activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
            Debug.Log("kslee : currentActivity");


        }
        using (ajc = new AndroidJavaClass("com.example.kslee.librarynavi4.MainActivity"))
        {
            Debug.Log("kslee : ajc : " + ajc);
            if (ajc != null)
            {
                Debug.Log("kslee : callActivity");
                ajc.CallStatic("CallActivity", activityContext);
            }
        }
    }
	
    public static void work()
    {
    //    Debug.Log("sleep start");
        Thread.Sleep(2000);
        //Debug.Log("sleep 2sec");
    }

    public void ChangeScene(string msg)
    {
        //destPos = msg;
        //tank.SetDestFromAndroid(msg);
        destPos = msg;
        Debug.Log("changeScene() msg : " + msg);
        SceneManager.LoadScene("test");
    }
	// Update is called once per frame
	void Update () {
    }


}
