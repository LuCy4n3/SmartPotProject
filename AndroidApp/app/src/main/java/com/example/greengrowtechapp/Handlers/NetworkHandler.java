package com.example.greengrowtechapp.Handlers;

import android.content.Context;
import android.graphics.Bitmap;
import android.widget.ImageView;
import android.widget.Toast;

import androidx.lifecycle.MutableLiveData;

import com.android.volley.DefaultRetryPolicy;
import com.android.volley.Request;
import com.android.volley.RequestQueue;
import com.android.volley.Response;
import com.android.volley.VolleyError;
import com.android.volley.toolbox.ImageRequest;
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

    private MutableLiveData<String> errorText = new MutableLiveData<>(); // Use LiveData
    private  MutableLiveData<Integer> errorCode = new MutableLiveData<>();
    public NetworkHandler(Context context,JSONresponseHandler responseHandler) throws JSONException {
        ctx = context;
        this.responseHandler = responseHandler;
        requestQueue = getRequestQueue();
        JSONresponse = new JSONObject("{}");
        StringRequest stringRequest = new StringRequest(Request.Method.GET, "http://192.168.0.192:3000/api/Plant/a/15/0",
                new Response.Listener<String>() {
                    @Override
                    public void onResponse(String response) {
                        // Handle the response
                        //errorText.postValue("Ping successful!"); // Update LiveData
                        errorCode.postValue(0);
                    }
                },
                new Response.ErrorListener() {
                    @Override
                    public void onErrorResponse(VolleyError error) {
                        // Handle the error
                        errorText.postValue("Ping failed!"); // Update LiveData
                        errorCode.postValue(-1);
                    }
                });

        // Add the request to the RequestQueue
        requestQueue.add(stringRequest);
    }
    public void sendDeleteRequest(String url,String name) {
        StringRequest deleteRequest = new StringRequest(Request.Method.DELETE, url,
                new Response.Listener<String>() {
                    @Override
                    public void onResponse(String response) {
                        errorCode.postValue(0); // Indicate success
                        Toast.makeText(ctx, "Deleted the pot "+name , Toast.LENGTH_SHORT).show();

                        //Log.d("DELETE_REQUEST", "DELETE request successful");
                    }
                },
                new Response.ErrorListener() {
                    @Override
                    public void onErrorResponse(VolleyError error) {
                        errorText.postValue("DELETE request failed: " + error.toString());
                        errorCode.postValue(-1); // Indicate failure
                        //Log.e("DELETE_REQUEST", "DELETE request failed: " + error.toString());
                    }
                });

        // Add the request to the RequestQueue
        requestQueue.add(deleteRequest);
    }
    public void sendImageRequest(String url, final ImageView imageView, final NetworkCallback callback) {
        // Create an ImageRequest
        ImageRequest imageRequest = new ImageRequest(url,
                new Response.Listener<Bitmap>() {
                    @Override
                    public void onResponse(Bitmap response) {
                        imageView.setImageBitmap(response);
                        //errorText = "No error";
                        if (callback != null) {
                            callback.onSuccess();
                        }
                    }
                },
                0, 0, ImageView.ScaleType.CENTER_CROP, Bitmap.Config.RGB_565,
                new Response.ErrorListener() {
                    @Override
                    public void onErrorResponse(VolleyError error) {
                        Toast.makeText(ctx, "ERROR! " + error.toString(), Toast.LENGTH_SHORT).show();
                        errorText.postValue(error.toString());
                        errorCode.postValue(-2);
                        // Call the failure callback
                        if (callback != null) {
                            callback.onFailure();
                        }
                    }
                });

        getRequestQueue().add(imageRequest);
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
                //errorText.postValue("No error");
                //todo:add response handler
            }
        };
        Response.ErrorListener errorListener = new Response.ErrorListener() {
            @Override
            public void onErrorResponse(VolleyError error) {
                if(errorCode.getValue() >= 0)
                {
                    errorText.postValue(error.toString());
                    errorCode.postValue(-3);
                }

                //TODO: add response handler
            }
        };
        StringRequest stringRequest = new StringRequest(Request.Method.PUT, url+"/"+header, listener, errorListener);
        Toast.makeText(ctx, "Sent "+url+"/"+header+" PUT req", Toast.LENGTH_SHORT).show();
        getRequestQueue().add(stringRequest);
    }
    public void sendGetRequestListPlant(String url, NetworkCallback callback) {
        JsonArrayRequest jsonArrayRequest = new JsonArrayRequest(url,
                new Response.Listener<JSONArray>() {
                    @Override
                    public void onResponse(JSONArray response) {
                        Toast.makeText(ctx, "Got response!", Toast.LENGTH_SHORT).show();

                        try {
                            // Process JSON in chunks to avoid memory issues
                            List<Plant> plantList = responseHandler.parsePlantDataList(response);
                            //errorText.postValue("No error");

                            if (callback != null) {
                                callback.onPlantListGetSucces(plantList);
                            }
                        } catch (JSONException e) {
                            errorText.postValue(e.toString());
                            errorCode.postValue(-5);
                            e.printStackTrace();
                            if (callback != null) {
                                callback.onFailure();
                            }
                        }
                    }
                },
                new Response.ErrorListener() {
                    @Override
                    public void onErrorResponse(VolleyError error) {
                        Toast.makeText(ctx, "ERROR " + url + " !", Toast.LENGTH_SHORT).show();
                        errorText.postValue(error.toString());
                        errorCode.postValue(-6);
                        if (callback != null) {
                            callback.onFailure();
                        }
                    }
                });

        // Set timeout & retry policy to prevent memory overload
        jsonArrayRequest.setRetryPolicy(new DefaultRetryPolicy(
                10000, // 10 seconds timeout
                DefaultRetryPolicy.DEFAULT_MAX_RETRIES,
                DefaultRetryPolicy.DEFAULT_BACKOFF_MULT));

        getRequestQueue().add(jsonArrayRequest);
    }

    public void sendGetRequestListPot(String url, NetworkCallback callback) {
        JsonArrayRequest jsonArrayRequest = new JsonArrayRequest(Request.Method.GET, url, null,
                new Response.Listener<JSONArray>() {
                    @Override
                    public void onResponse(JSONArray response) {
                        Toast.makeText(ctx, "Got pot response!", Toast.LENGTH_SHORT).show();

                        try {
                            // Process JSON response into a list of Pot objects
                            List<Pot> potList = responseHandler.parsePotDataList(response);

                            if (callback != null) {
                                callback.onPotListGetSucces(potList);
                            }
                        } catch (JSONException e) {
                            errorText.postValue(e.toString());
                            errorCode.postValue(-5);
                            e.printStackTrace();
                            if (callback != null) {
                                callback.onFailure();
                            }
                        }
                    }
                },
                new Response.ErrorListener() {
                    @Override
                    public void onErrorResponse(VolleyError error) {
                        Toast.makeText(ctx, "ERROR " + url + " !", Toast.LENGTH_SHORT).show();
                        if(errorCode.getValue() >=0)
                        {
                            errorText.postValue(error.toString());
                            errorCode.postValue(-6);
                        }

                        if (callback != null) {
                            callback.onFailure();
                        }
                    }
                });

        // Set timeout & retry policy to prevent memory overload
        jsonArrayRequest.setRetryPolicy(new DefaultRetryPolicy(
                10000, // 10 seconds timeout
                DefaultRetryPolicy.DEFAULT_MAX_RETRIES,
                DefaultRetryPolicy.DEFAULT_BACKOFF_MULT));

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

                            responseHandler.parsePotData(response);
                            //errorText.postValue("No error");


                            // Call the success callback
                            if (callback != null) {
                                callback.onSuccess();
                            }

                        }).start();
                    }
                }, new Response.ErrorListener() {
            @Override
            public void onErrorResponse(VolleyError error) {
                if(error.networkResponse.statusCode == 404)
                {
                    errorText.postValue("No pots found!");
                    Toast.makeText(ctx, "ERROR no pots found", Toast.LENGTH_SHORT).show();
                }
                else if(errorCode.getValue()>=0)
                {
                    responseHandler.setError(error.toString());
                    errorCode.postValue(-7);
                }

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
    public MutableLiveData<String> getErrorText(){return errorText;}
    public MutableLiveData<Integer> getErrorCode(){return errorCode;}
}
