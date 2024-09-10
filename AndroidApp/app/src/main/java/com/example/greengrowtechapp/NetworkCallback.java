package com.example.greengrowtechapp;

import com.example.greengrowtechapp.Handlers.Plant;

import java.util.List;

public interface NetworkCallback {
    void onSuccess();
    void onListGetSucces(List<Plant> plants);
    void onFailure();
}
