package com.ray_lucid.blemeasure;

import android.app.Activity;
import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.os.Build;
import android.os.Environment;
import android.util.Log;
import android.widget.Toast;
import com.unity3d.player.UnityPlayer;

import java.io.BufferedWriter;
import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileWriter;
import java.io.IOException;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;

/**
 * Created by ray_lucid on 2018-01-29.
 */

public class BLEPluginClass {
    final Activity activity = UnityPlayer.currentActivity;
    private static BLEPluginClass m_instance;
    final static String uuid = "E2C56DB5-DFFB-48D2-B060-D0F5A71096E0";
    final static double n = 2;
    final static double TxPower = -59;
    final static double processNoise = 0.125;//Process noise
    final static double measurementNoise = 0.8;//Measurement noise

    BluetoothAdapter mAdapter;
    boolean isScanning;
    Thread ScanThread;
    ArrayList<Beacon> BeaconList;
    int count = 0;
    Comparator<Beacon> comparator = new Comparator<Beacon>() {
        @Override
        public int compare(Beacon o1, Beacon o2) {
            int result = 0;
            if(o1.distance == o2.distance)
                result =  0;
            else if(o1.distance < o2.distance)
                result = -1;
            else if(o1.distance > o2.distance)
                result = 1;
            return result;
        }
    };
    private final BroadcastReceiver mReceiver = new BroadcastReceiver() {
        public void onReceive(Context context, Intent intent) {
            String action = intent.getAction();
            // When discovery finds a device
            if (BluetoothDevice.ACTION_FOUND.equals(action)) {
                // Get the BluetoothDevice object from the Intent
                BluetoothDevice device = intent.getParcelableExtra(BluetoothDevice.EXTRA_DEVICE);
                // Add the name and address to an array adapter to show in a ListView
                String name = device.getName();
                if(name != null && name.length() > 6 && name.substring(4, 7).equals("lib"))
                {
                    double RSSI = intent.getShortExtra(BluetoothDevice.EXTRA_RSSI, Short.MIN_VALUE);
                    for(int i = 0; i < BeaconList.size(); i++)
                    {
                        if(BeaconList.get(i).name.substring(0,3).equals(name.substring(0,3)))
                        {
                            Beacon beacon = BeaconList.get(i);
                            //https://github.com/fgroch/beacon-rssi-resolver/blob/master/src/main/java/tools/blocks/filter/KalmanFilter.java
                            double priorRSSI;
                            double kalmanGain;
                            double priorErrorCovarianceRSSI;
                            priorRSSI = beacon.estimatedRSSI;
                            priorErrorCovarianceRSSI = beacon.errorCovarianceRSSI + processNoise;
                            kalmanGain = priorErrorCovarianceRSSI / (priorErrorCovarianceRSSI + measurementNoise);
                            beacon.estimatedRSSI = priorRSSI + (kalmanGain * (RSSI - priorRSSI));
                            beacon.errorCovarianceRSSI = (1 - kalmanGain) * priorErrorCovarianceRSSI;
                            return;
                        }
                    }
                    BeaconList.add(new Beacon(name, RSSI));
                }
            }
        }
    };
    public static BLEPluginClass instance()
    {
        if(m_instance == null)
        {
            m_instance = new BLEPluginClass();
        }
        return m_instance;
    }
    private void ShowToast(final String toastStr)
    {
        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                Toast.makeText(activity, toastStr, Toast.LENGTH_LONG).show();
            }
        });
    }
    private void AndroidVersionCheck(String objName, String objMethod)
    {
        UnityPlayer.UnitySendMessage(objName, objMethod, Build.VERSION.RELEASE);
    }
    private void init(String str)
    {
        isScanning = false;
        BeaconList = new ArrayList<Beacon>();
        // Register the BroadcastReceiver
        IntentFilter filter = new IntentFilter(BluetoothDevice.ACTION_FOUND);
        activity.registerReceiver(mReceiver, filter); // Don't forget to unregister during onDestroy
        checkBluetooth("");
    }
    private void stopscan(String str)
    {
        if (isScanning) {
            ScanThread.interrupt();
            isScanning = false;
        }
        activity.unregisterReceiver(mReceiver);
    }
    public void startscan()
    {
        ScanThread = new Thread("Scan Thread"){
            public void run()
            {
                while (isScanning)
                {
                    mAdapter.cancelDiscovery();
                    mAdapter.startDiscovery();
                    try{
                        ScanThread.sleep(100);
                    }catch (Exception ex){}
                    count++;
                    if(count !=0 && count%10 == 0)
                    {
                        for(int i = 0; i < BeaconList.size(); i++)
                        {
                            Beacon beacon = BeaconList.get(i);
                            double RSSI = beacon.estimatedRSSI;
                            beacon.distance = Math.pow(10, (TxPower - RSSI) / (10*n));
                        }
                        Collections.sort(BeaconList, comparator);
                        UnityPlayer.UnitySendMessage("csTank", "ClearBeaconList", "");
                        for(int i = 0; i < BeaconList.size(); i++)
                        {
                            Beacon beacon = BeaconList.get(i);
                            String msg = beacon.name.substring(0,3) + "," + beacon.name.substring(2,3) + "," + beacon.estimatedRSSI + "," + beacon.distance;
                            UnityPlayer.UnitySendMessage("csTank", "AddBeacon", msg);
                        }
                    }
                }
            }
        };
        ScanThread.start();
    }
    private void checkBluetooth(String str) {
        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                mAdapter = BluetoothAdapter.getDefaultAdapter();
                if(mAdapter == null)
                {
                    activity.finish();
                }
                else if(!mAdapter.isEnabled())
                {
                    showBLEDialog();
                }
                else {
                    if (!isScanning) {
                        isScanning = true;
                        startscan();
                    }
                }
            }
        });
    }
    private void showBLEDialog() {
        Intent intent = new Intent();
        intent.setClass(activity, BLE.class);
        activity.startActivity(intent);
    }
}
