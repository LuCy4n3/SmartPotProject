import json
import re

# Function to parse and serialize JSON, handling extra formatting
def parse_and_serialize_json(json_text):
    try:
        # Clean up the JSON text by removing any non-JSON prefix/suffix
        # For example, this will remove '```json' at the start and '```' at the end if present
        json_text = json_text.strip("'''json").strip("```").strip()  # Removes leading/trailing backticks and whitespace

        print(json_text)
        # Parse the cleaned JSON text into a Python dictionary
        data = json.loads(json_text)
        
        # Serialize the dictionary back to a JSON-formatted string with indentation
        serialized_data = json.dumps(data, indent=2)
        
        return serialized_data
    
    except json.JSONDecodeError:
        return "Invalid JSON format"

# Example JSON string input with extra characters
json_text = '''json
{
  "PlantName": "Garlic",
  "PlantGroup": "Allium sativum",
  "WaterPref": "Moderate",
  "LifeCycle": "Annual",
  "PlantHabit": "Bulb",
  "FlowerColor": "White",
  "PhMinVal": 6.0,
  "PhMaxVal": 7.0,
  "MinTemp": -10,
  "MaxTemp": 30,
  "SunReq": 6,
  "PlantHeight": 60,
  "PlantWidth": 15,
  "FruitingTime": "Summer",
  "FlowerTime": "Spring",
  "SoilType": "Loamy",
  "Nitrogen": 10,
  "Phosphorus": 10,
  "Potassium": 10,
  "Spacing": 20,
  "Humidity": 50
}
'''

# Call the function and print the result
print(parse_and_serialize_json(json_text))
