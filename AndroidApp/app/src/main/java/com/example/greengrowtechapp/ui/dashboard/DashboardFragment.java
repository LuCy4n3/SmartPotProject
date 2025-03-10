package com.example.greengrowtechapp.ui.dashboard;

import android.os.Bundle;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.SearchView;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.fragment.app.Fragment;
import androidx.lifecycle.Observer;
import androidx.lifecycle.ViewModelProvider;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.example.greengrowtechapp.Handlers.JSONresponseHandler;
import com.example.greengrowtechapp.Handlers.NetworkHandler;
import com.example.greengrowtechapp.Handlers.Plant;
import com.example.greengrowtechapp.Handlers.Pot;
import com.example.greengrowtechapp.NetworkCallback;
import com.example.greengrowtechapp.databinding.FragmentDashboardBinding;
import com.example.greengrowtechapp.ui.home.HomeViewModel;

import java.util.ArrayList;
import java.util.List;

public class DashboardFragment extends Fragment implements PlantAdapter.OnItemClickListener {

    private FragmentDashboardBinding binding;
    private DashboardViewModel dashViewModel;
    private HomeViewModel homeViewModel;
    private JSONresponseHandler responseHandler = null;
    private NetworkHandler networkHandler = null;
    private PlantAdapter plantAdapter;
    private PotAdapter potAdapter;
    private List<Plant> plantArray = new ArrayList<>();
    private List<Pot> potArray = new ArrayList<>();

    private boolean isLoading = false;
    private int offset = 0;
    private static final int LIMIT = 10; // Load 50 items per request

    public View onCreateView(@NonNull LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
        dashViewModel = new ViewModelProvider(requireActivity()).get(DashboardViewModel.class);
        homeViewModel = new ViewModelProvider(requireActivity()).get(HomeViewModel.class);

        binding = FragmentDashboardBinding.inflate(inflater, container, false);
        View root = binding.getRoot();

        Toast.makeText(getContext(), "Dash Fragment created!", Toast.LENGTH_SHORT).show();
        TextView dashText = binding.textDashboard;
        dashText.setText("Trying to connect to server...");



        setupRecyclerView();
        observeViewModels();

        dashViewModel.getText().observe(getViewLifecycleOwner(), new Observer<String>() {
            @Override
            public void onChanged(String s) {
                dashText.setText(s);
            }
        });
        if (networkHandler != null) {

            networkHandler.getErrorCode().observe(getViewLifecycleOwner(), new Observer<Integer>() {
                @Override
                public void onChanged(Integer integer) {
                    dashText.setText(networkHandler.getErrorText().getValue());
                }
            });
        }
        binding.searchViewDashBoard.setOnQueryTextListener(new SearchView.OnQueryTextListener() {
            @Override
            public boolean onQueryTextSubmit(String query) {
                return false;
            }

            @Override
            public boolean onQueryTextChange(String newText) {
                if (!newText.isEmpty() && networkHandler != null) {
                    offset = 0; // Reset offset on new search
                    plantArray.clear(); // Clear old data
                    plantAdapter.notifyDataSetChanged();
                    fetchPlants(newText);
                } else {
                    plantArray.clear();
                    plantAdapter.notifyDataSetChanged();
                }
                return true;
            }
        });

        binding.buttonUpdate.setOnClickListener(v->fetchPots());


        return root;
    }

    private void setupRecyclerView() {
        plantAdapter = new PlantAdapter(plantArray, this); // Pass 'this' as the listener
        binding.recyclerViewDashBoard.setLayoutManager(new LinearLayoutManager(getContext()));
        binding.recyclerViewDashBoard.setAdapter(plantAdapter);

        // Initialize PotAdapter with click listeners
        potAdapter = new PotAdapter(potArray, new PotAdapter.OnItemClickListener() {
            @Override
            public void onItemClick(Pot pot) {
                // Handle item click
                Toast.makeText(getContext(), "Clicked pot: " + pot.getPotName(), Toast.LENGTH_SHORT).show();
            }
        }, new PotAdapter.OnButtonClickListener() {
            @Override
            public void onButtonClick(Pot pot) {
                // Handle button click
                Toast.makeText(getContext(), "Button clicked for pot: " + pot.getPotName(), Toast.LENGTH_SHORT).show();
            }
        });

        binding.recyclerViewDashBrdForPots.setLayoutManager(new LinearLayoutManager(getContext()));
        binding.recyclerViewDashBrdForPots.setAdapter(potAdapter);

        // Add scroll listeners (existing code)
        binding.recyclerViewDashBoard.addOnScrollListener(new RecyclerView.OnScrollListener() {
            @Override
            public void onScrolled(@NonNull RecyclerView recyclerView, int dx, int dy) {
                super.onScrolled(recyclerView, dx, dy);
                LinearLayoutManager layoutManager = (LinearLayoutManager) recyclerView.getLayoutManager();
                if (layoutManager != null && !isLoading) {
                    int visibleItemCount = layoutManager.getChildCount();
                    int totalItemCount = layoutManager.getItemCount();
                    int firstVisibleItemPosition = layoutManager.findFirstVisibleItemPosition();

                    // Check if the user has scrolled to the end of the list
                    if ((visibleItemCount + firstVisibleItemPosition) >= totalItemCount
                            && firstVisibleItemPosition >= 0
                            && totalItemCount >= LIMIT) {
                        fetchPlants(binding.searchViewDashBoard.getQuery().toString());
                    }
                }
            }
        });

        binding.recyclerViewDashBrdForPots.addOnScrollListener(new RecyclerView.OnScrollListener() {
            @Override
            public void onScrolled(@NonNull RecyclerView recyclerView, int dx, int dy) {
                super.onScrolled(recyclerView, dx, dy);
                LinearLayoutManager layoutManager = (LinearLayoutManager) recyclerView.getLayoutManager();
                if (layoutManager != null && !isLoading) {
                    int visibleItemCount = layoutManager.getChildCount();
                    int totalItemCount = layoutManager.getItemCount();
                    int firstVisibleItemPosition = layoutManager.findFirstVisibleItemPosition();

                    // Check if the user has scrolled to the end of the list
                    if ((visibleItemCount + firstVisibleItemPosition) >= totalItemCount
                            && firstVisibleItemPosition >= 0
                            && totalItemCount >= LIMIT) {
                        fetchPots(); // Call method to fetch more pots
                    }
                }
            }
        });
    }

