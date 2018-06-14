using System.Collections;
using System.Collections.Generic;
//using UnityEditor;
using UnityEngine;



public class PlaneManager : MonoBehaviour {

    //논리적 바닥.
    //가로 100 세로 100인 바닥을 1 by 1의 바닥의 집합으로 생각하기 위해
    //자료형 정의
    public struct PlaneData
    {
        public Vector3 pos; //위치
        public int type; //

    };

    //바닥(prefab)
    public GameObject normalPlane;
    //서가(prefab)
    public GameObject normalShelf;
    //바닥들
    public PlaneData[] planes = null;
 //   public GameObject[] gamePlanes = null; 

    //바닥갯수
    private int ground_width = 48 + 1;
    private int ground_height = 96 + 1;
    private const int MAX_PLANES_NUM = (48+1)*(96+1);

   

    //플레이어위치
    private Vector3 playerPos;
    //플레이어인덱스
    private int playerPlaneIdx;
    //A * 알고리즘을 계산해주는 객체
    private AStar aStar = null;
    //현재 스크립트 파일에 대한 객체
    private static PlaneManager instance;
   
    //사용자가 움직이지 않으면 경로를 다시 그리지 않게 하기 위해 선언한 플래그 변수
    private bool redrawFlag = false;
    //목적지에 도착했음을 알려주는 플래그 변수
    private bool arriveFlag = true;
    //플레이어객체
    private csTank playerInfo;
    //목적지 인덱스
    private int dest_index;
  
    private Vector3 direction;

    //목적지가 정해졌는지 여부를 담고 있는 변수
    private bool isDestSelected;
    //목적지(빨간기둥)
    private Vector3 dest;
  //  private GameObject gameDest;

    //게임오브젝트를 생성, 삭제 반복시 부하가 발생. 부하를 막기 위해 게임오브젝트의 pool로 만들어놓은 비콘리스트.
    private List<PlaneData> beaconList;
 //   private List<GameObject> beaconGameList;

  
    void Start () {
        isDestSelected = false;
       
        beaconList = new List<PlaneData>();
   //     beaconGameList = new List<GameObject>();
       
        //플레이어 객체 생성
        playerInfo = GameObject.Find("csTank").GetComponent <csTank> ();
        CreatePlanes();
        

        
        instance = this;
        //A * 객체 생성
        aStar = new AStar(instance);
     
    }

    void Update()
    {
            if (redrawFlag)
            {

                if (arriveFlag && aStar.CalculatePath() > 0) // -1 -> 오류, 0 -> 목적지 못찾음.
                {
                    
                    aStar.Draw();
                    List<Vector3> list = GetShortestPath();
                    
                    if (direction.Equals(new Vector3(-1, -1, -1)))
                    {
                        arriveFlag = false;
                    }
                    else
                    {
                        playerInfo.SetDirection(direction);
                        Debug.Log("update() direction : " + direction);
                        for(int i = 0; i < list.Count; i++)
                        {
                            Debug.Log("update() shortestlist[i] " + list[i]);
                        }
                        arriveFlag = true;
                    }

                }
                redrawFlag = false;
            }
    }

    //바닥들 생성
    private void CreatePlanes()
    {
        if (this.planes != null) return;

        planes = new PlaneData[MAX_PLANES_NUM]; //80*60
    //    gamePlanes = new GameObject[MAX_PLANES_NUM];

        //-5.5, 3.5
        for (int i = 0; i < MAX_PLANES_NUM; i++)
        {
            planes[i].pos = new Vector3(i % ground_width, 0.0f, i / ground_width);
            planes[i].type = 0;

            //gamePlanes[i] = GameObject.Instantiate(normalPlane, Vector3.zero, Quaternion.identity) as GameObject;
            //gamePlanes[i].transform.position = new Vector3(i % ground_width, 0.0f, i / ground_width);
            //gamePlanes[i].transform.Rotate(new Vector3(90, -90, -90));
            //gamePlanes[i].GetComponent<PlaneInfo>().SetIndex(i);
            //gamePlanes[i].GetComponent<PlaneInfo>().planeType = 0;
           
        }

    }

