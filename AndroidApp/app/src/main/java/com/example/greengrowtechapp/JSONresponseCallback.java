package com.example.greengrowtechapp;

import com.example.greengrowtechapp.Handlers.Plant;

import java.util.List;

public interface JSONresponseCallback {
    void onSuccess(List<Plant> plants);
    void onFailure();
}
