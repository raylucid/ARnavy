package com.example.kslee.librarynavi4;

import java.io.Serializable;

/**
 * Created by wjstk on 2018-05-09.
 */

public class BookInfo {

    public int id;
    public String title;
    public String author;
    public String publisher;
    public String booknum;

    public  BookInfo(){

    }

    public  BookInfo(int id,String title,String publisher,String author, String booknum)
    {
        this.id = id;
        this.title = title;
        this.author = author;
        this.publisher = publisher;
        this.booknum = booknum;
    }

    public int getId(){
        return id;
    }
    public void setId(int id){
        this.id = id;
    }

    public  String getTitle(){
        return  title;
    }
    public void setTitle(String title)
    {
        this.title = title;
    }

    public  String getPublisher(){
        return  publisher;
    }
    public void setPublisher(String publisher)
    {
        this.publisher = publisher;
    }

    public  String getAuthor(){
        return  author;
    }
    public void setAuthor(String author) {
        this.author = author;
    }

    public  String getBooknum(){
        return  booknum;
    }
    public void setBooknum(String booknum) {
        this.booknum = booknum;
    }



}