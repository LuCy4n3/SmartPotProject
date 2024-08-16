package com.example.greengrowtechapp;

import android.os.Bundle;

import androidx.appcompat.app.AppCompatActivity;
import androidx.fragment.app.Fragment;
import androidx.fragment.app.FragmentTransaction;
import androidx.lifecycle.ViewModelProvider;
import androidx.navigation.NavController;
import androidx.navigation.Navigation;
import androidx.navigation.ui.AppBarConfiguration;
import androidx.navigation.ui.NavigationUI;

import com.example.greengrowtechapp.Handlers.JSONresponseHandler;
import com.example.greengrowtechapp.Handlers.NetworkHandler;
import com.example.greengrowtechapp.R;
import com.example.greengrowtechapp.databinding.ActivityMainBinding;
import com.example.greengrowtechapp.ui.home.HomeFragment;
import com.example.greengrowtechapp.ui.home.HomeViewModel;
import com.google.android.material.bottomnavigation.BottomNavigationView;

import org.json.JSONException;

public class MainActivity extends AppCompatActivity {

    private ActivityMainBinding binding;

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
        //BottomNavigationView navView = findViewById(R.id.nav_view);
        // Initially load the HomeFragment
        //loadFragment(new HomeFragment(), bundle);

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
