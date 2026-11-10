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
import com.google.gson.Gson;

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
    private MutableLiveData<String> AIresp = new MutableLiveData<>();
    private  MutableLiveData<Integer> errorCode = new MutableLiveData<>();
    public NetworkHandler(Context context,JSONresponseHandler responseHandler) throws JSONException {
        ctx = context;
        this.responseHandler = responseHandler;
        requestQueue = getRequestQueue();
        JSONresponse = new JSONObject("{}");
        errorCode = new MutableLiveData<Integer>(0);
        StringRequest stringRequest = new StringRequest(Request.Method.GET, "/a/15/0",
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
    public void sendAiRequest(String url, String userMessage, AiCallback callback) {
        JSONObject body = new JSONObject();
        try {
            body.put("message", userMessage);  // adapt key to what your backend expects
        } catch (JSONException e) {
            if (callback != null) {
                callback.onFailure("JSON error: " + e.getMessage());
            }
            return;
        }

        JsonObjectRequest jsonObjectRequest = new JsonObjectRequest(
                Request.Method.POST,
                url,
                body,
                new Response.Listener<JSONObject>() {
                    @Override
                    public void onResponse(JSONObject response) {
                        // Example: backend returns { "reply": "..." }
                        String reply = response.optString("reply", "");
                        if (callback != null) {
                            callback.onSuccess(reply);
                        }
                    }
                },
                new Response.ErrorListener() {
                    @Override
                    public void onErrorResponse(VolleyError error) {
                        String msg = (error != null && error.networkResponse != null)
                                ? "AI error " + error.networkResponse.statusCode
                                : "AI error: " + error;

                        // optional: reuse your errorText/errorCode
                        errorText.postValue(msg);
                        errorCode.postValue(-10);

                        if (callback != null) {
                            callback.onFailure(msg);
                        }
                    }
                }
        ) {
            @Override
            public java.util.Map<String, String> getHeaders() {
                java.util.Map<String, String> headers = new java.util.HashMap<>();
                headers.put("Content-Type", "application/json");
                // If your backend needs auth:
                // headers.put("Authorization", "Bearer YOUR_TOKEN");
                return headers;
            }
        };

        // (optional) set retry policy if you want
        jsonObjectRequest.setRetryPolicy(new DefaultRetryPolicy(
                10000,
                DefaultRetryPolicy.DEFAULT_MAX_RETRIES,
                DefaultRetryPolicy.DEFAULT_BACKOFF_MULT
        ));

        getRequestQueue().add(jsonObjectRequest);
    }
    public void createPot(Pot pot,String url) {
        // Define the URL for the POST request
         // Replace with your actual endpoint

        // Convert the Pot object to JSON using Gson
        Gson gson = new Gson();
        String potJson = gson.toJson(pot);

        // Create a StringRequest for the POST request
        StringRequest stringRequest = new StringRequest(Request.Method.POST, url,
                new Response.Listener<String>() {
                    @Override
                    public void onResponse(String response) {
                        // Handle the successful response
                        System.out.println("Pot created successfully: " + response);
                    }
                },
                new Response.ErrorListener() {
                    @Override
                    public void onErrorResponse(VolleyError error) {
                        // Handle the error
                        System.out.println("Error creating pot: " + error.getMessage());
                    }
                }) {
            @Override
            public byte[] getBody() {
                // Return the JSON representation of the Pot object as the request body
                return potJson.getBytes();
            }

            @Override
            public String getBodyContentType() {
                // Specify the content type as JSON
                return "application/json; charset=utf-8";
            }
        };

        // Add the request to the RequestQueue
        requestQueue.add(stringRequest);
    }
    public void sendDeleteRequest(String url, String name, int id, DeleteRequestCallback callback) {
        StringRequest deleteRequest = new StringRequest(Request.Method.DELETE, url + id,
                new Response.Listener<String>() {
                    @Override
                    public void onResponse(String response) {
                        errorCode.postValue(0); // Indicate success
                        Toast.makeText(ctx, "Deleted the pot " + name, Toast.LENGTH_SHORT).show();
                        callback.onSuccess(response); // Notify the caller of success
                    }
                },
                new Response.ErrorListener() {
                    @Override
                    public void onErrorResponse(VolleyError error) {
                        errorText.postValue("DELETE request failed: " + error.toString());
                        errorCode.postValue(-1); // Indicate failure
                        callback.onFailure(error.toString()); // Notify the caller of failure
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
                        //Toast.makeText(ctx, "Got response!", Toast.LENGTH_SHORT).show();

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
                        //Toast.makeText(ctx, "Got pot response!", Toast.LENGTH_SHORT).show();

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
                        if(errorCode.getValue()!=null && errorCode.getValue() >=0)
                        {
                            errorText.postValue(error.toString());
                            errorCode.postValue(-6);
                        }
                        else if(errorCode.getValue() == null)
                            errorText.postValue("Could not connect!");



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
                        //Toast.makeText(ctx, "Got response!", Toast.LENGTH_SHORT).show();// has to run here bcs toas runs on the main thread
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
                if(error!= null && error.networkResponse!=null&& error.networkResponse.statusCode == 404)
                {
                    errorText.postValue("No pots found!");
                    Toast.makeText(ctx, "ERROR no pots found", Toast.LENGTH_SHORT).show();
                }
                else if(errorCode.getValue()!= null && errorCode.getValue()>=0)
                {
                    responseHandler.setError(error.toString());
                    errorCode.postValue(-7);
                }
                else if(errorCode.getValue() == null)
                {
                    responseHandler.setError("cant communicate with server ");
                    errorCode.postValue(-20);
                }

                // Call the failure callback
                if (callback != null) {
                    callback.onFailure();
                }
            }
        });

        getRequestQueue().add(jsonObjectRequest);
    }
    public void sendGetRequestString(String url, NetworkCallback callback) {
        StringRequest stringRequest = new StringRequest(
                Request.Method.GET,
                url,
                new Response.Listener<String>() {
                    @Override
                    public void onResponse(String response) {
                        // If you want to keep JSONresponse for other code, you can skip it here
                        // or wrap the string into a JSON object if needed.

                        // Example: directly pass the string to the callback
                        if (callback != null) {
                            // you’ll need a callback method that accepts String, e.g. onStringSuccess
                            //callback.onStringSuccess(response);
                            AIresp.postValue(response);
                        }
                    }
                },
                new Response.ErrorListener() {
                    @Override
                    public void onErrorResponse(VolleyError error) {
                        if (error != null && error.networkResponse != null
                                && error.networkResponse.statusCode == 404) {
                            AIresp.postValue("Error AT AI");
                            Toast.makeText(ctx, "ERROR on AI", Toast.LENGTH_SHORT).show();
                        }
                        if (callback != null) {
                            callback.onFailure();
                        }
                    }
                }
        );

        getRequestQueue().add(stringRequest);
    }

    public JSONObject getJSONresponse() {
        return JSONresponse;
    }
    public MutableLiveData<String> getErrorText(){return errorText;}
    public MutableLiveData<String> getAIresp(){return AIresp;}
    public MutableLiveData<Integer> getErrorCode(){return errorCode;}
    public interface DeleteRequestCallback {
        void onSuccess(String response); // Called when the request is successful
        void onFailure(String error);    // Called when the request fails
    }
    public interface AiCallback {
        void onSuccess(String reply);
        void onFailure(String error);
    }

}