    //pos 위치에 비콘 배치
    public void SetBeacon2(Vector3 pos)
    {
        int idx = FindPlane(pos);
        this.planes[idx].type = 5;
        PlaneData beacon = new PlaneData();
        beacon.type = 5;
        beacon.pos = pos;

        beaconList.Add(beacon);

        ////gameObject 부분
        //this.gamePlanes[idx].GetComponent<PlaneInfo>().planeType = 5;
        //GameObject gameBeacon = GameObject.Instantiate(normalShelf, Vector3.zero, Quaternion.identity);
        //gameBeacon.transform.position = pos;
        //beaconGameList.Add(gameBeacon);

        //beacons.add!
        playerInfo.SetBeacon(pos);

     
  //      Debug.Log("[SetBeacon2()] size : " + size);
    }

   

    //플레이어가 움직였을 때, 플레이어 위치를 갱신하는 함수
    public void UpdatePlayerPos(Vector3 playerPos)
    {
        //Debug.Log(playerPos);
        //Debug.Log(this.playerPos);
        if (planes == null || aStar == null) return;
     //   Debug.Log("update!");
        if (this.playerPos.Equals(playerPos)) return;
        
        this.playerPos = playerPos;

        //gamePlanes[playerPlaneIdx].GetComponent<PlaneInfo>().SetColor(Color.white);
        //gamePlanes[playerPlaneIdx].GetComponent<PlaneInfo>().planeType = 0;

        planes[playerPlaneIdx].type = 0;
        
        int plane_idx = FindPlane(playerPos);
        //gamePlanes[plane_idx].GetComponent<PlaneInfo>().planeType = 1;
        planes[plane_idx].type = 1;

        playerPlaneIdx = plane_idx;
        redrawFlag = true;

    }

    //위치에 해당하는 바닥의 인덱스를 구한다.실패시 -1 반환
    private int FindPlane(Vector3 playerPos)
    {
        if(this.planes == null)
        {
            CreatePlanes();
        }   
        for (int i = 0; i < MAX_PLANES_NUM; i++)
        {
            float x = planes[i].pos.x;
            float z = planes[i].pos.z;

            int difx = (int)(x - playerPos.x);
            int difz = (int)(z - playerPos.z);
            if (difx == 0 && difz == 0)
            {
                return i;
            }

        }
        return -1;
    }

 

    //pos 위치에 목적지 설정
    public void SetDest(Vector3 pos)
    {
        //목적지를 바꿀 때
        if (isDestSelected)
        {
            RestoreNormalShlef(dest);
        }

        int targetIndex = FindBeaconListIndex(pos);
        Debug.Log("SetDest() targetIndex : " + targetIndex);
       // gameDest = this.beaconGameList[targetIndex];
        PlaneData destData = this.beaconList[targetIndex];


        isDestSelected = true;
        //gameDest.transform.position = pos;
        dest = destData.pos;

//        dest_index = FindPlane(gameDest.transform.position);
  //      this.gamePlanes[dest_index].GetComponent<PlaneInfo>().planeType = 2;
    //    gameDest.transform.localScale = new Vector3(1, 5, 1);
      //  gameDest.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);

        isDestSelected = true;
        dest = destData.pos;
        dest_index = FindPlane(dest);

        //  Debug.Log("[SetDest()] dest_index : " + dest_index);
        destData.type = this.planes[dest_index].type = 2;
        redrawFlag = true;
    }

   

    //목적지 제거
    public void DeleteDest()
    {
        if (isDestSelected) {
            RestoreNormalShlef(dest);
        }
        isDestSelected = false;

        dest.Set(-1f, -1f, -1f);
  //      gameDest = null;
    }

    //pos 위치의 목적지를 일반 서가로 복원
    private void RestoreNormalShlef(Vector3 pos)
    {
        int prevIndex = FindPlane(pos);
        this.planes[prevIndex].type = 5;
    }

    int FindBeaconListIndex(Vector3 pos)
    {
        int size = this.beaconList.Count;
        Debug.Log("FindBeaconListIndex() pos : " + pos);
        Debug.Log("FindBeaconListIndex() count : " + size);
        for(int i = 0; i < size; i++)
        {
            Debug.Log("FindBeaconListIndex() beaconList[i].pos : " + beaconList[i].pos);
            if (beaconList[i].pos == pos)
            {
                return i;
            }
        }
        return -1;
    }

