package com.example.greengrowtechapp.ui.notifications;

import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.fragment.app.Fragment;
import androidx.lifecycle.Observer;
import androidx.lifecycle.ViewModelProvider;

import com.example.greengrowtechapp.Handlers.JSONresponseHandler;
import com.example.greengrowtechapp.Handlers.NetworkHandler;
import com.example.greengrowtechapp.databinding.FragmentNotificationsBinding;
import com.example.greengrowtechapp.ui.home.HomeViewModel;

public class NotificationsFragment extends Fragment {

    private FragmentNotificationsBinding binding;
    private NotificationsViewModel notViewModel;
    private HomeViewModel homeViewModel;
    private JSONresponseHandler responseHandler = null;
    private NetworkHandler networkHandler = null;



    public View onCreateView(@NonNull LayoutInflater inflater,
                             ViewGroup container, Bundle savedInstanceState) {
        NotificationsViewModel notificationsViewModel =
                new ViewModelProvider(this).get(NotificationsViewModel.class);
        Toast.makeText(getContext(), "Notifications Fragment created!", Toast.LENGTH_SHORT).show();
        binding = FragmentNotificationsBinding.inflate(inflater, container, false);

        View root = binding.getRoot();

        notViewModel = new ViewModelProvider(requireActivity()).get(NotificationsViewModel.class);
        homeViewModel = new ViewModelProvider(requireActivity()).get(HomeViewModel.class);


        notViewModel.getIndexOfCurrentPot().observe(getViewLifecycleOwner(), new Observer<Integer>() {

            @Override
            public void onChanged(Integer integer) {
                //Toast.makeText(getContext(), "got the indexof pot!", Toast.LENGTH_SHORT).show();
                notViewModel.setIndexOfCurrentPot(integer);
            }
        });

        notViewModel.getIndexOfCurrentUser().observe(getViewLifecycleOwner(), new Observer<Integer>() {

            @Override
            public void onChanged(Integer integer) {
                //Toast.makeText(getContext(), "got the indexof user!", Toast.LENGTH_SHORT).show();
                notViewModel.setIndexOfCurrentUser(integer);
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
                }
        });

        final TextView textView = binding.textNotifications;
        notificationsViewModel.getText().observe(getViewLifecycleOwner(), textView::setText);


        return root;
    }

    @Override
    public void onDestroyView() {
        super.onDestroyView();
        binding = null;
    }
}