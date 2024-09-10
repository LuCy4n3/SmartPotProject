package com.example.greengrowtechapp.ui.notifications;

import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;
import androidx.lifecycle.ViewModel;

import com.example.greengrowtechapp.Handlers.JSONresponseHandler;
import com.example.greengrowtechapp.Handlers.NetworkHandler;

public class NotificationsViewModel extends ViewModel {

    private final MutableLiveData<String> mText;
    private final MutableLiveData<Integer> mindexOfCurrentPot = new MutableLiveData<>();
    private final MutableLiveData<Integer> mindexOfCurrentUser = new MutableLiveData<>();
    private final MutableLiveData<String> mURL = new MutableLiveData<>();
    private final MutableLiveData<JSONresponseHandler> mResponseHandler = new MutableLiveData<>();
    private final MutableLiveData<NetworkHandler> mNetworkHandler = new MutableLiveData<>();

    public NotificationsViewModel() {
        mText = new MutableLiveData<>();
        mText.setValue("This is notifications fragment");

        mResponseHandler.postValue(null);

        mNetworkHandler.postValue(null);

        mindexOfCurrentPot.postValue(0);

        mindexOfCurrentUser.postValue(0);

        mURL.postValue(null);

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
    public LiveData<String> getText() {
        return mText;
    }
}