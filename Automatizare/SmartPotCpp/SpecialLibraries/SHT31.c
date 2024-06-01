/*
 * SHT31.c
 *
 * Created: 5/30/2024 2:28:59 PM
 *  Author: simon
 */ 
#include "SHT31.h"
#include <include/i2c_master.h>


uint8_t testFunct(uint8_t test)
{
	return test;
}

uint16_t readTempHum(void)
{
	uint8_t readBuffer[6];
	uint8_t i2cResponse;
	
	return 16;
}
