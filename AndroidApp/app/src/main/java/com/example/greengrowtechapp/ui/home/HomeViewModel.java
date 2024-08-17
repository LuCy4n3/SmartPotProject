package com.example.greengrowtechapp.ui.home;

import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;
import androidx.lifecycle.ViewModel;

import com.example.greengrowtechapp.Handlers.JSONresponseHandler;
import com.example.greengrowtechapp.Handlers.NetworkHandler;

public class HomeViewModel extends ViewModel {

    private final MutableLiveData<String> mText;
    private final MutableLiveData<String> mButtonText; // LiveData for button text
    private final MutableLiveData<Boolean> mButtonPump; // LiveData for button state
    private final MutableLiveData<Boolean> mButtonGreenHouse; // LiveData for button state
    private final MutableLiveData<Integer> mindexOfCurrentPot = new MutableLiveData<>();
    private final MutableLiveData<Integer> mindexOfCurrentUser = new MutableLiveData<>();
    private final MutableLiveData<String> mURL = new MutableLiveData<>();


    private final MutableLiveData<JSONresponseHandler> mResponseHandler = new MutableLiveData<>();


    private final MutableLiveData<NetworkHandler> mNetworkHandler = new MutableLiveData<>();



    public HomeViewModel() {
        mText = new MutableLiveData<>();
        mText.postValue("This is home fragment");

        mButtonText = new MutableLiveData<>();
        mButtonText.postValue("Click ME 2"); // Initial button text

        mButtonPump = new MutableLiveData<>();
        mButtonPump.postValue(false); // Initial button state

        mButtonGreenHouse = new MutableLiveData<>();
        mButtonGreenHouse.postValue(false); // Initial button state

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

    public LiveData<String> getButtonText() {
        return mButtonText;
    }

    public MutableLiveData<Boolean> getmButtonPump() {
        return mButtonPump;
    }

    public MutableLiveData<Boolean> getmButtonGreenHouse() {
        return mButtonGreenHouse;
    }



    public void updateTextView() {
        String aux = null;
        // Handle the data based on your requirements
        JSONresponseHandler auxHandler = mResponseHandler.getValue();
        if(auxHandler != null)
            switch (auxHandler.getType()) {
            case 0:
                aux ="Nu exista givechi!";
                break;
            case 1:
                aux = "Ghiveciul " + auxHandler.getIndex() + "\n" + auxHandler.getPlantName() + " Planta in ghiveci" + "\n" + auxHandler.getTemp() + " Temperatura \n" + auxHandler.getHum() + " Umiditate \n" + auxHandler.getTempExt() + " Temperatura Exterioara\n" + auxHandler.getHumExt() + " Umiditate Exterioara\n";
                break;
            case 2:
                aux = "Ghiveciul " + auxHandler.getIndex() + "\n" + auxHandler.getPlantName() + " Planta in ghiveci" + "\n" + auxHandler.getTempExt()  + " Temperatura Exterioara\n" + auxHandler.getHumExt() + "Umiditate Exterioara\n" + auxHandler.getTemp() + " Temperatura \n" + auxHandler.getHum() + " Umiditate \n" + auxHandler.getPot() + " Potasiu \n" + auxHandler.getNi() + " Azot \n" + auxHandler.getPho() + " Phospor \n";
                break;
            case 3:
                aux = "Ghiveciul " + auxHandler.getIndex() + "beneficeaza si sera \n" + auxHandler.getPlantName() + " Planta in ghiveci" + "\n" + auxHandler.getTemp() + " Temperatura \n" + auxHandler.getHum() + "Umiditate \n" + auxHandler.getPot() + "Potasiu \n" + auxHandler.getNi() + " Azot \n" + auxHandler.getPho() + " Phospor \n" + auxHandler.getTempExt() + " Temperatura Exterioara\n" + auxHandler.getHumExt() + " Umiditate Exterioara\n";
                break;
            case 4:
                aux = "Sera " + auxHandler.getIndex() + "\n" + auxHandler.getPlantName() + " Planta in ghiveci" + "\n" + auxHandler.getTemp() + " Temperatura Exterioara\n" + auxHandler.getHumExt() + " Umiditate Exterioara\n";
                break;
            default:
                aux = "Error : didnt get a good type data";
                break;
        }
        else aux = "FATAL ERROR ";
        mText.postValue(aux);
        if(auxHandler.getType() == 2)
        {
            mButtonPump.postValue(true);
            mButtonGreenHouse.postValue(false);
        }
        else if(auxHandler.getType() == 3)
        {
            mButtonPump.postValue(true);
            mButtonGreenHouse.postValue(true);
        }
        else if(auxHandler.getType() == 4)
        {
            mButtonPump.postValue(false);
            mButtonGreenHouse.postValue(true);
        }
        else
        {
            mButtonPump.postValue(false);
            mButtonGreenHouse.postValue(false);
        }
    }
}
