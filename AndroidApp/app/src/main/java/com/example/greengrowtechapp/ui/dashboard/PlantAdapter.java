package com.example.greengrowtechapp.ui.dashboard;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;
import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;
import com.example.greengrowtechapp.Handlers.Plant;
import com.example.greengrowtechapp.R;

import java.util.List;

public class PlantAdapter extends RecyclerView.Adapter<PlantAdapter.PlantViewHolder> {
    private List<Plant> plantList;
    private OnItemClickListener listener;

    public interface OnItemClickListener {
        void onItemClick(Plant plant);
    }

    public PlantAdapter(List<Plant> plantList, OnItemClickListener listener) {
        this.plantList = plantList;
        this.listener = listener;
    }

    @NonNull
    @Override
    public PlantViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        // Use the new item_plant.xml layout
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_plant, parent, false);
        return new PlantViewHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull PlantViewHolder holder, int position) {
        Plant plant = plantList.get(position);
        holder.textViewPlantName.setText("Plant Name: " + plant.getPlantName());
        holder.textViewSunReq.setText("Sun Requirement: " + plant.getSunReq());

        // Set the click listener for the item
        holder.itemView.setOnClickListener(v -> {
            if (listener != null) {
                listener.onItemClick(plant);
            }
        });
    }

    @Override
    public int getItemCount() {
        return plantList.size();
    }

    static class PlantViewHolder extends RecyclerView.ViewHolder {
        TextView textViewPlantName;
        TextView textViewSunReq;

        PlantViewHolder(View itemView) {
            super(itemView);
            textViewPlantName = itemView.findViewById(R.id.textViewPlantName);
            textViewSunReq = itemView.findViewById(R.id.textViewSunReq);
        }
    }
}