package com.example.greengrowtechapp.Handlers;

import android.content.Context;
import android.widget.Toast;

import com.android.volley.Request;
import com.android.volley.RequestQueue;
import com.android.volley.Response;
import com.android.volley.VolleyError;
import com.android.volley.toolbox.JsonArrayRequest;
import com.android.volley.toolbox.JsonObjectRequest;
import com.android.volley.toolbox.StringRequest;
import com.android.volley.toolbox.Volley;
import com.example.greengrowtechapp.NetworkCallback;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.io.Serializable;
import java.util.List;

public class NetworkHandler implements Serializable {
    private static NetworkHandler instance;
    private RequestQueue requestQueue;
    private static Context ctx;
    private JSONObject JSONresponse;
    private JSONresponseHandler responseHandler;
    public NetworkHandler(Context context,JSONresponseHandler responseHandler) throws JSONException {
        ctx = context;
        this.responseHandler = responseHandler;
        requestQueue = getRequestQueue();
        JSONresponse = new JSONObject("{}");
    }

    public static synchronized NetworkHandler getInstance(Context context,JSONresponseHandler responseHandler) throws JSONException {
        if (instance == null) {
            instance = new NetworkHandler(context,responseHandler);
        }
        return instance;
    }

    public RequestQueue getRequestQueue() {
        if (requestQueue == null) {
            requestQueue = Volley.newRequestQueue(ctx.getApplicationContext());
        }
        return requestQueue;
    }

    public void sendPutRequest(String url,String header) {
        Response.Listener<String> listener = new Response.Listener<String>() {
            @Override
            public void onResponse(String response) {
                //to do:add response handler
            }
        };
        Response.ErrorListener errorListener = new Response.ErrorListener() {
            @Override
            public void onErrorResponse(VolleyError error) {
                //to do: add response handler
            }
        };
        StringRequest stringRequest = new StringRequest(Request.Method.PUT, url+"/"+header, listener, errorListener);
        Toast.makeText(ctx, "Sent "+url+"/"+header+" PUT req", Toast.LENGTH_SHORT).show();
        getRequestQueue().add(stringRequest);
    }
    public void sendGetRequestList(String url, NetworkCallback callback) {
        JsonArrayRequest jsonArrayRequest = new JsonArrayRequest(url,
                new Response.Listener<JSONArray>() {
                    @Override
                    public void onResponse(JSONArray response) {
                        Toast.makeText(ctx, "Got response!", Toast.LENGTH_SHORT).show();

                        new Thread(() -> {
                            try {
                                // Parse the JSON response into a list of Plant objects
                                List<Plant> plantList = responseHandler.parsePlantData(response);

                                // Call the success callback with the list of plants
                                if (callback != null) {
                                    callback.onListGetSucces(plantList);
                                }
                            } catch (JSONException e) {
                                e.printStackTrace();
                                if (callback != null) {
                                    callback.onFailure();
                                }
                            }
                        }).start();
                    }
                },
                new Response.ErrorListener() {
                    @Override
                    public void onErrorResponse(VolleyError error) {
                        Toast.makeText(ctx, "ERROR " + url + " !", Toast.LENGTH_SHORT).show();
                        if (callback != null) {
                            callback.onFailure();
                        }
                    }
                }
        );

        getRequestQueue().add(jsonArrayRequest);
    }
    public void sendGetRequest(String url, NetworkCallback callback) {
        JsonObjectRequest jsonObjectRequest = new JsonObjectRequest(url, null,
                new Response.Listener<JSONObject>() {
                    @Override
                    public void onResponse(JSONObject response) {
                        Toast.makeText(ctx, "Got response!", Toast.LENGTH_SHORT).show();// has to run here bcs toas runs on the main thread
                        new Thread(() -> {
                            JSONresponse = response;
                            try {
                                responseHandler.parsePotData(response);


                                // Call the success callback
                                if (callback != null) {
                                    callback.onSuccess();
                                }
                            } catch (JSONException e) {
                                throw new RuntimeException(e);
                            }
                        }).start();
                    }
                }, new Response.ErrorListener() {
            @Override
            public void onErrorResponse(VolleyError error) {
                Toast.makeText(ctx, "ERROR!", Toast.LENGTH_SHORT).show();

                // Call the failure callback
                if (callback != null) {
                    callback.onFailure();
                }
            }
        });

        getRequestQueue().add(jsonObjectRequest);
    }

    public JSONObject getJSONresponse() {
        return JSONresponse;
    }
}