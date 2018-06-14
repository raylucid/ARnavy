using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Data;

using Mono.Data.SqliteClient;
using System.IO;


public class csTank : MonoBehaviour {

    //플레이어 이동속도
    public float speed = 10.0f;
    //플레이어 회전속도
    public float rotSpeed = 120.0f;
    private float dirRotSpeed = 0.5f;
    //탱크의 발포부
   // public GameObject turret;
    //플레이어 위치
    private Vector3 playerPos;
    //플레이어 위치 변경시 변경된 위치를 알려주기 위해 선언
    private PlaneManager observer;
    private float deltaTime;
    private Vector3 targetPos;
    

    private bool dirChangeFlag = false;
    private float dirSpeed = 0.1f;
    public GameObject dirTarget;
    //public GameObject ARcamera;
    private float rotTargetSpeed = 0.0f;
    private Vector3 curDir;


    private bool initDbFlag = false;
    private bool searchFlag = false;

  //  public Text m_MyText;
    private string destPos = null;

    // Use this for initialization
    public bool mutex = false;
    void Start () {
        StartCoroutine(Init());
    }
    IEnumerator Init()
    {
        InitDictionary();

        now = new CSvector(0.0f, 0.0f, 0.0f);

        playerPos = transform.position;
        observer = GameObject.Find("PlaneManager").GetComponent<PlaneManager>();

        float amtToRot = rotSpeed * deltaTime;

        // dirTarget = GameObject.Find("ARCamera");

        targetPos = new Vector3(0, 0, 0);

        using (javaClass = new AndroidJavaClass("com.ray_lucid.blemeasure.BLEPluginClass"))
        {
            if (javaClass != null)
            {
                javaClassInstance = javaClass.CallStatic<AndroidJavaObject>("instance");
            }
        }

        javaClassInstance.Call("init", "");
        if (initDbFlag == false)
        {
            initFakeDB();
        }

        float uiScale = Screen.height / 500f;
        int scaledFontSize = Mathf.RoundToInt(16 * uiScale);
        //  m_MyText.fontSize = scaledFontSize;
        yield break;
    }
    IEnumerator Measure()
    {
        Debug.Log("Update() playerPos : " + playerPos);
        if (initDbFlag == false || searchFlag == false) yield break;

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
        float amtTomove = speed * deltaTime;
        float amtToRot = rotSpeed * deltaTime;

        float front = Input.GetAxis("Vertical"); //위, 아래 -> w, s Input.GetAxis("Mouse X");
        float ang = Input.GetAxis("Horizontal"); //왼쪽, 오른쪽 -> a, d Input.GetAxis("Mouse Y");
        if (this.dirChangeFlag)
        {
            dirTarget.transform.position = Vector3.Lerp(dirTarget.transform.position, this.targetPos, rotTargetSpeed);
            curDir = dirTarget.transform.position - this.transform.position;
            Quaternion newRotation = Quaternion.LookRotation(curDir);
            dirChangeFlag = false;
        }

        transform.Rotate(0, ang * amtToRot, 0);


        playerPos = transform.position;
        mutex = true;
        Measurement();
        mutex = false;
        Vector3 tmp_pos = GetNow();
        Debug.Log("Update() playerPos : " + playerPos + " tmp_pos : " + tmp_pos);
        if (playerPos.x != tmp_pos.x || playerPos.z != tmp_pos.z)
        {
            playerPos = tmp_pos;
            transform.position = playerPos;

            Notify(playerPos);

        }
        yield break;
    }
    public void SetDestFromAndroid(string strPos)
    {
        //m_MyText.text = strPos;
        // Remove the parentheses
        if (strPos.StartsWith("(") && strPos.EndsWith(")"))
        {
            strPos = strPos.Substring(1, strPos.Length - 2);
        }

        // split the items
        string[] sArray = strPos.Split(',');

        // store as a Vector3
        Vector3 result = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]));

        result.y = 2.5f;
        observer.SetDest(result);
        this.searchFlag = true;
        mutex = true;
        Measurement();
        mutex = false;
        playerPos = GetNow();
       Debug.Log("kslee after Mesurement() playerPos  : " + playerPos);
        Notify(playerPos);

    }

    // Update is called once per frame
    void Update () {
        Debug.Log("Update()");
        StartCoroutine(Measure());
    }
    //플레이어의 변경된 위치를 PlaneManager에게 알려준다.
    public void Notify(Vector3 pos)
    {
        //       Debug.Log("playerPos : " + pos);
    
        observer.UpdatePlayerPos(pos);
    }

    public void SetDirection(Vector3 dir)
    {
        Vector3 diff = new Vector3(this.targetPos.x - dir.x, this.targetPos.y - dir.y, this.targetPos.z - dir.z);
        float diff_sum = Mathf.Abs(diff.x) + Mathf.Abs(diff.y) + Mathf.Abs(diff.z);
        if (diff_sum > 1.0)
        {
            this.targetPos = dir;
            dirChangeFlag = true;
        }
        else
        {
        }

    }

   
    void OnApplicationQuit()
    {
        observer.DeleteDest();
        Debug.Log("csTank 종료");
        CloseDB();
        javaClassInstance.Call("stopscan", "");
    }

    /*
     *
     * 
     * 여기서부터 연호 소스 
     * 
     * 
     */

    public struct Beacon
    {
        public string name { get; set; }
        public int index { get; set; }
        public float RSSI { get; set; }
        public float distance { get; set; }
    }
    public List<Beacon> BeaconList;
    public List<Dictionary<String, double>> area;

    public void InitDictionary()
    {
        BeaconList = new List<Beacon>();
        area = new List<Dictionary<string, double>>();
        area.Add(new Dictionary<string, double>() { });
        area.Add(new Dictionary<string, double>() { });
        area.Add(new Dictionary<string, double>() { });
        //0 구역 데이터 입력
        area[0].Add("1346", 12.32); area[0].Add("1348", 21.67); area[0].Add("1384", 5.42);
        area[0].Add("1386", 0.99); area[0].Add("3146", 7.88); area[0].Add("3148", 0.99);
        area[0].Add("4123", 1.48); area[0].Add("4132", 0.99); area[0].Add("4138", 2.46);
        area[0].Add("4182", 0.49); area[0].Add("4183", 11.33); area[0].Add("4186", 0.99);
        area[0].Add("4213", 1.48); area[0].Add("4218", 15.27); area[0].Add("4231", 2.96);
        area[0].Add("4238", 2.96); area[0].Add("4281", 2.96); area[0].Add("4318", 0.49);
        area[0].Add("4381", 0.99); area[0].Add("4386", 0.99); area[0].Add("4813", 1.48);
        area[0].Add("4816", 0.49); area[0].Add("4831", 2.46); area[0].Add("4836", 0.49);
        //1 구역 데이터 입력
        area[1].Add("1354", 0.94); area[1].Add("1358", 7.08); area[1].Add("1385", 9.91);
        area[1].Add("1386", 0.94); area[1].Add("1835", 4.25); area[1].Add("1836", 0.47);
        area[1].Add("5416", 10.38); area[1].Add("5461", 8.49); area[1].Add("5462", 1.42);
        area[1].Add("5467", 5.66); area[1].Add("5476", 0.47); area[1].Add("5486", 2.36);
        area[1].Add("6483", 1.42); area[1].Add("6841", 3.3); area[1].Add("6843", 18.87);
        area[1].Add("8243", 0.47); area[1].Add("8412", 1.42); area[1].Add("8421", 12.74);
        area[1].Add("8423", 7.08); area[1].Add("8431", 1.42); area[1].Add("8432", 0.94);
        //2 구역 데이터 입력
        area[2].Add("4576", 0.92); area[2].Add("4578", 1.84); area[2].Add("4756", 1.84);
        area[2].Add("4758", 7.37); area[2].Add("4765", 1.84); area[2].Add("5476", 0.46);
        area[2].Add("5478", 7.83); area[2].Add("5486", 0.46); area[2].Add("5746", 0.46);
        area[2].Add("5748", 2.3); area[2].Add("5784", 0.92); area[2].Add("6481", 1.38);
        area[2].Add("6485", 0.46); area[2].Add("6834", 1.38); area[2].Add("6835", 1.84);
        area[2].Add("6841", 0.46); area[2].Add("6843", 11.98); area[2].Add("6845", 12.9);
        area[2].Add("6847", 4.61); area[2].Add("6853", 0.46); area[2].Add("6854", 1.38);
        area[2].Add("6874", 2.3); area[2].Add("6875", 3.23); area[2].Add("7456", 8.76);
        area[2].Add("7458", 3.23); area[2].Add("7465", 1.38); area[2].Add("7485", 1.38);
        area[2].Add("7548", 0.46); area[2].Add("7645", 0.46); area[2].Add("7845", 2.3);
        area[2].Add("7854", 2.3); area[2].Add("8643", 2.76); area[2].Add("8645", 5.99);
        area[2].Add("8651", 0.46); area[2].Add("8652", 0.46); area[2].Add("8654", 1.38);
    }

    public void AddBeacon(string msg)
    {
        if (mutex)
            return;
        string[] list = msg.Split(',');
        BeaconList.Add(new Beacon()
        {
            name = list[0],
            index = Int32.Parse(list[1]) - 1,
            RSSI = float.Parse(list[2]),
            distance = float.Parse(list[3])
        });
        
    }
    //비콘리스트.
    private List<CSvector> beacons = new List<CSvector>();
    private AndroidJavaClass javaClass = null;
    private AndroidJavaObject javaClassInstance = null;

    public float BeaconHeight = 2.3f;

    //비콘들을 이용하여 계산한 현재 위치
    private CSvector now;

    public void ClearBeaconList(string str)
    {
        if (mutex)
            return;
        BeaconList.Clear();
    }
   
    public void checkBluetooth(string str)
    {
        
        javaClassInstance.Call("checkBluetooth", "");
    }


    float getDistance(CSvector a, CSvector b)
    {
        float distance = 0;
        distance = (float)Mathf.Sqrt(Mathf.Pow(a.getx() - b.getx(), 2) + Mathf.Pow(a.getz() - b.getz(), 2));
        return distance;
    }

    //실제값과 가상값을 따로 가져간다.
    //비콘으로부터 계산은 가상값을 기준으로 한다.
    //비콘 이름에 따른 서가는 가상시스템에서 이미 배치되었다고 가정.
    //비콘을 통해 비콘 이름을 얻으면, 그 이름을 디비에서 검색하여 가상 시스템에서의 서가 인덱스를 구한다.
    //planes 배열에서 인덱스를 통해 위치를 얻어낸다.

    CSvector getCoordinate(CSvector p1, CSvector p2, CSvector p3, float d1, float d2, float d3)
    {
        //if (BeaconNameList.Count < 4) return now;
        CSvector ex = (p2 - p1) / (float)(p2 - p1).getLength();
        float i = ex % (p3 - p1);
        CSvector a = (p3 - p1) - (ex * i);
        CSvector ey = a / (float)a.getLength();
        CSvector ez = ex * ey;
        float d = (float)(p2 - p1).getLength();
        float j = ey % (p3 - p1);
        float x = (float)(Mathf.Pow(d1, 2) - Mathf.Pow(d2, 2) + Mathf.Pow(d, 2)) / (2 * d);
        float y = (float)(Mathf.Pow(d1, 2) - Mathf.Pow(d3, 2) + Mathf.Pow(i, 2) + Mathf.Pow(j, 2)) / (2 * j) - (i / j) * x;
        float b = (float)(Mathf.Pow(d1, 2) - Mathf.Pow(x, 2) - Mathf.Pow(y, 2));
        if (Mathf.Abs(b) < 0.0000000001)
            b = 0;
        float z = (float)Mathf.Sqrt(b);
        a = p1 + (ex * x) + (ey * y);
        CSvector p4a = p1 + (ex * x) + (ey * y) + (ez * z);
        CSvector p4b = p1 + (ex * x) + (ey * y) - (ez * z);
        if (float.IsNaN(z))
        {
            a.setError(-1);
            return a;
        }
        else
        {
            a.setError(1);
            p4a.setError(1);
            p4b.setError(1);
        }
        if (z == 0)
        {
            return a;
        }
        else
        {
            if (p4a.gety() < BeaconHeight)
                return p4a;
            else
                return p4b;
        }
    }


    CSvector ConvertToCSVector(Vector3 pos)
    {
        CSvector newPos = new CSvector(pos.x, pos.y, pos.z);
        return newPos;
    }

    //beacons[i]를 0,0부터 할당
    //name별로 인덱스 부여.
    //이름이 주어지면, 인덱스를 찾고, 가상 위치에 결정
    //가상위치는 1 거리당 1m.
    //실제 위치는 예를 들어 1.24 실제 거리가 나오면 가상거리 1로 친다.

    public void Measurement()
    {
        //Debug.Log("Measurement() : " + BeaconList.Count);
        if (BeaconList.Count < 8) return;
        Debug.Log("Measurement() : " + BeaconList.Count);
        String chk = (BeaconList[0].index + 1) + "" + (BeaconList[1].index + 1) + "" + (BeaconList[2].index + 1) + "" + (BeaconList[3].index + 1);
        int dindex = -1;
        double max = -1;
        int b1 = -1, b2 = -1, b3 = -1, b4 = -1;
        CSvector p1;
        CSvector p2;
        CSvector p3;
        CSvector p4;
        for (int i = 0; i < 3; i++)
        {
            if (area[i].ContainsKey(chk))
            {
                if (max < area[i][chk])
                {
                    dindex = i;
                    max = area[i][chk];
                }
                else
                    continue;
            }
            else
                continue;
        }
        switch (dindex)
        {
            case 0:
                for (int i = 0; i < 8; i++)
                {
                    if (BeaconList[i].index + 1 == 1)
                        b1 = i;
                    if (BeaconList[i].index + 1 == 2)
                        b2 = i;
                    if (BeaconList[i].index + 1 == 3)
                        b3 = i;
                    if (BeaconList[i].index + 1 == 4)
                        b4 = i;
                    if (b1 != -1 && b2 != -1 && b3 != -1 && b4 != -1)
                        break;
                }
                break;
            case 1:
                for (int i = 0; i < 8; i++)
                {
                    if (BeaconList[i].index + 1 == 3)
                        b1 = i;
                    if (BeaconList[i].index + 1 == 4)
                        b2 = i;
                    if (BeaconList[i].index + 1 == 5)
                        b3 = i;
                    if (BeaconList[i].index + 1 == 6)
                        b4 = i;
                    if (b1 != -1 && b2 != -1 && b3 != -1 && b4 != -1)
                        break;
                }
                break;
            case 2:
                for (int i = 0; i < 8; i++)
                {
                    if (BeaconList[i].index + 1 == 4)
                        b1 = i;
                    if (BeaconList[i].index + 1 == 5)
                        b2 = i;
                    if (BeaconList[i].index + 1 == 6)
                        b3 = i;
                    if (BeaconList[i].index + 1 == 7)
                        b4 = i;
                    if (b1 != -1 && b2 != -1 && b3 != -1 && b4 != -1)
                        break;
                }
                break;
            default:
                break;
        }
        if (dindex != -1)
        {
           
            p1 = WeightCalculate(beacons[BeaconList[b1].index], beacons[BeaconList[b2].index], beacons[BeaconList[b3].index], Math.Abs(BeaconList[b1].RSSI) - 59, Math.Abs(BeaconList[b2].RSSI) - 59, Math.Abs(BeaconList[b3].RSSI) - 59);
            p2 = WeightCalculate(beacons[BeaconList[b1].index], beacons[BeaconList[b2].index], beacons[BeaconList[b4].index], Math.Abs(BeaconList[b1].RSSI) - 59, Math.Abs(BeaconList[b2].RSSI) - 59, Math.Abs(BeaconList[b4].RSSI) - 59);
            p3 = WeightCalculate(beacons[BeaconList[b2].index], beacons[BeaconList[b3].index], beacons[BeaconList[b4].index], Math.Abs(BeaconList[b2].RSSI) - 59, Math.Abs(BeaconList[b3].RSSI) - 59, Math.Abs(BeaconList[b4].RSSI) - 59);
            p4 = WeightCalculate(beacons[BeaconList[b1].index], beacons[BeaconList[b3].index], beacons[BeaconList[b4].index], Math.Abs(BeaconList[b1].RSSI) - 59, Math.Abs(BeaconList[b3].RSSI) - 59, Math.Abs(BeaconList[b4].RSSI) - 59);
        }
        else
        {
            p1 = WeightCalculate(beacons[BeaconList[0].index], beacons[BeaconList[1].index], beacons[BeaconList[2].index], Math.Abs(BeaconList[0].RSSI) - 59, Math.Abs(BeaconList[1].RSSI) - 59, Math.Abs(BeaconList[2].RSSI) - 59);
            p2 = WeightCalculate(beacons[BeaconList[0].index], beacons[BeaconList[1].index], beacons[BeaconList[3].index], Math.Abs(BeaconList[0].RSSI) - 59, Math.Abs(BeaconList[1].RSSI) - 59, Math.Abs(BeaconList[3].RSSI) - 59);
            p3 = WeightCalculate(beacons[BeaconList[1].index], beacons[BeaconList[2].index], beacons[BeaconList[3].index], Math.Abs(BeaconList[1].RSSI) - 59, Math.Abs(BeaconList[2].RSSI) - 59, Math.Abs(BeaconList[3].RSSI) - 59);
            p4 = WeightCalculate(beacons[BeaconList[0].index], beacons[BeaconList[2].index], beacons[BeaconList[3].index], Math.Abs(BeaconList[0].RSSI) - 59, Math.Abs(BeaconList[2].RSSI) - 59, Math.Abs(BeaconList[3].RSSI) - 59);
        }
        CSvector mid = (p1 + p2 + p3 + p4) / 4;
        SetNow(mid.getx(), mid.gety(), mid.getz());
    }
    //beacons[i]를 0,0부터 할당
    //name별로 인덱스 부여.
    //이름이 주어지면, 인덱스를 찾고, 가상 위치에 결정
    //가상위치는 1 거리당 1m.
    //실제 위치는 예를 들어 1.24 실제 거리가 나오면 가상거리 1로 친다.

    //실제 비콘이랑 합칠 때 사용할 코드
    public void TriangleMeasure()
    {
        if (BeaconList.Count < 8) return;
        float x = 0, y = 0, z = 0;
        if (BeaconList.Count < 8) return;
        String chk = (BeaconList[0].index + 1) + "" + (BeaconList[1].index + 1) + "" + (BeaconList[2].index + 1) + "" + (BeaconList[3].index + 1);
        int dindex = -1;
        double max = -1;
        int b1 = -1, b2 = -1, b3 = -1, b4 = -1;

        CSvector p1;
        CSvector p2;
        CSvector p3;
        CSvector p4;
        for (int i = 0; i < 3; i++)
        {
            if (area[i].ContainsKey(chk))
            {
                if (max < area[i][chk])
                {
                    dindex = i;
                    max = area[i][chk];
                }
                else
                    continue;
            }
            else
                continue;
        }
        switch (dindex)
        {
            case 0:
                for (int i = 0; i < 8; i++)
                {
                    if (BeaconList[i].index + 1 == 1)
                        b1 = i;
                    if (BeaconList[i].index + 1 == 2)
                        b2 = i;
                    if (BeaconList[i].index + 1 == 3)
                        b3 = i;
                    if (BeaconList[i].index + 1 == 4)
                        b4 = i;
                    if (b1 != -1 && b2 != -1 && b3 != -1 && b4 != -1)
                        break;
                }
                break;
            case 1:
                for (int i = 0; i < 8; i++)
                {
                    if (BeaconList[i].index + 1 == 3)
                        b1 = i;
                    if (BeaconList[i].index + 1 == 4)
                        b2 = i;
                    if (BeaconList[i].index + 1 == 5)
                        b3 = i;
                    if (BeaconList[i].index + 1 == 6)
                        b4 = i;
                    if (b1 != -1 && b2 != -1 && b3 != -1 && b4 != -1)
                        break;
                }
                break;
            case 2:
                for (int i = 0; i < 8; i++)
                {
                    if (BeaconList[i].index + 1 == 4)
                        b1 = i;
                    if (BeaconList[i].index + 1 == 5)
                        b2 = i;
                    if (BeaconList[i].index + 1 == 6)
                        b3 = i;
                    if (BeaconList[i].index + 1 == 7)
                        b4 = i;
                    if (b1 != -1 && b2 != -1 && b3 != -1 && b4 != -1)
                        break;
                }
                break;
            default:
                break;
        }
        if (dindex != -1)
        {
            p1 = getCoordinate(beacons[BeaconList[b1].index], beacons[BeaconList[b2].index], beacons[BeaconList[b3].index], BeaconList[b1].distance, BeaconList[b2].distance, BeaconList[b3].distance);
            p2 = getCoordinate(beacons[BeaconList[b1].index], beacons[BeaconList[b2].index], beacons[BeaconList[b4].index], BeaconList[b1].distance, BeaconList[b2].distance, BeaconList[b4].distance);
            p3 = getCoordinate(beacons[BeaconList[b2].index], beacons[BeaconList[b3].index], beacons[BeaconList[b4].index], BeaconList[b2].distance, BeaconList[b3].distance, BeaconList[b4].distance);
            p4 = getCoordinate(beacons[BeaconList[b1].index], beacons[BeaconList[b3].index], beacons[BeaconList[b4].index], BeaconList[b1].distance, BeaconList[b3].distance, BeaconList[b4].distance);
        }
        else
        {
            p1 = getCoordinate(beacons[BeaconList[0].index], beacons[BeaconList[1].index], beacons[BeaconList[2].index], BeaconList[0].distance, BeaconList[1].distance, BeaconList[2].distance);
            p2 = getCoordinate(beacons[BeaconList[0].index], beacons[BeaconList[1].index], beacons[BeaconList[3].index], BeaconList[0].distance, BeaconList[1].distance, BeaconList[3].distance);
            p3 = getCoordinate(beacons[BeaconList[1].index], beacons[BeaconList[2].index], beacons[BeaconList[3].index], BeaconList[1].distance, BeaconList[2].distance, BeaconList[3].distance);
            p4 = getCoordinate(beacons[BeaconList[0].index], beacons[BeaconList[2].index], beacons[BeaconList[3].index], BeaconList[0].distance, BeaconList[2].distance, BeaconList[3].distance);
        }
        int count = 0;
        if (p1.getError() > 0)
        {
            x += p1.getx();
            y += p1.gety();
            z += p1.getz();
            count++;
        }
        if (p2.getError() > 0)
        {
            x += p2.getx();
            y += p2.gety();
            z += p2.getz();
            count++;
        }
        if (p3.getError() > 0)
        {
            x += p3.getx();
            y += p3.gety();
            z += p3.getz();
            count++;
        }
        if (p4.getError() > 0)
        {
            x += p4.getx();
            y += p4.gety();
            z += p4.getz();
            count++;
        }
        if (count != 0)
        {
            x /= count;
            y /= count;
            z /= count;
            SetNow(x, y, z);
        }
    }

    CSvector WeightCalculate(CSvector p1, CSvector p2, CSvector p3, float d1, float d2, float d3)
    {
        CSvector m1, m2, m3, m;
        m1 = p2 + (p1 - p2) * (d2 / (d1 + d2));
        m2 = p3 + (p1 - p3) * (d3 / (d1 + d3));
        m3 = p3 + (p2 - p3) * (d3 / (d2 + d3));
        m = (m1 + m2 + m3) / 3;
        return m;
    }


   

    //실제 비콘이랑 합칠 때 사용할 코드

  

    private void SetNow(float x, float y, float z)
    {
        now.setx(x);
        now.sety(y);
        now.setz(z);
    }

    public Vector3 GetNow()
    {
        Vector3 now = new Vector3(this.now.getx(), this.now.gety(), this.now.getz());
        return now;
    }

    public void SetBeacon(Vector3 pos)
    {
        CSvector cs_pos = new CSvector(pos.x, pos.y, pos.z);
        this.beacons.Add(cs_pos);
    }


    class CSvector
    {
        float[] coordinates;

        public CSvector() //zero values
        {
            coordinates = new float[4];
            coordinates[0] = 0;
            coordinates[1] = 0;
            coordinates[2] = 0;
            coordinates[3] = 0;
        }
        public CSvector(float x, float y, float z)//initial values
        {
            coordinates = new float[4];
            coordinates[0] = x;
            coordinates[1] = y;
            coordinates[2] = z;
            coordinates[3] = 0;
        }
        //Component-wise functions for retrieving the x,y,z values
        public float getx() { return coordinates[0]; }
        public float gety() { return coordinates[1]; }
        public float getz() { return coordinates[2]; }
        public float getError() { return coordinates[3]; }
        public float[] getCSvector()
        {
            float[] copy = new float[3];
            for (int i = 0; i < 3; i++)
                copy[i] = coordinates[i];
            return copy;
        }
        //Component-wise functions for setting the x,y,z values
        public void setx(float x) { coordinates[0] = x; }
        public void sety(float y) { coordinates[1] = y; }
        public void setz(float z) { coordinates[2] = z; }
        public void setError(float e) { coordinates[3] = e; }
        public void setCSvector(float x, float y, float z)
        {
            coordinates[0] = x;
            coordinates[1] = y;
            coordinates[2] = z;
        }
        public static CSvector operator +(CSvector a, CSvector b)//CSvector addition
        {
            CSvector c = new CSvector(a.getx() + b.getx(), a.gety() + b.gety(), a.getz() + b.getz());
            return c;
        }
        public static CSvector operator -(CSvector a, CSvector b)//CSvector subtraction
        {
            CSvector c = new CSvector(a.getx() - b.getx(), a.gety() - b.gety(), a.getz() - b.getz());
            return c;
        }
        public static CSvector operator !(CSvector b)//Negation
        {
            CSvector a = new CSvector(-b.getx(), -b.gety(), -b.getz());
            return a;
        }
        public static CSvector operator *(CSvector b, float s)//Scalar multiplication
        {
            CSvector a = new CSvector(b.getx() * s, b.gety() * s, b.getz() * s);
            return a;
        }
        public static CSvector operator /(CSvector b, float s)//Scalar divide
        {
            CSvector a = new CSvector(b.getx() / s, b.gety() / s, b.getz() / s);
            return a;
        }
        public static float operator %(CSvector a, CSvector b)//Dot product
        {
            return ((a.getx() * b.getx()) + (a.gety() * b.gety()) + (a.getz() * b.getz()));
        }
        public static CSvector operator *(CSvector b, CSvector c)//Cross product
        {
            CSvector a = new CSvector((b.gety() * b.getz()) - (b.getz() * b.gety()), (b.getz() * b.getx()) - (b.getx() * b.getz()), (b.getx() * b.gety()) - (b.gety() * b.getx()));
            return a;
        }
        public static bool operator ==(CSvector a, CSvector b)
        {
            if (a.getx() == b.getx() && a.gety() == b.gety() && a.getz() == b.getz())
                return true;
            else
                return false;
        }
        public static bool operator !=(CSvector a, CSvector b)
        {
            if (a.getx() == b.getx() && a.gety() == b.gety() && a.getz() == b.getz())
                return false;
            else
                return true;
        }
        public double getLength()//Length of the CSvector
        {
            return Mathf.Sqrt((this.getx() * this.getx()) + (this.gety() * this.gety()) + (this.getz() * this.getz()));
        }
    };

    private string connection;
    private IDbConnection dbcon;
    private IDbCommand dbcmd;
    private IDataReader reader;




    void initFakeDB()
    {
        float[] arr_x = { 0, 48, 0, 48, 0, 48, 0, 48 };
        float[] arr_y = { 0, 0, 24, 24, 72, 72, 96, 96 };
        string[] nameList = { "001_lib13097", "002_lib13097", "001_lib13097", "003_lib13097"
                , "004_lib13097", "005_lib13097", "006_lib13097","007_lib13097","008_lib13097" };   
        for (int i = 0; i < 8; i++)
        {
            float posx = arr_x[i];
            float posz = arr_y[i];
            Vector3 pos = new Vector3(posx, 2.5f, posz);

            this.observer.SetBeacon2(pos);
        }
        destPos = InitUI.destPos;
        SetDestFromAndroid(destPos);
        initDbFlag = true;

    }

    void CloseDB()
    {
        if (dbcon != null)
        {
            dbcon.Close();
        }
    }

    
        
    public IDataReader ExeQuery(string query)
    {
        dbcmd = dbcon.CreateCommand();
        dbcmd.CommandText = query;
        return dbcmd.ExecuteReader();
    }

  
    public int GetBeaconIndexByName(string name)
    {
        reader = ExeQuery("SELECT NUMBER FROM SHELF WHERE NAME = '" + name + "'");
        reader.Read();
        return reader.GetInt32(0);
    }

    Vector3 GetPosByBookName(string bookName) //책 제목으로 서가 번호(인덱스)를 찾는다.
    {
        reader = ExeQuery("SELECT 서가 FROM 도서정보 WHERE 서명 ='" + bookName + "'");
        reader.Read();
        int shelfNumber = reader.GetInt32(0);

        reader = ExeQuery("SELECT pos_x, pos_z FROM SHELF WHERE NUMBER='" + shelfNumber + "'");
        reader.Read();
        float pos_x = reader.GetFloat(0);
        float pos_z = reader.GetFloat(1);

        return new Vector3(pos_x, 2.5f, pos_z);
    }



    public Vector3 GetBeaconPos(int idx)
    {
   //     Debug.Log("leekasong !!! : idx : " + idx);
        CSvector pos = this.beacons[idx];
        return new Vector3(pos.getx(), pos.gety(), pos.getz());
    }


}
