package com.example.myapplication;
import android.widget.TextView;
import com.android.volley.RequestQueue;
import com.android.volley.Response;
import com.android.volley.VolleyError;
import com.android.volley.toolbox.JsonObjectRequest;
import com.android.volley.toolbox.Volley;
import org.json.JSONException;
import org.json.JSONObject;

public class NetworkRequestThread extends Thread {

    private String url;
    private TextView textView;

    public void NetworkRequestThread(String url, TextView textView) {
        this.url = url;
        if(textView!=null)
        this.textView = textView;
    }



    @Override
    public void run() {
        // Create a request
        JsonObjectRequest jsonObjectRequest = new JsonObjectRequest(
                url,
                null,
                new Response.Listener<JSONObject>() {
                    @Override
                    public void onResponse(JSONObject response) {
                        try {
                            int index = response.getInt("index");
                            int type = response.getInt("type");
                            double temp = response.getDouble("temp");
                            textView.setText(index + "\n" + temp + "\n" + index + "\n");
                        } catch (JSONException e) {
                            throw new RuntimeException(e);
                        }
                    }
                },
                new Response.ErrorListener() {
                    @Override
                    public void onErrorResponse(VolleyError error) {
                        textView.setText(error.toString());
                    }
                });

        // Create a RequestQueue and add the request
        RequestQueue requestQueue = Volley.newRequestQueue(textView.getContext());
        requestQueue.add(jsonObjectRequest);
    }
}