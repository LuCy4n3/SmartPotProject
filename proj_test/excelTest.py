# Import the necessary libraries
import openpyxl
import pandas as pd

# Create a new Excel workbook
workbook = openpyxl.Workbook()
# Select the default sheet (usually named 'Sheet')
sheet = workbook.active
# Add data to the Excel sheet
data = [
    ["Name", "Age", "City"],
    ["John", 28, "New York"],
    ["Alice", 24, "San Francisco"],
    ["Bob", 32, "Los Angeles"]
]
for row in data:
    sheet.append(row)
# Save the workbook to a file
workbook.save("my_excel_file.xlsx")
sheet['A1'] = 42

# Rows can also be appended
sheet.append([1, 2, 3])

# Python types will automatically be converted
import datetime
sheet['A2'] = datetime.datetime.now()

# Save the file
workbook.save("sample.xlsx")
# Print a success message
print("Excel file created successfully!")