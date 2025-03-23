package com.example.greengrowtechapp;

import android.os.Bundle;

import androidx.appcompat.app.AppCompatActivity;
import androidx.appcompat.app.AppCompatDelegate;
import androidx.fragment.app.Fragment;
import androidx.fragment.app.FragmentTransaction;
import androidx.lifecycle.Observer;
import androidx.lifecycle.ViewModelProvider;
import androidx.navigation.NavController;
import androidx.navigation.Navigation;
import androidx.navigation.ui.AppBarConfiguration;
import androidx.navigation.ui.NavigationUI;

import com.example.greengrowtechapp.Handlers.JSONresponseHandler;
import com.example.greengrowtechapp.Handlers.NetworkHandler;
import com.example.greengrowtechapp.Handlers.Plant;
import com.example.greengrowtechapp.Handlers.Pot;
import com.example.greengrowtechapp.databinding.ActivityMainBinding;
import com.example.greengrowtechapp.ui.dashboard.DashboardViewModel;
import com.example.greengrowtechapp.ui.home.HomeViewModel;
import com.example.greengrowtechapp.ui.notifications.NotificationsViewModel;

import org.json.JSONException;

import java.util.List;

public class MainActivity extends AppCompatActivity {

    private ActivityMainBinding binding;
    private static Integer indexOfCurrentPot = new Integer(1);
    private static Integer indexOfCurrentUser = new Integer(1);
    private static String Url = "http://192.168.0.200:3000/api/";
    //private static String URLpot = new String("http://roka.go.ro:3000/api/Pot/");
    private static String URLpot = new String(Url+"Pot/");
    //private static String URLplant = new String("http://roka.go.ro:3000/api/Plant/");
    private static String URLplant = new String(Url+"Plant/");
    //private static String URLimage = new String("http://roka.go.ro:3000/api/image/test.jpg");
    private static String URLimage = new String(Url+"image/test.jpg");
    private static boolean pumpStat,hasCamera;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        binding = ActivityMainBinding.inflate(getLayoutInflater());
        setContentView(binding.getRoot());


        JSONresponseHandler responseHandler = new JSONresponseHandler();


        NetworkHandler networkHandler = null;
        try {
            networkHandler = new NetworkHandler(this, responseHandler);
        } catch (JSONException e) {
            throw new RuntimeException(e);
        }



        HomeViewModel homeViewModel = new ViewModelProvider(this).get(HomeViewModel.class);
        homeViewModel.setResponseHandler(responseHandler);
        homeViewModel.setNetworkHandler(networkHandler);


        NotificationsViewModel notViewModel = new ViewModelProvider(this).get(NotificationsViewModel.class);
        notViewModel.setNetworkHandler(networkHandler);
        notViewModel.setResponseHandler(responseHandler);



        DashboardViewModel dashViewModel = new ViewModelProvider(this).get(DashboardViewModel.class);
        dashViewModel.setNetworkHandler(networkHandler);
        dashViewModel.setResponseHandler(responseHandler);



        homeViewModel.getIndexOfCurrentPot().observe(this, new Observer<Integer>() {
            @Override
            public void onChanged(Integer newPotIndex) {
                notViewModel.setIndexOfCurrentPot(newPotIndex);
                dashViewModel.setIndexOfCurrentPot(newPotIndex);
            }
        });
        NetworkHandler finalNetworkHandler = networkHandler;
        networkHandler.sendGetRequestListPot(URLpot + "1", new NetworkCallback() {
            @Override
            public void onSuccess() {

            }

            @Override
            public void onPlantListGetSucces(List<Plant> plants) {

            }

            @Override
            public void onPotListGetSucces(List<Pot> pots) {
                Pot aux = pots.get(0);
                indexOfCurrentPot = aux.getPotId();
                updatePumpStatusBeforeFragmentStart(finalNetworkHandler,responseHandler, homeViewModel, notViewModel, dashViewModel);

            }

            @Override
            public void onFailure() {
                int a = 0;
                updatePumpStatusBeforeFragmentStart(finalNetworkHandler,responseHandler, homeViewModel, notViewModel, dashViewModel);

            }
        });


