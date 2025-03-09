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

public class DashboardFragment extends Fragment {

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
        adapter = new PlantAdapter(plantArray);
        binding.recyclerViewDashBoard.setLayoutManager(new LinearLayoutManager(getContext()));
        binding.recyclerViewDashBoard.setAdapter(adapter);


        binding.recyclerViewDashBoard.addOnScrollListener(new RecyclerView.OnScrollListener() {
            @Override
            public void onScrolled(@NonNull RecyclerView recyclerView, int dx, int dy) {
                LinearLayoutManager layoutManager = (LinearLayoutManager) recyclerView.getLayoutManager();
                if (!isLoading && layoutManager != null && layoutManager.findLastCompletelyVisibleItemPosition() == plantArray.size() - 1) {
                    fetchPlants(binding.searchViewDashBoard.getQuery().toString());
                }
            }
        });
        binding.recyclerViewDashBoard.addOnItemTouchListener(new RecyclerView.OnItemTouchListener() {
            @Override
            public boolean onInterceptTouchEvent(@NonNull RecyclerView recyclerView, @NonNull MotionEvent motionEvent) {

                RecyclerView.ViewHolder viewHolder = recyclerView.findChildViewUnder(motionEvent.getX(), motionEvent.getY());

                if (viewHolder != null) {
                    int position = viewHolder.getAdapterPosition();  // Get the position of the clicked item
                    if (position != RecyclerView.NO_POSITION) {
                        // Get the clicked item (use your adapter to fetch data from the list)
                        Plant clickedPlant = plantArray.get(position);  // Get the Plant object based on position
                        // Handle the click event, e.g., open a detailed view or show a toast
                        if (getActivity() != null) {
                            getActivity().runOnUiThread(() ->
                                    Toast.makeText(getContext(), "Click plant" + clickedPlant.getPlantName(), Toast.LENGTH_LONG).show()
                            );
                        }                    }
                }


                return false;
            }

            @Override
            public void onTouchEvent(@NonNull RecyclerView recyclerView, @NonNull MotionEvent motionEvent) {

            }

            @Override
            public void onRequestDisallowInterceptTouchEvent(boolean b) {

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
    public void onDestroyView() {
        super.onDestroyView();
        binding = null;
    }
}
