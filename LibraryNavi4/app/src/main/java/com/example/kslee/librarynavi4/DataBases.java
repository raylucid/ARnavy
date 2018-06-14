package com.example.kslee.librarynavi4;

import android.provider.BaseColumns;

/**
 * Created by wjstk on 2018-05-14.
 */

public final class DataBases {
    public static final class CreateDB implements BaseColumns{

        public static final String TITLE = "서명";
        public static final String PUBLISHER = "저자";
        public static final String AUTHOR = "출판사";
        public static final String BOOKNUM = "서가";
        public static final String _TABLENAME = "도서정보";
        public static final String _CREATE =
            "create table "+_TABLENAME+"("
                    +_ID+" integer primary key autoincrement, "
                    +TITLE  +" text not null , "
                    +PUBLISHER+" text not null , "
                    +AUTHOR+" text not null , "
                    +BOOKNUM+" text not null );";
}
}
