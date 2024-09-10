package com.example.greengrowtechapp.ui.dashboard;

import android.widget.ArrayAdapter;

import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;
import androidx.lifecycle.ViewModel;

import com.example.greengrowtechapp.Handlers.JSONresponseHandler;
import com.example.greengrowtechapp.Handlers.NetworkHandler;
import com.example.greengrowtechapp.Handlers.Plant;

import java.util.List;

public class DashboardViewModel extends ViewModel {

    private final MutableLiveData<String> mText;
    private final MutableLiveData<Integer> mindexOfCurrentPot = new MutableLiveData<>();
    private final MutableLiveData<Integer> mindexOfCurrentUser = new MutableLiveData<>();
    private final MutableLiveData<String> mURL = new MutableLiveData<>();
    private final MutableLiveData<JSONresponseHandler> mResponseHandler = new MutableLiveData<>();
    private final MutableLiveData<NetworkHandler> mNetworkHandler = new MutableLiveData<>();
    private final MutableLiveData<ArrayAdapter<String>> mAdapter = new MutableLiveData<>();
    private final MutableLiveData<List<Plant>> mPlantArray = new MutableLiveData<>();
    public DashboardViewModel() {
        mText = new MutableLiveData<>();
        mText.setValue("This is dashboard fragment");
        mResponseHandler.postValue(null);

        mNetworkHandler.postValue(null);

        mindexOfCurrentPot.postValue(0);

        mindexOfCurrentUser.postValue(0);

        mURL.postValue(null);
        mAdapter.postValue(null);

        mPlantArray.postValue(null);
    }
    public void setPlantArray(List<Plant> array)
    {
        mPlantArray.postValue(array);
    }
    public MutableLiveData<List<Plant>> getPlantArray()
    {
        return mPlantArray;
    }
    public void setAdapter(ArrayAdapter<String> adapter)
    {
        mAdapter.postValue(adapter);
    }

    public MutableLiveData<ArrayAdapter<String>> getAdapter() {
        return mAdapter;
    }

    public MutableLiveData<Integer> getIndexOfCurrentPot() {
        return mindexOfCurrentPot;
    }
    public void setIndexOfCurrentPot(Integer val) {
        mindexOfCurrentPot.postValue(val);
    }
    public MutableLiveData<Integer> getIndexOfCurrentUser() {
        return mindexOfCurrentUser;
    }
    public void setIndexOfCurrentUser(Integer val) {
        mindexOfCurrentUser.postValue(val);
    }
    public MutableLiveData<String> getURL() {
        return mURL;
    }
    public void setURL(String url) {
        mURL.postValue(url);
    }
    public void setResponseHandler(JSONresponseHandler obj)
    {
        mResponseHandler.postValue(obj);
    }
    public void setNetworkHandler(NetworkHandler obj)
    {
        mNetworkHandler.postValue(obj);
    }
    public LiveData<JSONresponseHandler> getResponseHandler()
    {
        return mResponseHandler;
    }
    public MutableLiveData<NetworkHandler> getNetworkHandler() {
        return mNetworkHandler;
    }
    public void setText(String text)
    {
        mText.postValue(text);
    }
    public LiveData<String> getText() {
        return mText;
    }
}