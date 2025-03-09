import serial
import os
import requests

def read_serial():
    # Configure the serial connection
    ser = serial.Serial(
        port='/dev/ttyAMA0',  # UART port
        baudrate=9600,        # Baud rate
        timeout=1             # Timeout for read operation
    )
    
    print("Reading from serial port...")
    
    try:
        while True:
            if ser.in_waiting > 0:
                data = ser.read(ser.in_waiting)  # Read available bytes
                hex_vector = list(data)  # Convert bytes to a vector (list of integers)
                if len(hex_vector) > 2:
                    if hex_vector[0] == 0x15 and hex_vector[1] == 0x1 and hex_vector[2] == 0x5:
                        print(f"Received (vector): {hex_vector}")
                        os.system('libcamera-still -t 10 -n -o test.jpg')
                        
                        url = 'http://roka.go.ro:3000/api/image/upload'
                        with open('test.jpg', 'rb') as img_file:
                            files = {'image': ('test.jpg', img_file, 'multipart/form-data')}
                            response = requests.post(url, files=files)
                        
                        print(response.text)
                    else:
                        print(f"Wrong data : {hex_vector}")
                else:
                    print("Wrong data size")
    except KeyboardInterrupt:
        print("Stopping serial reading...")
    finally:
        ser.close()
        print("Serial connection closed.")

if __name__ == "__main__":
    read_serial()