    //최적 경로 좌표들을 담고 있는 리스트를 반환한다.
    public List<Vector3> GetShortestPath()
    {
        return this.aStar.shortPath;
    }
    

    class AStar
    {
        struct PlaneNode
        {
            public int idx;

            //-9 : start
            public int parent;
            //F -> G+H
            //G -> 시작 노드에서 현재 노드와의 거리
            //H -> 현재 노드에서 목적지 노드와의 거리
            public float F, G, H;
        }

        private const int START = -9;
        //외부 클래스 객체의 멤버에 접근하기 위해 선언
        private PlaneManager outer;
        //바닥 갯수
        private const int MAX_PLANE_NUM = PlaneManager.MAX_PLANES_NUM;
        //A * 계산시 필요한 배열들
        private PlaneNode[] openList, closeList;
        //최단 경로 추적시 필요한 배열
        private int[] path;
        //openList, closeList에 대한 커서.
        private int openCursor, closeCursor;
        //목적지 인덱스와 플레이어 인덱스
        private int destIdx = -1, playerIdx;
        private int nextIdx = -1;

        //AR 팝업 조건. 목적지와 얼마나 가까워졌는가를 앞으로 가야 할 바닥의 갯수로 파악
        private const int LEFT_PLANE_NUMBER_FOR_AR_POPUP = 7;
        //AR 팝업 플래그
        private bool arFlag = false;

        //최적 경로를 이루는 바닥의 위치들의 리스트
        public List<Vector3> shortPath = new List<Vector3>();

        //멤버변수 초기화
        public AStar(PlaneManager outer)
        {
            this.outer = outer;
            openList = new PlaneNode[MAX_PLANES_NUM];
            closeList = new PlaneNode[MAX_PLANES_NUM];
            path = new int[MAX_PLANES_NUM];
            for (int i = 0; i < path.Length; i++) path[i] = -1;
            openCursor = -1;
            closeCursor = -1;
        }

        //A * 알고리즘을 적용하여 경로를 구하는 함수
        public int CalculatePath()
        {

            for (int i = 0; i < MAX_PLANES_NUM; i++)
            {
                int type = this.outer.planes[i].type;
           //     PlaneInfo pi = this.outer.gamePlanes[i].GetComponent<PlaneInfo>();
                //이전에 구한 경로를 지운다.
                if (type == 4)
                {
                    type = 0;
             //       pi.planeType = 0;
                    this.outer.planes[i].type = 0;
             //       pi.SetColor(Color.white);
                }
                else if (type == 1)
                {
               //     pi.planeType = 1;
                //    pi.SetColor(Color.white);
                }

            }

            //플레이어 인덱스 저장
            this.playerIdx = this.outer.playerPlaneIdx;
          //     Debug.Log("[CalculatePath()] : playerIndex : " + this.outer.planes[playerIdx].pos);

            //목적지 인덱스를 구한다. 실패시 -1 반환
            destIdx = FindDestination();
            if (destIdx == -1) return -1;

            //현재 위치에서 인접한 8개의 노드를 구한다.
            //8개의 노드 == 오른쪽 위, 위, 왼쪽 위, 왼쪽, 왼쪽 아래, 아래, 오른쪽 아래, 오른쪽 
            PlaneNode[] neighbors = FindNeighbors(this.playerIdx);
            for (int i = 0; i < 8; i++)
            {
                if (neighbors[i].idx != -1)
                {
                    //찾은 노드들을 오픈리스트에 넣는다.(A * 계산시 필요)
                    InsertIntoOpenList(neighbors[i]);
                }
            }


            PlaneNode startNode = new PlaneNode();
            startNode.parent = START;
            startNode.idx = this.playerIdx;
            startNode.G = 0f; startNode.H = CalculateH(this.playerIdx, this.destIdx);
            startNode.F = startNode.G + startNode.H;
            path[startNode.idx] = START;
            InsertIntoCloseList(startNode);

            //경로를 찾는다. find가 true가 돼서 반복문을 종료하면 찾은 것임
            bool find = false;
            while (!find && openCursor >= 0)
            {
                int minIdx = FindMinimumFNode();

                if (minIdx == -1)
                {
                    //       Debug.Log("[CalculatePath()] minIdx : " + minIdx);
                    break;
                }
                PlaneNode curNode = openList[minIdx];
                //Debug.Log("[CalculatePath()] pos : " + this.outer.planes[curNode.idx].pos);
                RemoveNodeInOpenList(minIdx);
                if (curNode.idx == this.destIdx)
                {
                    find = true;
                    InsertIntoCloseList(curNode);
                    break;
                }
                InsertIntoCloseList(curNode);
                neighbors = FindNeighbors(curNode.idx);
                Vector3 pos = this.outer.planes[curNode.idx].pos;
                for (int i = 0; i < neighbors.Length; i++)
                {

                    if (neighbors[i].idx != -1 && !IsClosed(neighbors[i].idx))
                    {
                        int target = IsOpened(neighbors[i].idx);
                        //열린목록에 있지 않다면
                        if (target == -1)
                        {
                            neighbors[i].parent = curNode.idx;
                            InsertIntoOpenList(neighbors[i]);
                        }
                        else
                        {
                            if (neighbors[i].G < openList[target].G)
                            {
                                neighbors[i].parent = curNode.idx;
                                RemoveNodeInOpenList(target);
                                InsertIntoOpenList(neighbors[i]);
                            }
                        }
                    }
                }
                //  Debug.Log("[CalculatePath()] openCursor : " + openCursor);
            }

            if (find == false)
            {
              //  Debug.Log("목적지를 찾을 수 없습니다...");
                return 0;
            }
            //찾은 경로를 path 배열에 넣는다.
            MakeShortestPath();
         //   this.outer.direction = GetDirection();
            return 1;
        }

