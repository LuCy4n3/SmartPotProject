package com.example.greengrowtechapp;

import android.os.Bundle;

import androidx.appcompat.app.AppCompatActivity;
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
import com.example.greengrowtechapp.databinding.ActivityMainBinding;
import com.example.greengrowtechapp.ui.dashboard.DashboardViewModel;
import com.example.greengrowtechapp.ui.home.HomeViewModel;
import com.example.greengrowtechapp.ui.notifications.NotificationsViewModel;

import org.json.JSONException;

public class MainActivity extends AppCompatActivity {

    private ActivityMainBinding binding;
    private static Integer indexOfCurrentPot = new Integer(1);
    private static Integer indexOfCurrentUser = new Integer(1);
    private static String URLpot = new String("http://192.168.201.1:3000/api/Pot/");
    private static String URLplant = new String("http://192.168.201.1:3000/api/Plant/");


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
        homeViewModel.setIndexOfCurrentPot(indexOfCurrentPot);
        homeViewModel.setIndexOfCurrentUser(indexOfCurrentUser);
        homeViewModel.setURL(URLpot);

        homeViewModel.setResponseHandler(responseHandler);
        homeViewModel.setNetworkHandler(networkHandler);

        NotificationsViewModel notViewModel = new ViewModelProvider(this).get(NotificationsViewModel.class);

        notViewModel.setIndexOfCurrentPot(indexOfCurrentPot);
        notViewModel.setIndexOfCurrentUser(indexOfCurrentUser);
        notViewModel.setURL(URLpot);
        notViewModel.setNetworkHandler(networkHandler);
        notViewModel.setResponseHandler(responseHandler);

        DashboardViewModel dashViewModel = new ViewModelProvider(this).get(DashboardViewModel.class);

        dashViewModel.setIndexOfCurrentPot(indexOfCurrentPot);
        dashViewModel.setIndexOfCurrentUser(indexOfCurrentUser);
        dashViewModel.setURL(URLplant);
        dashViewModel.setNetworkHandler(networkHandler);
        dashViewModel.setResponseHandler(responseHandler);
        homeViewModel.getIndexOfCurrentPot().observe(this, new Observer<Integer>() {
            @Override
            public void onChanged(Integer newPotIndex) {
                notViewModel.setIndexOfCurrentPot(newPotIndex);
                dashViewModel.setIndexOfCurrentPot(newPotIndex);
            }
        });


        AppBarConfiguration appBarConfiguration = new AppBarConfiguration.Builder(
                R.id.navigation_home, R.id.navigation_dashboard, R.id.navigation_notifications)
                .build();
        NavController navController = Navigation.findNavController(this, R.id.nav_host_fragment_activity_main);
        NavigationUI.setupActionBarWithNavController(this, navController, appBarConfiguration);
        NavigationUI.setupWithNavController(binding.navView, navController);
    }

    private void loadFragment(Fragment fragment, Bundle bundle) {
        fragment.setArguments(bundle);
        FragmentTransaction transaction = getSupportFragmentManager().beginTransaction();
        transaction.replace(R.id.nav_host_fragment_activity_main, fragment);
        transaction.commit();
    }
}