        AppCompatDelegate.setDefaultNightMode(AppCompatDelegate.MODE_NIGHT_NO);


        AppBarConfiguration appBarConfiguration = new AppBarConfiguration.Builder(
                R.id.navigation_home, R.id.navigation_dashboard, R.id.navigation_notifications)
                .build();
        NavController navController = Navigation.findNavController(this, R.id.nav_host_fragment_activity_main);
        NavigationUI.setupActionBarWithNavController(this, navController, appBarConfiguration);
        NavigationUI.setupWithNavController(binding.navView, navController);



    }
    private void updatePumpStatusBeforeFragmentStart(NetworkHandler networkHandler,JSONresponseHandler responseHandler, HomeViewModel homeViewModel, NotificationsViewModel notViewModel, DashboardViewModel dashViewModel) {
        String url = URLpot + indexOfCurrentUser + "/" + indexOfCurrentPot;

        networkHandler.sendGetRequest(url, new NetworkCallback() {
            @Override
            public void onSuccess() {
                pumpStat = responseHandler.isPompa();
                hasCamera = responseHandler.isCamera();

                homeViewModel.setNetworkHandler(networkHandler);
                homeViewModel.setResponseHandler(responseHandler);
                homeViewModel.setmButtonPumpPress(pumpStat);
                homeViewModel.setmHasCamera(hasCamera);
                homeViewModel.setmButtonPumpPress(pumpStat);
                homeViewModel.setmHasCamera(hasCamera);
                homeViewModel.setIndexOfCurrentPot(indexOfCurrentPot);
                homeViewModel.setIndexOfCurrentUser(indexOfCurrentUser);
                homeViewModel.setURL(URLpot);
                homeViewModel.setURLimage(URLimage);


                notViewModel.setNetworkHandler(networkHandler);
                notViewModel.setResponseHandler(responseHandler);
                notViewModel.setIndexOfCurrentPot(indexOfCurrentPot);
                notViewModel.setIndexOfCurrentUser(indexOfCurrentUser);
                notViewModel.setURL(URLpot);


                dashViewModel.setNetworkHandler(networkHandler);
                dashViewModel.setResponseHandler(responseHandler);
                dashViewModel.setIndexOfCurrentPot(indexOfCurrentPot);
                dashViewModel.setIndexOfCurrentUser(indexOfCurrentUser);
                dashViewModel.setURL(URLplant);

                //notViewModel.setIndexOfCurrentPot(indexOfCurrentPot);
                //dashViewModel.setIndexOfCurrentPot(indexOfCurrentPot);
            }

            @Override
            public void onPlantListGetSucces(List<Plant> plants) {

            }

            @Override
            public void onPotListGetSucces(List<Pot> pots) {

            }

            @Override
            public void onFailure() {
                //Toast.makeText(MainActivity.this, "Failed to update pump status!", Toast.LENGTH_SHORT).show();
                homeViewModel.setNetworkHandler(networkHandler);
                homeViewModel.setResponseHandler(responseHandler);
                homeViewModel.setIndexOfCurrentPot(indexOfCurrentPot);
                homeViewModel.setIndexOfCurrentUser(indexOfCurrentUser);
                homeViewModel.setURL(URLpot);
                homeViewModel.setURLimage(URLimage);

                notViewModel.setNetworkHandler(networkHandler);
                notViewModel.setResponseHandler(responseHandler);
                notViewModel.setIndexOfCurrentPot(indexOfCurrentPot);
                notViewModel.setIndexOfCurrentUser(indexOfCurrentUser);
                notViewModel.setURL(URLpot);

                dashViewModel.setNetworkHandler(networkHandler);
                dashViewModel.setResponseHandler(responseHandler);
                dashViewModel.setIndexOfCurrentPot(indexOfCurrentPot);
                dashViewModel.setIndexOfCurrentUser(indexOfCurrentUser);
                dashViewModel.setURL(URLplant);
            }


        });
    }
    private void loadFragment(Fragment fragment, Bundle bundle) {
        fragment.setArguments(bundle);
        FragmentTransaction transaction = getSupportFragmentManager().beginTransaction();
        transaction.replace(R.id.nav_host_fragment_activity_main, fragment);
        transaction.commit();
    }
}
