package com.ray_lucid.blemeasure;

import java.util.ArrayList;

/**
 * Created by ray_lucid on 2018-02-05.
 */

public class Beacon {
    public String name;
    public double distance;
    public double estimatedRSSI;//calculated rssi
    public double errorCovarianceRSSI;//calculated covariance

    public Beacon(String name, double RSSI)
    {
        this.name = name;
        this.distance = 0;
        this.estimatedRSSI = RSSI;
        this.errorCovarianceRSSI = 1;
    }
}