        //최단 경로에 해당하는 바닥들을 표시한다.
        private void MakeShortestPath()
        {
            shortPath.Clear();
            int cur = closeList[closeCursor].idx;
            int size = closeCursor + 1;
            nextIdx = -1;

            //남은 바닥의 개수를 측정하는 변수
            int leftPlaneSize = 0;
            //출발지는 경로 색칠에서 제외
            for (int i = 0; i < size; i++)
            {
                

                if (cur == this.playerIdx)
                {
                    // Debug.Log(shortPath.Count);
                    this.outer.planes[this.destIdx].type = 2;
                    //nextIdx = cur;
                    //             Debug.Log("[MakeShortestPath()] 남은 바닥 갯수 : " + (leftPlaneSize - 1));
                    if (!arFlag && leftPlaneSize - 1 == LEFT_PLANE_NUMBER_FOR_AR_POPUP)
                    {

                        Debug.Log("AR PopUP!!!!");
                        arFlag = true;
                    }
                    return;
                }
                //최단 경로에 해당하는 바닥의 planeType을 4로 두고, 나중에 최단 경로를 출력할 때 4로 된 것들만 출력한다.
                this.outer.planes[cur].type = 4;
            //    this.outer.gamePlanes[cur].GetComponent<PlaneInfo>().planeType = 4;
                leftPlaneSize++;

                if (size - i == 10)
                {
                    nextIdx = cur;
                }
                cur = path[cur];
            }
        }

        //목적지에 해당하는 인덱스를 반환.
        private int FindDestination()
        {
            for (int i = 0; i < MAX_PLANES_NUM; i++)
            {
                int type = this.outer.planes[i].type;
                if (type == 2)
                {
                    //      Debug.Log("[FindDestination()] found! : " + i);
                    return i;
                }
            }
            return -1;
        }

        //A * 계산시 필요한 함수
        private float CalculateH(int curIdx, int destIdx)
        {
            Vector3 s = this.outer.planes[curIdx].pos;
            Vector3 d = this.outer.planes[destIdx].pos;
            float h = Mathf.Abs(s.x - d.x) + Mathf.Abs(s.z - d.z);
            return h;
        }
        //A * 계산시 필요한 함수
        private float CalculateG(int curIdx, int startIdx)
        {
            Vector3 s = this.outer.planes[startIdx].pos;
            Vector3 c = this.outer.planes[curIdx].pos;
            float g = Mathf.Sqrt(Mathf.Abs(s.x - c.x) * Mathf.Abs(s.x - c.x) + Mathf.Abs(s.z - c.z) * Mathf.Abs(s.z - c.z));

            return g;
        }

