package com.example.greengrowtechapp.ui.dashboard;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import com.example.greengrowtechapp.Handlers.NetworkHandler;
import com.example.greengrowtechapp.Handlers.Pot;
import com.example.greengrowtechapp.R;
import com.example.greengrowtechapp.ui.home.HomeViewModel;

import java.util.List;

public class PotAdapter extends RecyclerView.Adapter<PotAdapter.PotViewHolder> {
    private List<Pot> potList;
    private OnItemClickListener itemClickListener;
    private OnButtonClickListener buttonClickListener;
    private NetworkHandler networkHandler;
    private HomeViewModel homeViewModel;

    // Interfaces for click events
    public interface OnItemClickListener {
        void onItemClick(Pot pot);
    }

    public interface OnButtonClickListener {
        void onButtonClick(Pot pot);
    }

    // Constructor
    public PotAdapter(List<Pot> potList, OnItemClickListener itemClickListener, OnButtonClickListener buttonClickListener, NetworkHandler networkHandler,HomeViewModel homeViewModel) {
        this.potList = potList;
        this.itemClickListener = itemClickListener;
        this.buttonClickListener = buttonClickListener;
        this.networkHandler = networkHandler;
        this.homeViewModel = homeViewModel;
    }

    @NonNull
    @Override
    public PotViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_pot, parent, false);
        return new PotViewHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull PotViewHolder holder, int position) {
        Pot pot = potList.get(position);

        // Display Pot Name & Plant Name
        holder.textViewPotName.setText("Pot: " + pot.getPotName());
        holder.textViewPlantName.setText("Plant: " + pot.getPlantName());

        // Handle item click
        holder.itemView.setOnClickListener(v -> {
            if (itemClickListener != null) {
                itemClickListener.onItemClick(pot);
            }
        });

        // Handle button click
        holder.buttonAction.setOnClickListener(v -> {
            if (buttonClickListener != null) {
                buttonClickListener.onButtonClick(pot);
                //networkHandler.sendDeleteRequest(homeViewModel.getURL().getValue(),pot.getPotName(),pot.getPotId());
                Toast.makeText(holder.buttonAction.getContext() ,"Clicked button form menu!"+pot.getPotName(),Toast.LENGTH_LONG).show();
            }
        });
    }

    @Override
    public int getItemCount() {
        return potList.size();
    }

    // ViewHolder Class
    static class PotViewHolder extends RecyclerView.ViewHolder {
        TextView textViewPotName;
        TextView textViewPlantName;
        Button buttonAction;

        PotViewHolder(View itemView) {
            super(itemView);
            textViewPotName = itemView.findViewById(R.id.textViewPotName);
            textViewPlantName = itemView.findViewById(R.id.textViewPlantName);
            buttonAction = itemView.findViewById(R.id.buttonAction);
        }
    }
}
