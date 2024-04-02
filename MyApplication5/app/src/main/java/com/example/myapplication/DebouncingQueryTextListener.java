package com.example.myapplication;

import android.os.Handler;
import android.os.Looper;
import androidx.appcompat.widget.SearchView;
import androidx.lifecycle.Lifecycle;
import androidx.lifecycle.LifecycleObserver;
import androidx.lifecycle.OnLifecycleEvent;

public class DebouncingQueryTextListener implements SearchView.OnQueryTextListener, LifecycleObserver {
    private final Handler handler = new Handler(Looper.getMainLooper());
    private final Runnable debounceRunnable = new Runnable() {
        @Override
        public void run() {
            onDebouncingQueryTextChange.onDebouncingQueryTextChange(query);
        }
    };

    private final OnDebouncingQueryTextChange onDebouncingQueryTextChange;
    private final long debouncePeriod = 500;
    private String query = "";

    public DebouncingQueryTextListener(OnDebouncingQueryTextChange onDebouncingQueryTextChange) {
        this.onDebouncingQueryTextChange = onDebouncingQueryTextChange;
    }

    @Override
    public boolean onQueryTextSubmit(String query) {
        return false;
    }

    @Override
    public boolean onQueryTextChange(String newText) {
        query = newText;
        handler.removeCallbacks(debounceRunnable);
        handler.postDelayed(debounceRunnable, debouncePeriod);
        return false;
    }

    @OnLifecycleEvent(Lifecycle.Event.ON_DESTROY)
    public void onDestroy() {
        handler.removeCallbacks(debounceRunnable);
    }

    public interface OnDebouncingQueryTextChange {
        void onDebouncingQueryTextChange(String newText);
    }
}
