package com.example.myapplication;
import com.google.gson.annotations.SerializedName;

public class Plant {
    @SerializedName("plantId")
    private int plantId;

    @SerializedName("plantName")
    private String plantName;
    @SerializedName("plantGroup")
    private String plantGroup;
    @SerializedName("waterPref")
    private String waterPref;
    @SerializedName("lifeCycle")
    private String lifeCycle;
    @SerializedName("plantHabit")
    private String plantHabit;
    @SerializedName("flowerColor")
    private String flowerColor;
    @SerializedName("phMinVal")
    private int phMinVal;
    @SerializedName("phMaxVal")
    private int phMaxVal;
    @SerializedName("minTemp")
    private int minTemp;
    @SerializedName("sunReq")
    private int sunReq;
    @SerializedName("plantHeight")
    private int plantHeight;
    @SerializedName("plantWidth")
    private int plantWidth;
    @SerializedName("fruitingTime")
    private int fruitingTime;
    @SerializedName("flowerTime")
    private int flowerTime;
    public int getPlantId() {
        return plantId;
    }

    public String getPlantName() {
        return plantName;
    }

    public String getPlantGroup() {
        return plantGroup;
    }

    public String getWaterPref() {
        return waterPref;
    }

    public String getLifeCycle() {
        return lifeCycle;
    }

    public String getPlantHabit() {
        return plantHabit;
    }

    public String getFlowerColor() {
        return flowerColor;
    }

    public int getPhMinVal() {
        return phMinVal;
    }

    public int getPhMaxVal() {
        return phMaxVal;
    }

    public int getMinTemp() {
        return minTemp;
    }

    public int getSunReq() {
        return sunReq;
    }

    public int getPlantHeight() {
        return plantHeight;
    }

    public int getPlantWidth() {
        return plantWidth;
    }

    public int getFruitingTime() {
        return fruitingTime;
    }

    public int getFlowerTime() {
        return flowerTime;
    }
    // Add other fields and getters/setters as needed
}