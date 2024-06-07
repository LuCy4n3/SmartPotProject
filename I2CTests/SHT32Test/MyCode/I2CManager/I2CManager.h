/*
 * I2CManager.h
 *
 * Created: 1/16/2021 11:36:39 AM
 *  Author: Calin
 */ 


#ifndef I2CMANAGER_H_
#define I2CMANAGER_H_


#include "driver_init.h"

//#include "../../#include "driver_init.h""

class I2CManagerClass
{
	public:
	void Init();
	void ReadFromBuffer(uint8_t* buffer,uint8_t amountToRead);
	i2c_error_t WriteRegister(uint8_t address,uint8_t reg,uint8_t countToRead);
	i2c_error_t WriteRegister(uint8_t address,uint8_t reg,uint8_t firstByte,uint8_t countToRead);
	i2c_error_t WriteRegister(uint8_t address,uint8_t reg,uint8_t firstByte,uint8_t secondByte,uint8_t countToRead);
	void Disable();
	void Enable();
	uint8_t ReadOneByte(uint8_t address);
	i2c_error_t ReadAmount(uint8_t address, uint8_t amount);
	uint16_t Read16(uint8_t address,uint8_t reg);
	void Write16(uint8_t address,uint8_t reg,uint16_t val);
	//void ReadAmount(uint8_t address,uint8_t amount);
	uint8_t WriteBuffer[16];
	uint8_t ReadBuffer[16];
	//struct io_descriptor *I2C_1_io;
	private:
	i2c_error_t WriteRegisterFromInternalBuffer(uint8_t address, uint8_t countToRead, uint8_t countToWrite);
	void configure_i2c_master(void);
	
	//struct i2c_master_packet currentPacket;
	//struct i2c_master_module i2c_master_instance;
	bool isInitialized = false;
};

extern I2CManagerClass I2CManager;
#endif /* I2CMANAGER_H_ */