package com.example.greengrowtechapp.ui.home;

import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.fragment.app.Fragment;
import androidx.lifecycle.Observer;
import androidx.lifecycle.ViewModelProvider;

import com.example.greengrowtechapp.Handlers.JSONresponseHandler;
import com.example.greengrowtechapp.Handlers.NetworkHandler;
import com.example.greengrowtechapp.Handlers.Plant;
import com.example.greengrowtechapp.NetworkCallback;
import com.example.greengrowtechapp.databinding.FragmentHomeBinding;

import java.util.List;

public class HomeFragment extends Fragment {

    private FragmentHomeBinding binding;
    private HomeViewModel homeViewModel;

    private JSONresponseHandler responseHandler = null;
    private NetworkHandler networkHandler = null;
    private static final String[] paths = {"Dispozitiv 1", "Dispozitiv 2", "Dispozitiv 3", "Dispozitiv 4", "Dispozitiv 5"};

    @Override
    public View onCreateView(@NonNull LayoutInflater inflater,
                             ViewGroup container, Bundle savedInstanceState) {
        // ViewModel initialization
        homeViewModel = new ViewModelProvider(requireActivity()).get(HomeViewModel.class);

        // View Binding initialization
        binding = FragmentHomeBinding.inflate(inflater, container, false);
        View root = binding.getRoot();

        ArrayAdapter<String> adapter = new ArrayAdapter<>(getContext(),
                android.R.layout.simple_spinner_item, paths);
        adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
        binding.spinner.setAdapter(adapter);


        homeViewModel.getURL().observe(getViewLifecycleOwner(), text -> {
            homeViewModel.setURL(text);
        });
        homeViewModel.getIndexOfCurrentPot().observe(getViewLifecycleOwner(), new Observer<Integer>() {

            @Override
            public void onChanged(Integer integer) {
                //Toast.makeText(getContext(), "got the indexof pot!", Toast.LENGTH_SHORT).show();
                homeViewModel.setIndexOfCurrentPot(integer);
            }
        });

        homeViewModel.getIndexOfCurrentUser().observe(getViewLifecycleOwner(), new Observer<Integer>() {

            @Override
            public void onChanged(Integer integer) {
                //Toast.makeText(getContext(), "got the indexof user!", Toast.LENGTH_SHORT).show();
                homeViewModel.setIndexOfCurrentUser(integer);
            }
        });
        homeViewModel.getResponseHandler().observe(getViewLifecycleOwner(), new Observer<JSONresponseHandler>() {

            @Override
            public void onChanged(JSONresponseHandler jsoNresponseHandler) {
                responseHandler = jsoNresponseHandler;
                //Toast.makeText(getContext(), "got the shared data!", Toast.LENGTH_SHORT).show();
                if (responseHandler == null) {
                    Toast.makeText(getContext(), "Error: Response handler not found!", Toast.LENGTH_SHORT).show();
                }
            }

        });

        homeViewModel.getNetworkHandler().observe(getViewLifecycleOwner(), new Observer<NetworkHandler>() {
            @Override
            public void onChanged(NetworkHandler netHandler) {
                networkHandler = netHandler;
                if(homeViewModel.getURL().getValue() != null && homeViewModel.getIndexOfCurrentUser().getValue() != null && homeViewModel.getIndexOfCurrentPot().getValue() != null) {
                    String aux = homeViewModel.getURL().getValue() + homeViewModel.getIndexOfCurrentUser().getValue() +"/"+ homeViewModel.getIndexOfCurrentPot().getValue();

                    networkHandler.sendGetRequest(aux, new NetworkCallback() {

                        @Override
                        public void onSuccess() {
                            homeViewModel.updateTextView(); // Execute this after the network request completes successfully

                        }
                        @Override
                        public void onListGetSucces(List<Plant> plants) {

                        }
                        @Override
                        public void onFailure() {
                            Toast.makeText(getContext(), "Network request failed for index "+homeViewModel.getIndexOfCurrentPot().getValue() +" !", Toast.LENGTH_SHORT).show();
                        }
                    });
                }
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
                //Toast.makeText(getContext(), "Button state changed!", Toast.LENGTH_SHORT).show();
                if(aBoolean)
                    binding.buttonPump.setVisibility(View.VISIBLE);
                else
                    binding.buttonPump.setVisibility(View.INVISIBLE);
            }
        });

        homeViewModel.getmButtonGreenHouse().observe(getViewLifecycleOwner(), new Observer<Boolean>() {
            @Override
            public void onChanged(Boolean aBoolean) {
                //Toast.makeText(getContext(), "Button state changed!", Toast.LENGTH_SHORT).show();
                if(aBoolean)
                    binding.buttonGreenHouse.setVisibility(View.VISIBLE);
                else
                    binding.buttonGreenHouse.setVisibility(View.INVISIBLE);
            }
        });


        // Retrieving arguments from bundle





        // Set up button click listener
        binding.buttonUpdate.setOnClickListener(v -> {
            Toast.makeText(getContext(), "Button HOME FRAGMENT clicked!", Toast.LENGTH_SHORT).show();
            if(homeViewModel.getURL().getValue() != null && homeViewModel.getIndexOfCurrentUser().getValue() != null && homeViewModel.getIndexOfCurrentPot().getValue() != null) {
                String aux = homeViewModel.getURL().getValue() + homeViewModel.getIndexOfCurrentUser().getValue() +"/"+ homeViewModel.getIndexOfCurrentPot().getValue();

                int a = 0;
                networkHandler.sendGetRequest(aux, new NetworkCallback() {
                    @Override
                    public void onSuccess() {
                        //homeViewModel.onButtonClicked();
                        //Toast.makeText(getContext(), "Success!", Toast.LENGTH_SHORT).show();
                        homeViewModel.updateTextView(); // Execute this after the network request completes successfully
                    }

                    @Override
                    public void onListGetSucces(List<Plant> plants) {
                        int a = 0;
                    }
                    @Override
                    public void onFailure() {
                        Toast.makeText(getContext(), "Network request failed!", Toast.LENGTH_SHORT).show();
                    }
                });
            }

            //homeViewModel.onButtonClicked(); // Update the button state in the ViewModel
        });

        binding.spinner.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {
            @Override
            public void onItemSelected(AdapterView<?> parent, View view, int position, long id) {
                Toast.makeText(getContext(), "Changed pot index to "+(position+1)+" !", Toast.LENGTH_SHORT).show();
                homeViewModel.setIndexOfCurrentPot(position+1);
            }

            @Override
            public void onNothingSelected(AdapterView<?> parent) {

            }
        });
        return root;
    }

    @Override
    public void onDestroyView() {
        super.onDestroyView();
        binding = null;
    }
}