        //전체 바닥들의 범위 안에 있고, 장애물이 아닌 이웃 노드를 찾는다.
        private PlaneNode[] FindNeighbors(int baseIdx)
        {
            int[] distX = { -1, -1, -1, 0, 1, 1, 1, 0 };
            int[] distY = { -1, 0, 1, 1, 1, 0, -1, -1 };
            PlaneNode[] neighbors = new PlaneNode[8];
            for (int i = 0; i < 8; i++) neighbors[i].idx = -1;

            int w = this.outer.ground_width;
            int h = this.outer.ground_height;
            int bx = baseIdx % w;
            int by = baseIdx / w;
            for (int i = 0; i < 8; i++)
            {
                int nx = bx + distX[i];
                int ny = by + distY[i];
                if (nx >= 0 && ny >= 0 && nx < w && ny < h)
                {
                    int newIdx = ny * w + nx;
                    int planeType = this.outer.planes[newIdx].type;
                    Vector3 pos = this.outer.planes[newIdx].pos;

                    if (/*!outer.isBeaconLine(pos) && */planeType == 0 || planeType == 2)
                    {
                        neighbors[i].idx = newIdx;
                        neighbors[i].H = CalculateH(newIdx, this.destIdx);
                        neighbors[i].G = CalculateG(newIdx, baseIdx);
                        neighbors[i].F = neighbors[i].G + neighbors[i].H;
                        neighbors[i].parent = baseIdx;
                    }
                }
            }
            return neighbors;
        }

        //A * 계산시 필요한 함수
        private int FindMinimumFNode()
        {
            int minIdx = -1;
            int size = openCursor + 1;
            if (size > MAX_PLANES_NUM) return -1;
            float minF = 99999f;
            float minH = 99999f;
            for (int i = 0; i < size; i++)
            {
                if (minF > openList[i].F || (minF == openList[i].F && minH > openList[i].H))
                {
                    minF = openList[i].F;
                    minH = openList[i].H;
                    minIdx = i;
                }
            }

            return minIdx;
        }

        //A * 계산시 필요한 함수
        private void RemoveNodeInOpenList(int idx)
        {
            if (openCursor >= 0)
            {
                openList[idx] = openList[this.openCursor];
                this.openCursor--;
            }
        }
        //A * 계산시 필요한 함수
        private void InsertIntoOpenList(PlaneNode node)
        {
            if (this.openCursor < MAX_PLANES_NUM)
            {
                path[node.idx] = node.parent;
                openList[++(this.openCursor)] = node;
            }
        }
        //A * 계산시 필요한 함수
        private void InsertIntoCloseList(PlaneNode node)
        {
            if (this.closeCursor < MAX_PLANES_NUM)
            {
                closeList[++(this.closeCursor)] = node;
            }
        }
        //A * 계산시 필요한 함수
        private bool IsClosed(int idx)
        {
            for (int i = 0; i < this.closeCursor; i++)
            {
                if (this.closeList[i].idx == idx)
                {
                    return true;
                }
            }
            return false;
        }
        //A * 계산시 필요한 함수
        private int IsOpened(int idx)
        {
            for (int i = 0; i < this.openCursor + 1; i++)
            {
                if (this.openList[i].idx == idx)
                {
                    return i;
                }
            }
            return -1;
        }

        //최단 경로를 그린다.
        public void Draw()
        {

            for (int i = 0; i < MAX_PLANE_NUM; i++)
            {
                PlaneData pd = this.outer.planes[i];
                if (pd.type == 4)
                {
                    shortPath.Add(pd.pos);
                }
            }
            this.outer.direction = GetDirection();
            ClearLists();
        }

        //멤버변수 초기화
        public void ClearLists()
        {
            System.Array.Clear(openList, 0, openCursor + 1);
            System.Array.Clear(closeList, 0, closeCursor + 1);
            for (int i = 0; i < openCursor + 1; i++)
            {
                openList[i].idx = -1;
                openList[i].parent = -1;
            }
            for (int i = 0; i < closeCursor + 1; i++)
            {
                closeList[i].idx = -1;
                closeList[i].parent = -1;
            }

            openCursor = -1;
            closeCursor = -1;
        }

        public Vector3 GetDirection()
        {
           
            if (this.nextIdx != -1)
            {
                Vector3 target = this.outer.planes[nextIdx].pos;

                return target;
            }

            return new Vector3(-1f, -1f, -1f);
        }

    }

}
