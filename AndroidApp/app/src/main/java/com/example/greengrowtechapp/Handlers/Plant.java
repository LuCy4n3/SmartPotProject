package com.example.greengrowtechapp.Handlers;
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
    @SerializedName("maxTemp")
    private int maxTemp;
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

    public Plant(int plantId, String plantName, String plantGroup, String waterPref, String lifeCycle, String plantHabit, String flowerColor, int phMinVal, int phMaxVal, int minTemp,int maxTemp, int sunReq, int plantHeight, int plantWidth, int fruitingTime, int flowerTime) {
        this.plantId = plantId;
        this.plantName = plantName;
        this.plantGroup = plantGroup;
        this.waterPref = waterPref;
        this.lifeCycle = lifeCycle;
        this.plantHabit = plantHabit;
        this.flowerColor = flowerColor;
        this.phMinVal = phMinVal;
        this.phMaxVal = phMaxVal;
        this.minTemp = minTemp;
        this.maxTemp = maxTemp;
        this.sunReq = sunReq;
        this.plantHeight = plantHeight;
        this.plantWidth = plantWidth;
        this.fruitingTime = fruitingTime;
        this.flowerTime = flowerTime;
    }

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