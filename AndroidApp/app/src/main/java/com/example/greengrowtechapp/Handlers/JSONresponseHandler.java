package com.example.greengrowtechapp.Handlers;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.io.Serializable;
import java.util.ArrayList;
import java.util.List;

public class JSONresponseHandler implements Serializable {
    private int index, type;
    private boolean sera, pompa;
    private double temp, lumi, hum, pot, pho, ni, tempExt, humExt;
    private String plantName;
    public List<Plant> parsePlantData(JSONArray jsonArray) throws JSONException {
        List<Plant> plantList = new ArrayList<>();

        for (int i = 0; i < jsonArray.length(); i++) {
            JSONObject plantObject = jsonArray.getJSONObject(i);

            Plant plant = new Plant(
                    plantObject.getInt("plantId"),
                    plantObject.getString("plantName"),
                    plantObject.getString("plantGroup"),
                    plantObject.getString("waterPref"),
                    plantObject.getString("lifeCycle"),
                    plantObject.getString("plantHabit"),
                    plantObject.getString("flowerColor"),
                    plantObject.getInt("phMinVal"),
                    plantObject.getInt("phMaxVal"),
                    plantObject.getInt("minTemp"),
                    plantObject.getInt("maxTemp"),
                    plantObject.getInt("sunReq"),
                    plantObject.getInt("plantHeight"),
                    plantObject.getInt("plantWidth"),
                    plantObject.getInt("fruitingTime"),
                    plantObject.getInt("flowerTime")
            );

            plantList.add(plant);
        }

        return plantList;
    }
    public void parsePotData(JSONObject response) throws JSONException  {
        try {
            if (response != null) {
                if (response.has("potId")) {
                    index = response.getInt("potId");
                }
                if (response.has("plantName")) {
                    plantName = response.getString("plantName");
                }
                if (response.has("potType")) {
                    type = response.getInt("potType");
                }
                if (response.has("pompa")) {
                    pompa = response.getBoolean("pompa");
                }
                if (response.has("sera")) {
                    sera = response.getBoolean("sera");
                }
                if (response.has("temp")) {
                    temp = response.getDouble("temp");
                }
                if (response.has("humidity")) {
                    hum = response.getDouble("humidity");
                }
                if (response.has("potassium")) {
                    pot = response.getDouble("potassium");
                }
                if (response.has("phosphor")) {
                    pho = response.getDouble("phosphor");
                }
                if (response.has("nitrogen")) {
                    ni = response.getDouble("nitrogen");
                }
                if (response.has("greenHouseTemperature")) {
                    tempExt = response.getDouble("greenHouseTemperature");
                }
                if (response.has("greenHouseHumidity")) {
                    humExt = response.getDouble("greenHouseHumidity");
                }
        }
            int a =0;
        } catch (JSONException e) {
            throw new RuntimeException(e);
        }

    }

    public String getPlantName() {
        return plantName;
    }

    public int getType() {
        return type;
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
// Additional getters for other fields can be added as needed.
}