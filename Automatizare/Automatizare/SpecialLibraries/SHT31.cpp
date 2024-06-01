/*
 * SHT31.cpp
 *
 * Created: 5/28/2024 10:26:20 PM
 *  Author: simon
 */ 

#include "C:\Users\simon\Documents\Atmel Studio\7.0\Automatizare\Automatizare\mcc_generated_files\timer\delay.h"
#include "../mcc_generated_files/i2c_host/i2c_host_event_types.h"
#include "SHT31.h"

bool SHT31Class::writeCommand(uint16_t command,uint8_t readAmount) {
	uint8_t cmd[2];

	cmd[0] = command >> 8;
	cmd[1] = command & 0xFF;

	return i2c_host_interface_t.Write(i2cAddr,cmd,readAmount);
	//return i2c_host_interface_t.WriteRegister(i2cAddr,cmd[0],cmd[1],readAmount);
	//return i2c_dev->write(cmd, 2);
	
}
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