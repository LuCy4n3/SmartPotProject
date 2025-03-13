package com.example.greengrowtechapp.ui.dashboard;

import android.os.Bundle;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
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
    private static String[] types = {"No pot","Simple Pot","Advanced Pot","Advanced Pot with GreenHouse","GreenHouse"};

    public View onCreateView(@NonNull LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
        dashViewModel = new ViewModelProvider(requireActivity()).get(DashboardViewModel.class);
        homeViewModel = new ViewModelProvider(requireActivity()).get(HomeViewModel.class);

        binding = FragmentDashboardBinding.inflate(inflater, container, false);
        View root = binding.getRoot();

        Toast.makeText(getContext(), "Dash Fragment created!", Toast.LENGTH_SHORT).show();
        TextView dashText = binding.textDashboard;
        dashText.setText("Trying to connect to server...");
        ArrayAdapter<String> adapter = new ArrayAdapter<>(getContext(),
                android.R.layout.simple_spinner_item, types);
        adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
        binding.spinnerTypeSel.setAdapter(adapter);


        if(dashViewModel.getmTextAddBtn().getValue().equals("Add Pot."))
        {

            binding.recyclerViewDashBrdForPots.setVisibility(View.VISIBLE);
            binding.buttonAddPot.setVisibility(View.INVISIBLE);
            binding.editNamePot.setVisibility(View.INVISIBLE);
            binding.spinnerTypeSel.setVisibility(View.INVISIBLE);
        }
        else if(dashViewModel.getmTextAddBtn().getValue().equals("Back."))
        {

            binding.recyclerViewDashBrdForPots.setVisibility(View.INVISIBLE);
            binding.buttonAddPot.setVisibility(View.VISIBLE);
            binding.editNamePot.setVisibility(View.VISIBLE);
            binding.spinnerTypeSel.setVisibility(View.VISIBLE);
        }


        setupRecyclerView();
        observeViewModels();
        dashViewModel.getmTextAddBtn().observe(getViewLifecycleOwner(), new Observer<String>() {
            @Override
            public void onChanged(String s) {
                binding.buttonAdd.setText(s);
            }
        });
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
        binding.spinnerTypeSel.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {
            @Override
            public void onItemSelected(AdapterView<?> parent, View view, int position, long id) {
                //Toast.makeText(getContext(), "Changed pot index to "+(position)+" !", Toast.LENGTH_SHORT).show();
                dashViewModel.setIndexOfCurrentPot(position);
            }

            @Override
            public void onNothingSelected(AdapterView<?> parent) {

            }
        });
        binding.buttonUpdate.setOnClickListener(v->fetchPots());
        binding.buttonAdd.setOnClickListener(v->changeTextAddBtn());
        binding.buttonAddPot.setOnClickListener(v->addNewPot());

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
                String url = homeViewModel.getURL().getValue();
                networkHandler.sendDeleteRequest(url, pot.getPotName(), pot.getPotId(), new NetworkHandler.DeleteRequestCallback() {
                    @Override
                    public void onSuccess(String response) {
                        binding.buttonUpdate.performClick();
                    }

                    @Override
                    public void onFailure(String error) {

                    }
                });

            }
        },networkHandler,homeViewModel);

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
                    if(networkHandler.getErrorCode() != null && networkHandler.getErrorCode().getValue() < 0 )
                        dashViewModel.setText(String.valueOf(networkHandler.getErrorCode().getValue()));
                    else if(networkHandler.getErrorCode() == null)
                        dashViewModel.setText("Cant communicate with server.");
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
    public void changeTextAddBtn()
    {

        if(dashViewModel.getmTextAddBtn().getValue().equals("Add Pot."))
        {
            binding.recyclerViewDashBrdForPots.setVisibility(View.INVISIBLE);
            binding.buttonAddPot.setVisibility(View.VISIBLE);
            binding.editNamePot.setVisibility(View.VISIBLE);
            binding.spinnerTypeSel.setVisibility(View.VISIBLE);
            dashViewModel.setmTextAddBtn("Back.");
        }
        else
        {
            binding.recyclerViewDashBrdForPots.setVisibility(View.VISIBLE);
            binding.buttonAddPot.setVisibility(View.INVISIBLE);
            binding.editNamePot.setVisibility(View.INVISIBLE);
            binding.spinnerTypeSel.setVisibility(View.INVISIBLE);
            dashViewModel.setmTextAddBtn("Add Pot.");
        }
    }
    private void addNewPot() {
        if(binding.editNamePot.getText()!=null)
        {
            Toast.makeText(getContext(),binding.editNamePot.getText().toString()+" selection for type "+
                    dashViewModel.getIndexOfCurrentPot().getValue(),Toast.LENGTH_LONG).show();
            Pot pot = new Pot(
                    0, // potId (will be generated by the server)
                    binding.editNamePot.getText().toString(), // potName
                    dashViewModel.getIndexOfCurrentPot().getValue(), // potType
                    "Test", // plantName
                    1, // userId
                    false, // hasCamera
                    true, // pictReq
                    false, // pumpStatus
                    false, // greenHouseStatus
                    0, // greenHouseTemperature
                    0, // greenHouseHumidity
                    0, // greenHousePressure
                    0, // potPotassium
                    0, // potPhosphorus
                    0 // potNitrogen
            );

            networkHandler.createPot(pot,homeViewModel.getURL().getValue());
        }
        else
            Toast.makeText(getContext(),"Please add a name for the pot!",Toast.LENGTH_LONG).show();
    }
    @Override
    public void onDestroyView() {
        super.onDestroyView();
        binding = null;
    }

}
