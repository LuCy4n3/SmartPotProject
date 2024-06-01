/*
 * SHT31.c
 *
 * Created: 5/30/2024 2:28:59 PM
 *  Author: simon
 */ 
#include "SHT31.h"
#include "../include/i2c_master.h"



uint8_t * buffer,temp,hum;
uint8_t i2cAddr;
size_t length;

void errorHandler(i2c_error_t error)
{
	switch(error)
	{
		case I2C_NOERR:
			USART_1_write('n');
			break;
		case I2C_BUSY:
			USART_1_write('b');
			break;
		case I2C_FAIL:
			USART_1_write('f');
			break;
		default:
			USART_1_write('~'); //unhadled error
			break;

	}
}

i2c_error_t begin(uint8_t i2caddr)
{
	i2cAddr = i2caddr;
	I2C_0_init();
	buffer = getBuffer();
	i2c_error_t result = reset();
	errorHandler(result);
	return result;
}

i2c_error_t reset()
{
	i2c_error_t result = writeCommand(SHT31_SOFTRESET,0);
	_delay_ms(10);
	return result;
}

uint16_t readStatus(void) {
	writeCommand(SHT31_READSTATUS,3);

	//uint8_t data[3];
	//i2c_dev->read(data, 3);
	length = getLenght();
	if(length>1)
	{
		uint16_t stat = buffer[0];
		stat <<= 8;
		stat |= buffer[1];
		// Serial.println(stat, HEX);
		return stat;
	}
	else
		return 0xFFFF;
}

i2c_error_t writeCommand(uint16_t command,uint8_t readAmount)
{
		uint8_t cmd[2];
		i2c_error_t error;
		
		cmd[0] = command >> 8;
		cmd[1] = command & 0xFF;
		
		I2C_0_open(SHT31_DEFAULT_ADDR);
		I2C_0_set_address(SHT31_DEFAULT_ADDR);
		I2C_0_set_timeout(50);
		I2C_0_set_buffer(cmd,2);
		error = I2C_0_master_write();
		errorHandler(error);
		if(error != I2C_NOERR)
			return error;
		_delay_ms(10);
		if (readAmount > 0)
		{
			error = I2C_0_master_read();
			errorHandler(error);
			if (error != I2C_NOERR)
				return error;
			
		}
		//set the write buffer 
		//send the buffer 
		return error;
}
static uint8_t crc8(const uint8_t *data, int len) {
    /*
    *
    * CRC-8 formula from page 14 of SHT spec pdf
    *
    * Test data 0xBE, 0xEF should yield 0x92
    *
    * Initialization data 0xFF
    * Polynomial 0x31 (x^8 + x^5 + x^4 + 1)
    * Final XOR 0x00
    */

    const uint8_t POLYNOMIAL = 0x31;
    uint8_t crc = 0xFF;

    for (int j = len; j; --j) {
        crc ^= *data++;

        for (int i = 8; i; --i) {
            crc = (crc & 0x80) ? (crc << 1) ^ POLYNOMIAL : (crc << 1);
        }
    }
    return crc;
}
uint8_t testFunct(uint8_t test)
{
	return test;
}

sht31_error_t readTempHum(void)
{
	uint8_t readBuffer[6];
	uint8_t i2cResponse;
	
	i2cResponse = writeCommand(SHT31_MEAS_HIGHREP,0);
	if (i2cResponse != I2C_NOERR)
		{
			errorHandler(i2cResponse);
			return i2cResponse;
		}
	_delay_ms(20);
	
	I2C_0_set_buffer(readBuffer,6);
	i2cResponse = I2C_0_master_read();
	if(i2cResponse != I2C_NOERR)
	{
		errorHandler(i2cResponse);
		return i2cResponse;
	}
	
	if (readBuffer[2] != crc8(readBuffer, 2) ||
	readBuffer[5] != crc8(readBuffer + 3, 2))
		return CRC8_FAIL;
	
	int32_t stemp = (int32_t)(((uint32_t)readBuffer[0] << 8) | readBuffer[1]);
	// simplified (65536 instead of 65535) integer version of:
	// temp = (stemp * 175.0f) / 65535.0f - 45.0f;
	stemp = ((4375 * stemp) >> 14) - 4500;
	temp = stemp;

	uint32_t shum = ((uint32_t)readBuffer[3] << 8) | readBuffer[4];
	// simplified (65536 instead of 65535) integer version of:
	// humidity = (shum * 100.0f) / 65535.0f;
	shum = (625 * shum) >> 12;
	hum = shum;
	
	return READ_SUCCESS;
}
