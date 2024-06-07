/*!
*  @file SHT31.cpp
*
*  @mainpage Adafruit SHT31 Digital Humidity & Temp Sensor
*
*  @section intro_sec Introduction
*
*  This is a library for the SHT31 Digital Humidity & Temp Sensor
*
*  Designed specifically to work with the SHT31 Digital sensor from Adafruit
*
*  Pick one up today in the adafruit shop!
*  ------> https://www.adafruit.com/product/2857
*
*  These sensors use I2C to communicate, 2 pins are required to interface
*
*  Adafruit invests time and resources providing this open source code,
*  please support Adafruit andopen-source hardware by purchasing products
*  from Adafruit!
*
*  @section author Author
*
*  Limor Fried/Ladyada (Adafruit Industries).
*
*  @section license License
*
*  BSD license, all text above must be included in any redistribution
*/
#include "../I2CManager/I2CManager.h"
#include "SHT31.h"
#include <util/delay.h>
/*!
* @brief  SHT31 constructor using i2c
* @param  *theWire
*         optional wire
*/

/**
* Destructor to free memory in use.
*/

/**
* Initialises the I2C bus, and assigns the I2C address to us.
*
* @param i2caddr   The I2C address to use for the sensor.
*
* @return True if initialisation was successful, otherwise False.
*/
uint16_t SHT31Class::begin(uint8_t i2caddr) {
	i2cAddr = i2caddr;
	I2CManager.Init();
	uint16_t result = reset();
	if(result!=0)
	return result;
	return readStatus() != 0xFFFF ? 0 : 88;
}

/**
* Gets the current status register contents.
*
* @return The 16-bit status register.
*/
uint16_t SHT31Class::readStatus(void) {
	writeCommand(SHT31_READSTATUS,3);

	//uint8_t data[3];
	//i2c_dev->read(data, 3);

	uint16_t stat = I2CManager.ReadBuffer[0];
	stat <<= 8;
	stat |= I2CManager.ReadBuffer[1];
	// Serial.println(stat, HEX);
	return stat;
}

/**
* Performs a reset of the sensor to put it into a known state.
*/
uint16_t SHT31Class::reset(void) {
	
	uint16_t result = writeCommand(SHT31_SOFTRESET,0);
	_delay_ms(10);
	return result;
}

/**
* Enables or disabled the heating element.
*
* @param h True to enable the heater, False to disable it.
*/
//void SHT31::heater(bool h) {
//if (h)
//writeCommand(SHT31_HEATEREN);
//else
//writeCommand(SHT31_HEATERDIS);
//_delay_ms(1);
//}
//
///*!
//*  @brief  Return sensor heater state
//*  @return heater state (TRUE = enabled, FALSE = disabled)
//*/
//bool SHT31::isHeaterEnabled() {
//uint16_t regValue = readStatus();
//return (bool)bitRead(regValue, SHT31_REG_HEATER_BIT);
//}

/**
* Gets a single temperature reading from the sensor.
*
* @return A float value indicating the temperature.
*/
float SHT31Class::readTemperature(void) {
	if (!readTempHum())
	return NAN;

	return temp;
}

/**
* Gets a single relative humidity reading from the sensor.
*
* @return A float value representing relative humidity.
*/
float SHT31Class::readHumidity(void) {
	if (!readTempHum())
	return NAN;

	return humidity;
}

/**
* Gets a reading of both temperature and relative humidity from the sensor.
*
* @param temperature_out  Where to write the temperature float.
* @param humidity_out     Where to write the relative humidity float.
* @return True if the read was successful, false otherwise
*/
uint16_t SHT31Class::readBoth(int32_t *temperature_out, int32_t *humidity_out) {
	uint16_t errorCode= readTempHum();
	if (errorCode!=0) {
		*temperature_out = *humidity_out = NAN;
		return errorCode;
	}

	*temperature_out = temp;
	*humidity_out = humidity;
	return errorCode;
}

/**
* Performs a CRC8 calculation on the supplied values.
*
* @param data  Pointer to the data to use when calculating the CRC8.
* @param len   The number of bytes in 'data'.
*
* @return The computed CRC8 value.
*/
static uint8_t crc8(const uint8_t *data, int len) {
	/*
	*
	* CRC-8 formula from page 14 of SHT spec pdf
	*
	* Test data 0xBE, 0xEF should yield 0x92
	*
	* Initialization data 0xFF
	* Polynomial 0x31 (x8 + x5 +x4 +1)
	* Final XOR 0x00
	*/

	const uint8_t POLYNOMIAL(0x31);
	uint8_t crc(0xFF);

	for (int j = len; j; --j) {
		crc ^= *data++;

		for (int i = 8; i; --i) {
			crc = (crc & 0x80) ? (crc << 1) ^ POLYNOMIAL : (crc << 1);
		}
	}
	return crc;
}

/**
* Internal function to perform a temp + humidity read.
*
* @return True if successful, otherwise false.
*/
uint16_t SHT31Class::readTempHum(void) {
	uint8_t readbuffer[6];
	uint8_t i2cResponse;
	
	i2cResponse = writeCommand(SHT31_MEAS_HIGHREP,0);
	if (i2cResponse!=0)
	return i2cResponse+500;
	_delay_ms(20);
	i2cResponse = I2CManager.ReadAmount(i2cAddr,6);
	I2CManager.ReadFromBuffer(readbuffer,6);
	//return false;

	if (readbuffer[2] != crc8(readbuffer, 2) ||
	readbuffer[5] != crc8(readbuffer + 3, 2))
	return 99;

	int32_t stemp = (int32_t)(((uint32_t)readbuffer[0] << 8) | readbuffer[1]);
	// simplified (65536 instead of 65535) integer version of:
	// temp = (stemp * 175.0f) / 65535.0f - 45.0f;
	stemp = ((4375 * stemp) >> 14) - 4500;
	temp = stemp;

	uint32_t shum = ((uint32_t)readbuffer[3] << 8) | readbuffer[4];
	// simplified (65536 instead of 65535) integer version of:
	// humidity = (shum * 100.0f) / 65535.0f;
	shum = (625 * shum) >> 12;
	humidity = shum;

	return 0;
}

/**
* Internal function to perform and I2C write.
*
* @param cmd   The 16-bit command ID to send.
*/
uint16_t SHT31Class::writeCommand(uint16_t command,uint8_t readAmount) {
	uint8_t cmd[2];

	cmd[0] = command >> 8;
	cmd[1] = command & 0xFF;

	
	return I2CManager.WriteRegister(i2cAddr,cmd[0],cmd[1],readAmount);
	//return i2c_dev->write(cmd, 2);
	
}
