using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System.Data;
//using Mono.Data.SqliteClient;

public class DBTest : MonoBehaviour {

 //   IDbConnection _dbc;
 //   IDbCommand _dbcm;
 //   IDataReader _dbr;
 //   // Use this for initialization
 //   void Start () {
 //       //if (InitDB("URI=file:" + "test.db"))
 //       //{
 //       //    //int number = GetShelfNumberByBookName("Hello");
 //       //    //Debug.Log(number);
 //       //    //CloseDB();
 //       //}
 //   }
	
	//// Update is called once per frame
	//void Update () {
		
	//}

 //   //sqlite를 사용하기 위해 초기화 작업. 성공시 true를 반환
 //   public bool InitDB(string path)
 //   {
 //       string _constr = path; // ;

 //       _dbc = new Mono.Data.SqliteClient.SqliteConnection(_constr);
 //       _dbc.Open();
 //       _dbcm = _dbc.CreateCommand();
 //       return true;
 //   }

 //   public int GetShelfNumberByBookName(string bookName) //책 제목으로 서가 번호(인덱스)를 찾는다.
 //   {
 //       string query = string.Format("Select shelf_number from book where name = '{0}'", bookName);
 //       Debug.Log(query);
 //       _dbcm.CommandText = query;
 //       _dbr = _dbcm.ExecuteReader();
 //       if (_dbr.Read())
 //       {
 //           return _dbr.GetInt32(0);
 //       }
 //       else { return -1; }
 //   }

 //  public int GetShelfIndexByBeaconsName(string beaconName)
 //   {
 //       string query = string.Format("select index from shelf where name = '{0}'", beaconName);
 //       Debug.Log(query);
 //       _dbcm.CommandText = query;
 //       _dbr = _dbcm.ExecuteReader();
 //       if (_dbr.Read())
 //       {
 //           return _dbr.GetInt32(0);
 //       }
 //       else
 //       {
 //           return -1;
 //       }
 //   }


 //   //sqlite 종료 처리
 //   public void CloseDB()
 //   {
 //       _dbcm.Dispose();
 //       _dbcm = null;
 //       _dbc.Close();
 //       _dbc = null;
 //   }
}