    private void observeViewModels() {
        dashViewModel.getNetworkHandler().observe(getViewLifecycleOwner(), netHandler -> networkHandler = netHandler);
        dashViewModel.getResponseHandler().observe(getViewLifecycleOwner(), jsoNresponseHandler -> responseHandler = jsoNresponseHandler);
        dashViewModel.getNetworkHandler().observe(getViewLifecycleOwner(), netHandler -> {
            networkHandler = netHandler;
            if (networkHandler != null) {
                networkHandler.getErrorText().observe(getViewLifecycleOwner(), errorMessage -> {
                    binding.textDashboard.setText(errorMessage); // Update TextView
                    if(networkHandler.getErrorCode().getValue() >= 0 )
                        dashViewModel.setText("update pot.");
                    else
                        dashViewModel.setText(String.valueOf(networkHandler.getErrorCode().getValue()));
                });
            }
        });
        dashViewModel.getResponseHandler().observe(getViewLifecycleOwner(), jsoNresponseHandler -> responseHandler = jsoNresponseHandler);
    }

    private void fetchPlants(String query) {
        if (isLoading) return;
        isLoading = true;

        String url = dashViewModel.getURL().getValue() + query + "/" + LIMIT + "/" + offset;

        networkHandler.sendGetRequestListPlant(url, new NetworkCallback() {
            @Override
            public void onSuccess() {}

            @Override
            public void onPlantListGetSucces(List<Plant> plants) {
                if (!plants.isEmpty()) {
                    plantArray.addAll(plants);
                    plantAdapter.notifyDataSetChanged();
                    offset += LIMIT; // Increase offset for next request
                }
                isLoading = false;
            }

            @Override
            public void onPotListGetSucces(List<Pot> pots) {

            }

            @Override
            public void onFailure() {
                if (getActivity() != null) {
                    getActivity().runOnUiThread(() ->
                            Toast.makeText(getContext(), "Error fetching data!", Toast.LENGTH_SHORT).show()
                    );
                }
                isLoading = false;
            }
        });
    }
    private void fetchPots() {
        if (isLoading) return;
        isLoading = true;

        String url = homeViewModel.getURL().getValue() + "1";

        networkHandler.sendGetRequestListPot(url, new NetworkCallback() {
            @Override
            public void onSuccess() {}

            @Override
            public void onPlantListGetSucces(List<Plant> plants) {}

            @Override
            public void onPotListGetSucces(List<Pot> pots) {
                if (!pots.isEmpty()) {
                    Log.d("PotAdapter", "Received " + pots.size() + " pots");
                    for (Pot pot : pots) {
                        Log.d("PotAdapter", "Pot: " + pot.getPotName() + ", Plant: " + pot.getPlantName());
                    }
                    potArray.clear(); // Clear old data
                    potArray.addAll(pots); // Add new data
                    potAdapter.notifyDataSetChanged(); // Notify adapter of data change
                }
                isLoading = false;
            }

            @Override
            public void onFailure() {
                if (getActivity() != null) {
                    getActivity().runOnUiThread(() ->
                            Toast.makeText(getContext(), "Error fetching pots!", Toast.LENGTH_SHORT).show()
                    );
                }
                isLoading = false;
            }
        });
    }


    @Override
    public void onItemClick(Plant plant) {
        // Handle the item click, e.g., show a toast with the plant name
        Toast.makeText(getContext(), "Clicked plant: " + plant.getPlantName(), Toast.LENGTH_SHORT).show();
        String aux = homeViewModel.getURL().getValue()+"1/1";
        networkHandler.sendPutRequest(aux,"PlantName:"+plant.getPlantName());
    }

    @Override
    public void onDestroyView() {
        super.onDestroyView();
        binding = null;
    }
}
