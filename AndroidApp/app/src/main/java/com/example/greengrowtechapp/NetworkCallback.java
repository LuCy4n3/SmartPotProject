package com.example.greengrowtechapp;

import com.example.greengrowtechapp.Handlers.Plant;
import com.example.greengrowtechapp.Handlers.Pot;

import java.util.List;

public interface NetworkCallback {
    void onSuccess();
    void onPlantListGetSucces(List<Plant> plants);
    void onPotListGetSucces(List<Pot> pots);
    void onFailure();

}
