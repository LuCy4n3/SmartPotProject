import serial
import time

ser = serial.Serial('/dev/serial0', 9600, timeout=1)

print("UART activ pe Pi Zero 2W...")

while True:
	try:
		ser.write(b'\xAA\x55\x01\x1')
		print("Mesaj trimis spre Pi 5...")
	except Exception as e:
		print(f"Eroare UART: {e})
		time.sleep(2)