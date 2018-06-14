package com.ray_lucid.blemeasure;

import android.app.Activity;
import android.bluetooth.BluetoothAdapter;
import android.content.Intent;
import android.os.Bundle;

import com.unity3d.player.UnityPlayer;

public class BLE extends Activity {
    private static final int REQUEST_ENABLE_BT = 2;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_ble);
        Intent enableIntent = new Intent(BluetoothAdapter.ACTION_REQUEST_ENABLE);
        startActivityForResult(enableIntent, REQUEST_ENABLE_BT);
    }
    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        switch (requestCode) {
            case REQUEST_ENABLE_BT:
                if(resultCode == Activity.RESULT_CANCELED)
                    System.exit(0);
                else
                    UnityPlayer.UnitySendMessage("csTank", "checkBluetooth", "");
                break;
        }
        this.finish();
    }
}
