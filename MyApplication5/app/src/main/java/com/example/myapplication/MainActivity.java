package com.example.myapplication;

import android.os.Bundle;
import android.os.Handler;
import android.os.Looper;
import android.util.Log;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.ListView;
import android.widget.Spinner;
import android.widget.TextView;

import androidx.appcompat.app.AppCompatActivity;
import androidx.appcompat.widget.SearchView;

import com.android.volley.RequestQueue;
import com.android.volley.Response;
import com.android.volley.VolleyError;
import com.android.volley.toolbox.JsonObjectRequest;
import com.android.volley.toolbox.Volley;

import org.json.JSONException;
import org.json.JSONObject;

import java.util.ArrayList;
import java.util.List;

public class MainActivity extends AppCompatActivity implements AdapterView.OnItemSelectedListener {
    // private Button button;
    TextView textView2, textView;
    private androidx.appcompat.widget.SearchView searchView;
    private String plantName;
    private ListView listView;
    private ArrayAdapter<String> adapter;
    private List<String> dataset = new ArrayList<>();
    private Handler handler = new Handler(Looper.getMainLooper());
    private int delayMillis = 1000; // Set your desired delay in milliseconds
    private Spinner spinner;
    private Button button, button2;
    String indexOfPot="1";
    int index, type;
    boolean sera, pompa;
    double temp, lumi, hum, pot, pho, ni, tempExt, humExt;
    public static String url = " http://192.168.201.1:3000";
    private static final String[] paths = {"Dispozitiv 1", "Dispozitiv 2", "Dispozitiv 3", "Dispozitiv 4", "Dispozitiv 5"};

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        textView = findViewById(R.id.textView);
        textView2 = findViewById(R.id.textView2);
        listView = findViewById(R.id.listView);

