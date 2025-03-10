package com.example.greengrowtechapp.Handlers;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.io.Serializable;
import java.util.ArrayList;
import java.util.List;

public class JSONresponseHandler implements Serializable {
    private int index, type;
    private boolean sera, pompa,camera;
    private double temp, lumi, hum, pot, pho, ni, tempExt, humExt;
    private String plantName;
    private String error = "no error";


    public void setError(String error) {
        this.error = error;
    }

    public List<Plant> parsePlantDataList(JSONArray jsonArray) throws JSONException {
        List<Plant> plantList = new ArrayList<>();

        for (int i = 0; i < jsonArray.length(); i++) {
            JSONObject plantObject = jsonArray.getJSONObject(i);

            Plant plant = new Plant(
                    plantObject.optString("plantName", ""),
                    plantObject.optString("plantGroup", ""),
                    plantObject.optString("waterPref", ""),
                    plantObject.optString("lifeCycle", ""),
                    plantObject.optString("plantHabit", ""),
                    plantObject.optString("flowerColor", ""),
                    plantObject.optInt("phMinVal", 0),
                    plantObject.optInt("phMaxVal", 0),
                    plantObject.optInt("minTemp", 0),
                    plantObject.optInt("maxTemp", 0),
                    plantObject.optInt("sunReq", 0),
                    plantObject.optInt("plantHeight", 0),
                    plantObject.optInt("plantWidth", 0),
                    plantObject.optInt("fruitingTime", 0),
                    plantObject.optInt("flowerTime", 0),
                    plantObject.optString("soilType", ""),
                    plantObject.optInt("nitrogen", 0),
                    plantObject.optInt("phosphorus", 0),
                    plantObject.optInt("potassium", 0),
                    plantObject.optInt("spacing", 0),
                    plantObject.optInt("humidity", 0)
            );


            plantList.add(plant);
        }

        return plantList;
    }
    public List<Pot> parsePotDataList(JSONArray jsonArray) throws JSONException {
        List<Pot> potList = new ArrayList<>();

        for (int i = 0; i < jsonArray.length(); i++) {
            JSONObject potObject = jsonArray.getJSONObject(i);

            Pot pot = new Pot(
                    potObject.optInt("potId", 0), // Index
                    potObject.optString("potName",""),
                    potObject.optInt("potType", 0), // Pot type
                    potObject.optString("plantName", "Unknown"), // Plant name
                    potObject.optInt("userId",0),
                    potObject.optBoolean("hasCamera", false), // Camera presence
                    potObject.optBoolean("pictReq",false),
                    potObject.optBoolean("pumpStatus", false), // Pump status
                    potObject.optBoolean("greenHouseStatus", false), // Greenhouse status
                    potObject.optDouble("potPotassium", 0.0), // Potassium level
                    potObject.optDouble("potPhosphor", 0.0), // Phosphor level
                    potObject.optDouble("potNitrogen", 0.0), // Nitrogen level
                    potObject.optDouble("greenHouseTemperature", 0.0), // External temperature
                    potObject.optDouble("greenHouseHumidity", 0.0), // External humidity
                    potObject.optDouble("greenHousePressure",0.0)
            );

            potList.add(pot);
        }

        return potList;
    }



    public void parsePotData(JSONObject response) {
        if (response == null) return; // Avoid null pointer exceptions

        try {
            index = response.optInt("potId", -1); // Default to -1 if missing
            plantName = response.optString("plantName", "Unknown");
            type = response.optInt("potType", 0);
            camera = response.optBoolean("hasCamera", false);
            pompa = response.optBoolean("pumpStatus", false);
            sera = response.optBoolean("greenHouseStatus", false);
            temp = response.optDouble("temp", 0.0);
            hum = response.optDouble("humidity", 0.0);
            pot = response.optDouble("potassium", 0.0);
            pho = response.optDouble("phosphor", 0.0);
            ni = response.optDouble("nitrogen", 0.0);
            tempExt = response.optDouble("greenHouseTemperature", 0.0);
            humExt = response.optDouble("greenHouseHumidity", 0.0);
        } catch (Exception e) {
            e.printStackTrace();
            throw new RuntimeException("Error parsing pot data", e);
        }
    }


    public String getPlantName() {
        return plantName;
    }

    public int getType() {
        return type;
    }

    public boolean isCamera() {
        return camera;
    }

    public boolean isPompa() {
        return pompa;
    }

    public boolean isSera() {
        return sera;
    }

    public int getIndex() {
        return index;
    }

    public double getTemp() {
        return temp;
    }

    public double getLumi() {
        return lumi;
    }

    public double getHum() {
        return hum;
    }

    public double getPot() {
        return pot;
    }

    public double getPho() {
        return pho;
    }

    public double getNi() {
        return ni;
    }

    public double getTempExt() {
        return tempExt;
    }

    public double getHumExt() {
        return humExt;
    }

    public String getError(){return error;}
// Additional getters for other fields can be added as needed.
}
