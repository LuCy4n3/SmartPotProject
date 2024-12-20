package com.example.greengrowtechapp.ui.dashboard;

import android.os.Bundle;
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
    private List<Plant> plantArray;

    public View onCreateView(@NonNull LayoutInflater inflater,
                             ViewGroup container, Bundle savedInstanceState) {
        DashboardViewModel dashboardViewModel =
                new ViewModelProvider(this).get(DashboardViewModel.class);
        Toast.makeText(getContext(), "Dash Fragment created!", Toast.LENGTH_SHORT).show();
        binding = FragmentDashboardBinding.inflate(inflater, container, false);

        //binding.searchViewDashBoard.setQuery("", false);


        View root = binding.getRoot();

        dashViewModel = new ViewModelProvider(requireActivity()).get(DashboardViewModel.class);
        homeViewModel = new ViewModelProvider(requireActivity()).get(HomeViewModel.class);


        binding.searchViewDashBoard.clearFocus();
        dashboardViewModel.getPlantArray().observe(getViewLifecycleOwner(), new Observer<List<Plant>>() {
            @Override
            public void onChanged(List<Plant> plants) {
                plantArray = plants;
            }
        });
        dashViewModel.getIndexOfCurrentPot().observe(getViewLifecycleOwner(), new Observer<Integer>() {

            @Override
            public void onChanged(Integer integer) {
                //Toast.makeText(getContext(), "got the indexof pot!", Toast.LENGTH_SHORT).show();
                dashViewModel.setIndexOfCurrentPot(integer);
            }
        });

        dashViewModel.getIndexOfCurrentUser().observe(getViewLifecycleOwner(), new Observer<Integer>() {

            @Override
            public void onChanged(Integer integer) {
                //Toast.makeText(getContext(), "got the indexof user!", Toast.LENGTH_SHORT).show();
                dashViewModel.setIndexOfCurrentUser(integer);
            }
        });
        dashViewModel.getResponseHandler().observe(getViewLifecycleOwner(), new Observer<JSONresponseHandler>() {

            @Override
            public void onChanged(JSONresponseHandler jsoNresponseHandler) {
                responseHandler = jsoNresponseHandler;
                //Toast.makeText(getContext(), "got the shared data!", Toast.LENGTH_SHORT).show();
                if (responseHandler == null) {
                    Toast.makeText(getContext(), "Error: Response handler not found!", Toast.LENGTH_SHORT).show();
                }
            }

        });

        dashViewModel.getNetworkHandler().observe(getViewLifecycleOwner(), new Observer<NetworkHandler>() {
            @Override
            public void onChanged(NetworkHandler netHandler) {
                networkHandler = netHandler;
            }
        });

        binding.listViewDashBoard.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                if (getActivity() != null) {
                    getActivity().runOnUiThread(() -> {
                        String plantName = plantArray.get(position).getPlantName();
                        dashViewModel.setText(plantName);
                        binding.textDashboard.setText(plantName);

                        if (networkHandler != null && homeViewModel != null && homeViewModel.getURL().getValue() != null) {
                            String url = homeViewModel.getURL().getValue() + homeViewModel.getIndexOfCurrentUser().getValue() + "/" + dashViewModel.getIndexOfCurrentPot().getValue();
                            Toast.makeText(getContext(), "Req put on the new url" + url + "!", Toast.LENGTH_SHORT).show();
                            networkHandler.sendPutRequest(url, "PlantName:"+plantName);
                        }

                    });
                }
            }
        });

        binding.buttonUpdate.setOnClickListener(v -> {
            Toast.makeText(getContext(), "set new index of pot!", Toast.LENGTH_SHORT).show();
            networkHandler.sendGetRequestList("http://192.168.201.1:3000/api/Plant/", new NetworkCallback() {
                @Override
                public void onSuccess() {

                }

                @Override
                public void onListGetSucces(List<Plant> plants) {
//                    if (getActivity() != null) {
//                        getActivity().runOnUiThread(() ->
//                                Toast.makeText(getContext(), "got the list data!", Toast.LENGTH_SHORT).show()
//                        );
//                    }
                }

                @Override
                public void onFailure() {
                    if (getActivity() != null) {
                        getActivity().runOnUiThread(() ->
                                Toast.makeText(getContext(), "ERROR didnt get the list data!", Toast.LENGTH_SHORT).show()
                        );
                    }
                }
            });
        });

        dashViewModel.getAdapter().observe(getViewLifecycleOwner(), new Observer<ArrayAdapter<String>>() {
            @Override
            public void onChanged(ArrayAdapter<String> stringArrayAdapter) {
                binding.listViewDashBoard.setVisibility(View.VISIBLE);
                binding.listViewDashBoard.setAdapter(stringArrayAdapter);
            }
        });

        //binding.searchViewDashBoard.clearFocus();
        binding.searchViewDashBoard.setOnQueryTextListener(
                new SearchView.OnQueryTextListener() {
                    @Override
                    public boolean onQueryTextSubmit(String query) {
                        return false;
                    }

                    @Override
                    public boolean onQueryTextChange(String newText) {
                        if(!newText.equals("") && networkHandler!=null) {
                            if(dashViewModel.getURL().getValue() != null ) {
                                String aux = dashViewModel.getURL().getValue() +"'" + newText + "'";
                                NetworkCallback networkCallback = new NetworkCallback() {
                                    @Override
                                    public void onSuccess() {

                                    }

                                    @Override
                                    public void onListGetSucces(List<Plant> plants) {
                                        dashboardViewModel.setPlantArray(plants);
                                        List<String> listViewItems = new ArrayList<>();
                                        for (Plant plant : plants) {
                                            String listItem = "Plant sun req: " + plant.getSunReq() + "\n" +
                                                    "Plant Name: " + plant.getPlantName() + "\n\n";
                                            listViewItems.add(listItem);
                                        }
                                        ArrayAdapter<String> adapter = new ArrayAdapter<String>(getContext(), android.R.layout.simple_list_item_1, listViewItems);
                                        dashViewModel.setAdapter(adapter);
//                                        if (getActivity() != null) {
//                                            getActivity().runOnUiThread(() ->
//                                                    Toast.makeText(getContext(), "got the list data!", Toast.LENGTH_SHORT).show()
//                                            );
//                                        }
                                    }

                                    @Override
                                    public void onFailure() {
                                        if (getActivity() != null) {
                                            getActivity().runOnUiThread(() ->
                                                    Toast.makeText(getContext(), "ERROR didnt get the list data!", Toast.LENGTH_SHORT).show()
                                            );
                                        }
                                    }
                                };
                                networkHandler.sendGetRequestList(aux, networkCallback);
                            }
                        }
                        else
                        {
                            binding.listViewDashBoard.setVisibility(View.INVISIBLE);
                            if(plantArray != null)
                                plantArray.clear();
                        }

                        return true;
                    }
                }
        );

        final TextView textView = binding.textDashboard;
        dashboardViewModel.getText().observe(getViewLifecycleOwner(), new Observer<String>() {
            @Override
            public void onChanged(String s) {
                binding.textDashboard.setText(s);
            }
        });
        return root;
    }

    @Override
    public void onDestroyView() {
//        binding.searchViewDashBoard.setQuery("", false);
//        binding.searchViewDashBoard.clearFocus();
        super.onDestroyView();
        binding = null;
    }
}