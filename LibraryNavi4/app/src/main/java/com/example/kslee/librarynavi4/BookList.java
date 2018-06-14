package com.example.kslee.librarynavi4;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.content.res.AssetManager;
import android.database.Cursor;
import android.database.SQLException;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.preference.DialogPreference;
import android.support.v7.app.AlertDialog;
import android.support.v7.app.AppCompatActivity;
import android.database.sqlite.SQLiteDatabase;
import android.database.sqlite.SQLiteOpenHelper;
import android.util.Log;
import android.view.Menu;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.EditText;
import android.widget.ListView;
import android.app.Dialog;
import android.content.DialogInterface;
import android.widget.Spinner;
import android.widget.TextView;
import android.widget.Toast;

import org.w3c.dom.Text;

import java.io.BufferedInputStream;
import java.io.BufferedOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.util.ArrayList;
import java.util.List;

import static android.content.ContentValues.TAG;

/**
 * Created by wjstk on 2018-05-09.
 */
import com.unity3d.player.UnityPlayer;
import com.unity3d.player.UnityPlayerActivity;

public class BookList extends Activity {

    private ListView listView;
    private EditText mDisplay;
    private Cursor mCursor;
    private BookInfo mBookinfo;
    public static ArrayList<BookInfo> list;
    private MyAdapter myAdapter;
    private SQLiteDatabase mDatabase;
    private DbOpenHelper mDbOpenHelper;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.list_book);

        try {
         //   boolean bResult = isCheckDB(this);
            Log.d("kslee", "before copyDB()");
            copyDB(this);
            mDatabase = this.openOrCreateDatabase("test.db", Context.MODE_PRIVATE, null);
            getBookList();
        } catch (Exception e) {

        }

        UnityPlayer.UnitySendMessage("csTank", "SetDestFromAndroid", "onCreate()");
    }
    public void getBookList(){
        list = new ArrayList<>();
        TextView textView = (TextView)findViewById(R.id.edit1);
        String sld = textView.getText().toString();

        Log.d("getBookList()", "mDatabase : " + mDatabase.toString());
        Cursor cursor = mDatabase.rawQuery("select * from 도서정보 where 서명 like '% " + sld+"%'", null);

        Log.d("getBookList()", "cursor : " + cursor);
        if (cursor != null) {
            int count = 0;
            while (cursor.moveToNext()) {
                BookInfo bookInfo = new BookInfo();
                bookInfo.setId(cursor.getInt(cursor.getColumnIndex("번호")));
                bookInfo.setTitle(cursor.getString(cursor.getColumnIndex("서명")));
                bookInfo.setAuthor(cursor.getString(cursor.getColumnIndex("저자")));
                bookInfo.setPublisher(cursor.getString(cursor.getColumnIndex("출판사")));
                bookInfo.setBooknum(cursor.getString( cursor.getColumnIndex("서가")));
                list.add(bookInfo);
                count++;
            }
            Log.d("getbookList()", "count : " + count);
            cursor.close();
        }

    }

    public void onClick(View v){
        Toast.makeText(this, "onClick from android", Toast.LENGTH_SHORT).show();
        UnityPlayer.UnitySendMessage("testObj", "TestFunc", "onClick!");
        getBookList();
        listView = (ListView)findViewById(R.id.totallist);
        final MyAdapter adapter = new MyAdapter(this, list);
        mDisplay = (EditText) findViewById(R.id.edit1);
        if(adapter != null) {
            listView.setAdapter(adapter);
            listView.setOnItemClickListener(new AdapterView.OnItemClickListener() {
                @Override
                public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                    BookInfo bi = (BookInfo) adapter.getItem(position);
                    String shelfNumber= bi.getBooknum();
                    Log.d("onItemClick()", "bookNum : " + shelfNumber);
                    String query = "select * from shelf where number = " + shelfNumber + "";
                    Log.d("onItemClick()", query);
                    Cursor cursor = mDatabase.rawQuery(query, null);
                    cursor.moveToFirst();

                    String pos_x = cursor.getString(2);
                    String pos_z = cursor.getString(3);

                    String result = "(" + pos_x + ", " + " 0, " + pos_z + ")";

                    UnityPlayer.UnitySendMessage("testObj", "TestFunc", result);
                }
            });
        }

    }

    @Override
    public void onBackPressed() {
        super.onBackPressed();
        Log.d("onBackPressed()", "onBackPressed()");
        Intent intent = getIntent();
        intent.putExtra("terminated", "Y");
        setResult(RESULT_CANCELED, intent);
        finish();
    }

    public boolean isCheckDB(Context context){
        String filePath = "/data/data" + "/com.example.astar3d" + "/databases" + "/test.db";
        File file = new File(filePath);

        if(file.exists()){
            return true;
        }
        return false;
    }

    public void copyDB(Context context){
        Log.d("kslee", "copyDB");
        AssetManager manager = getResources().getAssets();

        try{
            String folderPath = "/data/data/" + "com.example.astar3d" + "/databases";
            String filePath = folderPath + "/test.db";
            File folder = new File(folderPath);
            File file = new File(filePath);

            FileOutputStream fos = null;

            if(folder.exists()){
                Log.d("copyDB()", "folder exist : " + folderPath);
            }else{
                folder.mkdirs();
            }
            if(file.exists()){
                file.delete();
                file.createNewFile();
            }

            InputStream is = manager.open("test.db");


            Log.d("kslee", "before fileoutputstream");
            fos = new FileOutputStream(file);
            Log.d("kslee", "after fileoutputstream");
            int read = -1;
            byte[] buffer = new byte[1024];
            while((read = is.read(buffer, 0, 1024)) != -1){

                fos.write(buffer, 0, read);
                Log.d("kslee", "[copyDB()] read : " + read);
            }

            fos.flush();
            fos.close();
            is.close();

        }catch(IOException e){
            Log.d("kslee", "Error : " + e.getMessage());
        }
    }

    public void ShowToast(String msg){
        Toast.makeText(this, msg, Toast.LENGTH_SHORT).show();
    }

    public static void CallActivity(Activity activity){
        Intent intent = new Intent(activity, BookList.class);
        activity.startActivity(intent);
    }
}




