package com.example.greengrowtechapp.ui.home;

import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.fragment.app.Fragment;
import androidx.lifecycle.Observer;
import androidx.lifecycle.ViewModelProvider;

import com.example.greengrowtechapp.Handlers.JSONresponseHandler;
import com.example.greengrowtechapp.Handlers.NetworkHandler;
import com.example.greengrowtechapp.NetworkCallback;
import com.example.greengrowtechapp.databinding.FragmentHomeBinding;

public class HomeFragment extends Fragment {

    private FragmentHomeBinding binding;
    private HomeViewModel homeViewModel;

    private JSONresponseHandler responseHandler = null;
    private NetworkHandler networkHandler = null;

    @Override
    public View onCreateView(@NonNull LayoutInflater inflater,
                             ViewGroup container, Bundle savedInstanceState) {
        // ViewModel initialization
        homeViewModel = new ViewModelProvider(requireActivity()).get(HomeViewModel.class);

        // View Binding initialization
        binding = FragmentHomeBinding.inflate(inflater, container, false);
        View root = binding.getRoot();

        homeViewModel.getResponseHandler().observe(getViewLifecycleOwner(), new Observer<JSONresponseHandler>() {

            @Override
            public void onChanged(JSONresponseHandler jsoNresponseHandler) {
                responseHandler = jsoNresponseHandler;
                Toast.makeText(getContext(), "got the shared data!", Toast.LENGTH_SHORT).show();
                if (responseHandler == null) {
                    Toast.makeText(getContext(), "Error: Response handler not found!", Toast.LENGTH_SHORT).show();
                }
            }

        });

        homeViewModel.getNetworkHandler().observe(getViewLifecycleOwner(), new Observer<NetworkHandler>() {
            @Override
            public void onChanged(NetworkHandler netHandler) {
                networkHandler = netHandler;

                networkHandler.sendGetRequest("http://192.168.201.1:3000/api/Pot/1/1", new NetworkCallback() {
                    @Override
                    public void onSuccess() {
                        //homeViewModel.onButtonClicked();
                        homeViewModel.updateTextView(); // Execute this after the network request completes successfully
                    }

                    @Override
                    public void onFailure() {
                        Toast.makeText(getContext(), "Network request failed!", Toast.LENGTH_SHORT).show();
                    }
                });
            }

        });

        // Observing LiveData from HomeViewModel
        homeViewModel.getText().observe(getViewLifecycleOwner(), new Observer<String>() {

                    @Override
                    public void onChanged(String s) {
                        binding.textHome.setText(s);
                    }
                });


//        homeViewModel.getButtonText().observe(getViewLifecycleOwner(), text -> {
//            binding.buttonHome.setText(text);
//        });

        homeViewModel.getmButtonPump().observe(getViewLifecycleOwner(), new Observer<Boolean>() {
            @Override
            public void onChanged(Boolean aBoolean) {
                Toast.makeText(getContext(), "Button state changed!", Toast.LENGTH_SHORT).show();
                if(aBoolean)
                    binding.buttonPump.setVisibility(View.VISIBLE);
                else
                    binding.buttonPump.setVisibility(View.INVISIBLE);
            }
        });

        homeViewModel.getmButtonGreenHouse().observe(getViewLifecycleOwner(), new Observer<Boolean>() {
            @Override
            public void onChanged(Boolean aBoolean) {
                Toast.makeText(getContext(), "Button state changed!", Toast.LENGTH_SHORT).show();
                if(aBoolean)
                    binding.buttonGreenHouse.setVisibility(View.VISIBLE);
                else
                    binding.buttonGreenHouse.setVisibility(View.INVISIBLE);
            }
        });


        // Retrieving arguments from bundle





        // Set up button click listener
//        binding.buttonHome.setOnClickListener(v -> {
//            homeViewModel.onButtonClicked(); // Update button state after successful request
//            Toast.makeText(getContext(), "Button HOME FRAGMENT clicked!", Toast.LENGTH_SHORT).show();
//            networkHandler.sendGetRequest("http://192.168.201.1:3000/api/Pot/1/1", new NetworkCallback() {
//                @Override
//                public void onSuccess() {
//
//                    homeViewModel.updateTextView();
//                }
//
//                @Override
//                public void onFailure() {
//                    Toast.makeText(getContext(), "Failed to execute request", Toast.LENGTH_SHORT).show();
//                }
//            });
//            //homeViewModel.onButtonClicked(); // Update the button state in the ViewModel
//        });

        return root;
    }

    @Override
    public void onDestroyView() {
        super.onDestroyView();
        binding = null;
    }
}
