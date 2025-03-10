package com.example.greengrowtechapp.Handlers;

import com.google.gson.annotations.SerializedName;

public class Pot {

    @SerializedName("potId")
    private int potId;

    @SerializedName("potName")
    private String potName;

    @SerializedName("potType")
    private int potType;

    @SerializedName("plantName")
    private String plantName;

    @SerializedName("userId")
    private int userId;

    @SerializedName("hasCamera")
    private boolean hasCamera;

    @SerializedName("pictReq")
    private boolean pictReq;

    @SerializedName("pumpStatus")
    private boolean pumpStatus;

    @SerializedName("greenHouseStatus")
    private boolean greenHouseStatus;

    @SerializedName("greenHouseTemperature")
    private double greenHouseTemperature;

    @SerializedName("greenHouseHumidity")
    private double greenHouseHumidity;

    @SerializedName("greenHousePressure")
    private double greenHousePressure;

    @SerializedName("potPotassium")
    private double potPotassium;

    @SerializedName("potPhospor")
    private double potPhosphorus;

    @SerializedName("potNitrogen")
    private double potNitrogen;

    public Pot(int potId, String potName, int potType, String plantName, int userId, boolean hasCamera, boolean pictReq,
               boolean pumpStatus, boolean greenHouseStatus, double greenHouseTemperature, double greenHouseHumidity,
               double greenHousePressure, double potPotassium, double potPhosphorus, double potNitrogen) {
        this.potId = potId;
        this.potName = potName;
        this.potType = potType;
        this.plantName = plantName;
        this.userId = userId;
        this.hasCamera = hasCamera;
        this.pictReq = pictReq;
        this.pumpStatus = pumpStatus;
        this.greenHouseStatus = greenHouseStatus;
        this.greenHouseTemperature = greenHouseTemperature;
        this.greenHouseHumidity = greenHouseHumidity;
        this.greenHousePressure = greenHousePressure;
        this.potPotassium = potPotassium;
        this.potPhosphorus = potPhosphorus;
        this.potNitrogen = potNitrogen;
    }

    // Getters
    public int getPotId() {
        return potId;
    }

    public String getPotName() {
        return potName;
    }

    public int getPotType() {
        return potType;
    }

    public String getPlantName() {
        return plantName;
    }

    public int getUserId() {
        return userId;
    }

    public boolean hasCamera() {
        return hasCamera;
    }

    public boolean isPictReq() {
        return pictReq;
    }

    public boolean isPumpStatus() {
        return pumpStatus;
    }

    public boolean isGreenHouseStatus() {
        return greenHouseStatus;
    }

    public double getGreenHouseTemperature() {
        return greenHouseTemperature;
    }

    public double getGreenHouseHumidity() {
        return greenHouseHumidity;
    }

    public double getGreenHousePressure() {
        return greenHousePressure;
    }

    public double getPotPotassium() {
        return potPotassium;
    }

    public double getPotPhosphorus() {
        return potPhosphorus;
    }

    public double getPotNitrogen() {
        return potNitrogen;
    }

    public String getPlant() {
        return plantName;
    }
}
