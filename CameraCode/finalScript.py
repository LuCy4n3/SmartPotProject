
import requests
import serial
from time import sleep
import os

ser = serial.Serial ("/dev/ttyAMA0", 9600)    # Open port with baud rate
while True:
    received_data = ser.read(3)              # Read serial port
    sleep(0.3)
    data_left = ser.inWaiting()              # Check for remaining bytes
    received_data += ser.read(data_left)

    # Check if the received data matches the expected data
    if received_data == b"test":
        print(received_data)
        print("match found")

    try:
        # Convert the byte sequence to an integer using `from_bytes`
        #integer = int.from_bytes(received_data, byteorder='big')  # 'big' or 'little' based on the byte order
        #print(hex(integer))  # Print the integer as a hexadecimal
        #integer = int(received_data[0],16)
        val = received_data[0]
        if val == 15:
            val = received_data[1]
            if val == 1:
                val = received_data[2]
                if val == 5:
                    print("got data")

                    os.system('libcamera-still -t 10 -n -o test.jpg')

                    url = 'http://roka.go.ro:3000/api/image/upload'
                    files = {'image': ('test.jpg', open('test.jpg', 'rb'),
                                    'multipart/form-data')}

                    response = requests.post(url, files=files)

                    print(response.text)


            else:
                print("Wrong index ")
        else:
            print("wrong header ")
    except ValueError:
        print("Could not convert received data to integer")

    # Write back the received data to the serial port
    #ser.write(received_data)
