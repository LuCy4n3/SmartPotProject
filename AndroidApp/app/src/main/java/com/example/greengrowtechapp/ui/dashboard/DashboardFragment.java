package com.example.greengrowtechapp.ui.dashboard;

import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.MotionEvent;
import android.view.View;
import android.view.ViewGroup;
import android.widget.SearchView;
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
    private PlantAdapter adapter;
    private List<Plant> plantArray = new ArrayList<>();

    private boolean isLoading = false;
    private int offset = 0;
    private static final int LIMIT = 10; // Load 50 items per request

    public View onCreateView(@NonNull LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
        dashViewModel = new ViewModelProvider(requireActivity()).get(DashboardViewModel.class);
        homeViewModel = new ViewModelProvider(requireActivity()).get(HomeViewModel.class);

        binding = FragmentDashboardBinding.inflate(inflater, container, false);
        View root = binding.getRoot();

        Toast.makeText(getContext(), "Dash Fragment created!", Toast.LENGTH_SHORT).show();

        setupRecyclerView();
        observeViewModels();

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
                    adapter.notifyDataSetChanged();
                    fetchPlants(newText);
                } else {
                    plantArray.clear();
                    adapter.notifyDataSetChanged();
                }
                return true;
            }
        });

        return root;
    }

    private void setupRecyclerView() {
        adapter = new PlantAdapter(plantArray, this); // Pass 'this' as the listener
        binding.recyclerViewDashBoard.setLayoutManager(new LinearLayoutManager(getContext()));
        binding.recyclerViewDashBoard.setAdapter(adapter);

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
    }

    private void observeViewModels() {
        dashViewModel.getNetworkHandler().observe(getViewLifecycleOwner(), netHandler -> networkHandler = netHandler);
        dashViewModel.getResponseHandler().observe(getViewLifecycleOwner(), jsoNresponseHandler -> responseHandler = jsoNresponseHandler);
    }

    private void fetchPlants(String query) {
        if (isLoading) return;
        isLoading = true;

        String url = dashViewModel.getURL().getValue() + query + "/" + LIMIT + "/" + offset;

        networkHandler.sendGetRequestList(url, new NetworkCallback() {
            @Override
            public void onSuccess() {}

            @Override
            public void onListGetSucces(List<Plant> plants) {
                if (!plants.isEmpty()) {
                    plantArray.addAll(plants);
                    adapter.notifyDataSetChanged();
                    offset += LIMIT; // Increase offset for next request
                }
                isLoading = false;
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
