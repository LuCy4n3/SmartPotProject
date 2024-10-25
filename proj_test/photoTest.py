
import requests
from time import sleep
import os

while True:
    sleep(0.3)
    os.system('libcamera-still -t 10 -n -o test.jpg')

    url = "http://192.168.0.188:3000/api/image/upload"
    files = {'image': ('test.jpg', open('test.jpg', 'rb'),
                                    'multipart/form-data')}

    response = requests.post(url, files=files)

    print(response.text)
