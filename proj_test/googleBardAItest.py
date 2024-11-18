import os
import google.generativeai as genai

genai.configure(api_key="AIzaSyDSNuqPbWMoHLbcyyliNbJsYLgxNtKKau4")

# Create the model
generation_config = {
  "temperature": 2,
  "top_p": 0.95,
  "top_k": 40,
  "max_output_tokens": 8192,
  "response_mime_type": "text/plain",
}

model = genai.GenerativeModel(
  model_name="gemini-1.5-flash",
  generation_config=generation_config,
)

chat_session = model.start_chat(
  history=[
    {
      "role": "user",
      "parts": [
        "what are the growth parameters for Daylily (Hemerocallis citrina). I want u to give me the answer as simple as u can in the following fields: water preference, life cycle, plant habit, flower color, pH min, pH max, minim temperature, max temperature, sun req, plant height, plant width, fruiting time, flower time, soil type, NPK values, spacing, humidity and the plant name which is just the species of plant. Give the response just as a JSON object with all the params. The names will be:PlantName,PlantGroup,WaterPref,LifeCycle,PlantHabit,FlowerColor,PhMinVal,PhMaxVal,MinTemp,MaxTemp,SunReq,PlantHeight,PlantWidth,FruitingTime,FlowerTime,SoilType,Nitrogen,Phosphorus,Potassium,Spacing,Humidity. Give me the answers for the distances, temps, and humidity in metric without adding the measurement. For distances leave only the max value. For NPK add the values in percentages for solutions per chemical agent and don't add \"%\". Set the humidity in percentage value choose the avg. For the sun req make it in hours.\n\n\n",
      ],
    },
    {
      "role": "model",
      "parts": [
        "```json\n{\n  \"PlantName\": \"Daylily\",\n  \"PlantGroup\": \"Hemerocallis citrina\",\n  \"WaterPref\": \"Moderate\",\n  \"LifeCycle\": \"Perennial\",\n  \"PlantHabit\": \"Clumping\",\n  \"FlowerColor\": \"Yellow\",\n  \"PhMinVal\": 6.0,\n  \"PhMaxVal\": 7.5,\n  \"MinTemp\": -29,\n  \"MaxTemp\": 38,\n  \"SunReq\": 6,\n  \"PlantHeight\": 90,\n  \"PlantWidth\": 60,\n  \"FruitingTime\": \"Summer\",\n  \"FlowerTime\": \"Summer\",\n  \"SoilType\": \"Well-drained\",\n  \"Nitrogen\": 10,\n  \"Phosphorus\": 5,\n  \"Potassium\": 10,\n  \"Spacing\": 45,\n  \"Humidity\": 60\n}\n``` \n",
      ],
    },
  ]
)

response = chat_session.send_message("for the same requests as said before tell me the params for Garlic (Allium sativum)")

print(response.text)