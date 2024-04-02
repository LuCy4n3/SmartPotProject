package com.example.myapplication;

import java.util.List;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;
import retrofit2.Retrofit;
import retrofit2.converter.gson.GsonConverterFactory;

public class PlantRepository {
    private static final String BASE_URL = MainActivity.url;

    private PlantApi plantApi;

    public PlantRepository() {
        Retrofit retrofit = new Retrofit.Builder()
                .baseUrl(BASE_URL)
                .addConverterFactory(GsonConverterFactory.create())
                .build();

        plantApi = retrofit.create(PlantApi.class);
    }

    public void getAllPlants(final PlantCallback<List<Plant>> callback) {
        try {
            Call<List<Plant>> call = plantApi.getAllPlants();
            call.enqueue(new Callback<List<Plant>>() {
                @Override
                public void onResponse(Call<List<Plant>> call, Response<List<Plant>> response) {
                    if (response.isSuccessful()) {
                        callback.onSuccess(response.body());
                    } else {
                        callback.onError(new Throwable("Failed to get plants"));
                    }
                }

                @Override
                public void onFailure(Call<List<Plant>> call, Throwable t) {
                    callback.onError(t);
                }
            });
        } catch (Exception e) {
            throw new RuntimeException(e);
        }

    }

    public interface PlantCallback<T> {
        void onSuccess(T data);
        void onError(Throwable throwable);
    }
}