        spinner = findViewById(R.id.spinner);
        ArrayAdapter<String> adapter = new ArrayAdapter<>(MainActivity.this,
                android.R.layout.simple_spinner_item, paths);
        adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
        spinner.setAdapter(adapter);
        spinner.setOnItemSelectedListener(this);
        button = findViewById(R.id.button);
        button2 = findViewById(R.id.button2);
        button.setVisibility(View.INVISIBLE);
        button2.setVisibility(View.INVISIBLE);
        listView.setVisibility(View.INVISIBLE);
        PlantRepository plantRepository = new PlantRepository();
        // Button click listeners
        button.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                pompa = !pompa; // Toggle pompa value
                sendPutRequest();
            }
        });

        button2.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                sera = !sera; // Toggle sera value
                sendPutRequest();
            }
        });
        try {
            searchView= findViewById(R.id.searchView);
            searchView.clearFocus();
            textView2.setText("start");
            searchView.setOnQueryTextListener(new SearchView.OnQueryTextListener() {
                @Override
                public boolean onQueryTextSubmit(String query) {
                    return false;
                }

                @Override
                public boolean onQueryTextChange(String newText) {
                    PlantRepository plantRepository = new PlantRepository();
                    plantRepository.getAllPlants(new PlantRepository.PlantCallback<List<Plant>>() {
                        @Override
                        public void onSuccess(List<Plant> plants) {
                            String targetPlantGroup = newText;
                            //textView2.setText(newText);
                            // Create a new list to store filtered plants
                            List<Plant> filteredPlants = new ArrayList<>();
                            if (!targetPlantGroup.equals(""))
                                // Iterate through each plant
                            {
                                for (Plant plant : plants) {
                                    // Check if the plantGroup matches the target value
                                    if (plant.getPlantGroup().toLowerCase().contains(targetPlantGroup.toLowerCase())) {
                                        // If it matches, add the plant to the filtered list
                                        filteredPlants.add(plant);
                                    }
                                }

                            List<String> listViewItems = new ArrayList<>();
                            for (Plant filteredPlant : filteredPlants) {
                                String listItem = "Plant sun req: " + filteredPlant.getSunReq() + "\n" +
                                        "Plant Name: " + filteredPlant.getPlantName() + "\n\n";
                                listViewItems.add(listItem);
                            }

                            ArrayAdapter<String> adapter = new ArrayAdapter<String>(getApplicationContext(), android.R.layout.simple_list_item_1, listViewItems);
                            listView.setVisibility(View.VISIBLE);
                            listView.setAdapter(adapter);
                            listView.setOnItemClickListener(new AdapterView.OnItemClickListener() {
                                    @Override
                                    public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                                        // Handle item click here
                                        //String selectedItem = (String) parent.getItemAtPosition(position);
                                        plantName=filteredPlants.get(position).getPlantName();
                                        sendPutRequest();
                                        textView2.setText("Item selected: " + plantName);
                                    }
                            });

                            }
                            else
                                listView.setVisibility(View.INVISIBLE);
                                //textView2.setText(stringBuilder.toString());
                            }


                        @Override
                        public void onError(Throwable throwable) {
                            // Handle error
                            textView2.setText("nu am reusit");
                        }
                    });
                    // Start periodic update
                    handler.postDelayed(new Runnable() {
                        @Override
                        public void run() {
                            updateJsonRequest();
                            handler.postDelayed(this, delayMillis);
                        }
                    }, delayMillis);

                    return true;
                }
            });
        } catch (Exception e) {
            Log.d("Error",e.toString());
            throw new RuntimeException(e);
        }

        // Create an instance of DebouncingQueryTextListener
        handler.postDelayed(new Runnable() {
            @Override
            public void run() {
                updateJsonRequest();
                handler.postDelayed(this, delayMillis);
            }
        }, delayMillis);


    }

    private void filterData(String searchText) {
        List<String> filteredData = new ArrayList<>();
        for (String item : dataset) {
            if (item.toLowerCase().contains(searchText.toLowerCase())) {
                filteredData.add(item);
            }
        }
        adapter.clear();
        adapter.addAll(filteredData);
        adapter.notifyDataSetChanged();
    }



    private void sendPutRequest() {
        JSONObject jsonBody = new JSONObject();
        try {
            jsonBody.put("index", index);
            jsonBody.put("plantName",plantName);
            jsonBody.put("type", type);
            jsonBody.put("temp", temp);
            jsonBody.put("pompa",pompa);
            jsonBody.put("sera",sera);
            jsonBody.put("humidity", hum);
            jsonBody.put("potassium", pot);
            jsonBody.put("phosphor", pho);
            jsonBody.put("nitrogen", ni);
            jsonBody.put("tempExt", tempExt);
            jsonBody.put("humExt", humExt);
        } catch (JSONException e) {
            e.printStackTrace();
        }

        JsonObjectRequest jsonObjectRequest = new JsonObjectRequest(JsonObjectRequest.Method.PUT, url+"/pots",
                jsonBody,
                new Response.Listener<JSONObject>() {
                    @Override
                    public void onResponse(JSONObject response) {
                        // Handle the response if needed
                        // Note: You may want to check the response and update UI accordingly
                    }
                },
                new Response.ErrorListener() {
                    @Override
                    public void onErrorResponse(VolleyError error) {
                        textView.setText("Error: " + error.toString());
                        // Handle the error and check the error.getMessage() for more details
                    }
                }) {
            @Override
            public String getBodyContentType() {
                return "application/json";
            }

            @Override
            public byte[] getBody() {
                return jsonBody.toString().getBytes();
            }
        };

        RequestQueue requestQueue = Volley.newRequestQueue(MainActivity.this);
        requestQueue.add(jsonObjectRequest);
    }
    private void updateJsonRequest() {
        Integer aux= spinner.getSelectedItemPosition()+1;
        indexOfPot=aux.toString().trim();
        JsonObjectRequest jsonObjectRequest = new JsonObjectRequest(url+"/pots/" + indexOfPot,
                null,
                new Response.Listener<JSONObject>() {
                    @Override
                    public void onResponse(JSONObject response) {
                        try {
                            if (response.has("index")) {
                                index = response.getInt("index");
                            }
                            if(response.has("plantName"))
                            {
                                plantName = response.getString("plantName");
                            }
                            if (response.has("type")) {
                                type = response.getInt("type");
                            }
                            if (response.has("pompa")) {
                                pompa = response.getBoolean("pompa");
                            }if (response.has("sera")) {
                                sera = response.getBoolean("sera");
                            }

                            if (response.has("temp")) {
                                temp = response.getDouble("temp");
                            }
                            if (response.has("humidity")) {
                                hum = response.getDouble("humidity");
                            }
                            if (response.has("potassium")) {
                                pot = response.getDouble("potassium");
                            }
                            if (response.has("phosphor")) {
                                pho = response.getDouble("phosphor");
                            }
                            if (response.has("nitrogen")) {
                                ni = response.getDouble("nitrogen");
                            }
                            if (response.has("tempExt")) {
                                tempExt = response.getDouble("tempExt");
                            }
                            if (response.has("humExt")) {
                                humExt = response.getDouble("humExt");
                            }
                            updateTextView();
                            // Handle the data based on your requirements

                        } catch (JSONException e) {
                            throw new RuntimeException(e);
                        }
                    }
                },
                new Response.ErrorListener() {
                    @Override
                    public void onErrorResponse(VolleyError error) {
                        textView.setText("eroare api pots:"+error.toString());
                    }
                });
        RequestQueue requestQueue= Volley.newRequestQueue(MainActivity.this);
        requestQueue.add(jsonObjectRequest);

        // Note: You may want to move the switch statement to handle the response
        // inside onResponse, depending on your requirements.
    }
    private void updateTextView() {
        // Handle the data based on your requirements
        switch (type) {
            case 0:
                textView.setText("Nu exista givechi!");
                break;
            case 1:
                textView.setText("Ghiveciul " + index + "\n"+plantName+" Planta in ghiveci"+"\n" + temp + " Temperatura \n" + hum + " Umiditate \n" + tempExt + " Temperatura Exterioara\n" + humExt + " Umiditate Exterioara\n");
                break;
            case 2:
                textView.setText("Ghiveciul " + index + "\n"+plantName+" Planta in ghiveci"+"\n"  + temp + " Temperatura \n" + hum + " Umiditate \n" + pot + " Potasiu \n" + ni + " Azot \n" + pho + " Phospor \n" + tempExt + " Temperatura Exterioara\n" + humExt + "Umiditate Exterioara\n");
                break;
            case 3:
                textView.setText("Ghiveciul " + index + "beneficeaza si sera \n"+plantName+" Planta in ghiveci"+"\n"  + temp + " Temperatura \n" + hum + "Umiditate \n" + pot + "Potasiu \n" + ni + " Azot \n" + pho + " Phospor \n" + tempExt + " Temperatura Exterioara\n" + humExt + " Umiditate Exterioara\n");
                break;
            case 4:
                textView.setText("Sera " + index + "\n"+plantName+" Planta in ghiveci"+"\n"  + temp + " Temperatura Exterioara\n" + hum + " Umiditate Exterioara\n");
                break;
            default:
                // Handle the default case if needed
                break;
        }
        if(type==3)
        {
            button.setVisibility(View.VISIBLE);
            button2.setVisibility(View.VISIBLE);
        }
        else if(type==4)
        {
            button.setVisibility(View.INVISIBLE);
            button2.setVisibility(View.VISIBLE);

        }
        else if(type==2)
        {
            button2.setVisibility(View.INVISIBLE);
            button.setVisibility(View.VISIBLE);

        }
        else
        {
            button2.setVisibility(View.INVISIBLE);
            button.setVisibility(View.INVISIBLE);
        }
    }

    public void onItemSelected(AdapterView<?> parent, View v, int position, long id) {

        switch (position) {
            case 0:
                // Whatever you want to happen when the first item gets selected
                 indexOfPot="1";
                break;
            case 1:
                // Whatever you want to happen when the second item gets selected
                 indexOfPot="2";
                break;
            case 2:
                // Whatever you want to happen when the thrid item gets selected
                 indexOfPot="3";
                break;
            case 3:
                indexOfPot="4";
                break;
            case 4:
                indexOfPot="5";
                break;

        }



    }


    public void onNothingSelected(AdapterView<?> parent) {
        //textView.setText("Sera " + index + "\n" + tempExt + " Temperatura Exterioara\n" + humExt + " Umiditate Exterioara\n");
    }

}
