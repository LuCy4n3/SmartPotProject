import requests
import os

url = 'http://roka.go.ro:3000/api/image/upload'


files = {'image': ('test.jpg', open('test.jpg', 'rb'),
                  'multipart/form-data')}

response = requests.post(url, files=files)

print(response.text)
