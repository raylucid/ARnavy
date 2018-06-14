package com.example.kslee.librarynavi4;

import android.app.Activity;
import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

import java.util.List;

/**
 * Created by wjstk on 2018-05-09.
 */

public class MyAdapter extends BaseAdapter{
    private List<BookInfo> list;
    private LayoutInflater inflater;
    Activity activity;
    private ViewHolder holder;

    public  MyAdapter(){

    }

    public MyAdapter(Context c,List<BookInfo> array)
    {
     //  this.list = list;
       // this.activity = activity;
       // inflater = (LayoutInflater) activity.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
        inflater = LayoutInflater.from(c);
        list = array;
    }

    @Override
    public int getCount() {
        return list.size();
    }

    @Override
    public  Object getItem(int position)
    {
        return  list.get(position);
    }

    @Override
    public long getItemId(int position) {
        return list.get(position).getId();
    }

    @Override
    public View getView(int position, View convertView, ViewGroup parent) {
       /* if (convertView == null) {
            convertView = inflater.inflate(R.layout.informaition_book, parent, false);
            LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MATCH_PARENT, ViewGroup.LayoutParams.MATCH_PARENT);
            convertView.setLayoutParams(layoutParams);
        }

        ImageView imageView = (ImageView) convertView.findViewById(R.id.cover);
        Bitmap albumImage = getAlbumImage(activity, Integer.parseInt((list.get(position)).getId()), 170);
        if(albumImage!=null && imageView != null)
            imageView.setImageBitmap(albumImage);

        TextView title = (TextView) convertView.findViewById(R.id.title);
        title.setText(list.get(position).getTitle());

        return convertView;*/
       View v= convertView;

        if (v == null) {
            holder = new ViewHolder();
            v = inflater.inflate(R.layout.informaition_book, null);
            holder.title = (TextView)v.findViewById(R.id.tvtitle);
            holder.author = (TextView)v.findViewById(R.id.tvauthor);
            holder.publisher = (TextView)v.findViewById(R.id.tvpublisher);
            holder.booknum = (TextView)v.findViewById(R.id.tvbooknum);
            v.setTag(holder);
        } else {
            holder = (ViewHolder)v.getTag();
        }

        //InfoClass를 생성하여 각 뷰의 포지션에 맞는 데이터를 가져옴
        BookInfo info = list.get(position);

        //리스트뷰의 아이템에 맞는 String값을 입력
        holder.title.setText(info.title);
        holder.author.setText(info.author);
        holder.publisher.setText(info.publisher);
        holder.booknum.setText(info.booknum);
        return v;
    }



    private static final BitmapFactory.Options options = new BitmapFactory.Options();



    private static Bitmap getAlbumImage(Context context, int album_id, int MAX_IMAGE_SIZE) {


        // NOTE: There is in fact a 1 pixel frame in the ImageView used to
        // display this drawable. Take it into account now, so we don't have to
        // scale later. 여기는 DB에서 책 사진 불러오는 함수
       /* ContentResolver res = context.getContentResolver();
        Uri uri = Uri.parse("content://media/external/audio/albumart/" + album_id);
        if (uri != null) {
            ParcelFileDescriptor fd = null;
            try {
                fd = res.openFileDescriptor(uri, "r");
                options.inJustDecodeBounds = true;
                BitmapFactory.decodeFileDescriptor(
                        fd.getFileDescriptor(), null, options);
                int scale = 0;
                if (options.outHeight > MAX_IMAGE_SIZE || options.outWidth > MAX_IMAGE_SIZE) {
                    scale = (int) Math.pow(2, (int) Math.round(Math.log(MAX_IMAGE_SIZE / (double) Math.max(options.outHeight, options.outWidth)) / Math.log(0.5)));
                }
                options.inJustDecodeBounds = false;
                options.inSampleSize = scale;



                Bitmap b = BitmapFactory.decodeFileDescriptor(
                        fd.getFileDescriptor(), null, options);

                if (b != null) {
                    // finally rescale to exactly the size we need
                    if (options.outWidth != MAX_IMAGE_SIZE || options.outHeight != MAX_IMAGE_SIZE) {
                        Bitmap tmp = Bitmap.createScaledBitmap(b, MAX_IMAGE_SIZE, MAX_IMAGE_SIZE, true);
                        b.recycle();
                        b = tmp;
                    }
                }

                return b;
            } catch (FileNotFoundException e) {
            } finally {
                try {
                    if (fd != null)
                        fd.close();
                } catch (IOException e) {
                }
            }
        }*/
        return null;
    }


    private class ViewHolder {
        TextView title;
        TextView author;
        TextView publisher;
        TextView booknum;
    }
}


