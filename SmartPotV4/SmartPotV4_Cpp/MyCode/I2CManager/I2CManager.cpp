/*
* I2CManager.cpp
*
* Created: 1/16/2021 11:36:31 AM
*  Author: Calin
*/

#include <atmel_start.h>
#include <i2c_types.h>
#include <i2c_master.h>
#include <i2c_simple_master.h>
#include <utils/atomic.h>
#include "i2c_master.h"
#include "I2CManager.h"

typedef struct {
	uint8_t *data;
	uint8_t  size;
} transfer_descriptor_t;

i2c_operations_t I2C_Module_read_handler(void *d)
{
	transfer_descriptor_t *desc = (transfer_descriptor_t *)d;
	I2C_Module_set_buffer((void *)desc->data, desc->size);
	// Set callback to terminate transfer and send STOP after read is complete
	I2C_Module_set_data_complete_callback(i2c_cb_return_stop, NULL);
	return i2c_restart_read; // Send REPEATED START before read
}


I2CManagerClass I2CManager;
void I2CManagerClass::ReadFromBuffer(uint8_t* buffer,uint8_t amountToRead)
{
	for(uint8_t i = 0;i<amountToRead;i++)
	{
		buffer[i] = ReadBuffer[i];
	}
}

void I2CManagerClass::Disable()
{
}

void I2CManagerClass::Enable()
{
	
}

void I2CManagerClass::configure_i2c_master(void)
{
}

void I2CManagerClass::Init()
{
	if(!isInitialized)
	{
		isInitialized = true;
		I2C_Module_init();
	}
}

i2c_error_t I2CManagerClass::WriteRegister(uint8_t address,uint8_t reg,uint8_t countToRead){
	WriteBuffer[0] = reg;
	return WriteRegisterFromInternalBuffer(address,1,countToRead);
}
i2c_error_t I2CManagerClass::WriteRegister(uint8_t address,uint8_t reg,uint8_t firstByte,uint8_t countToRead){
	WriteBuffer[0] = reg;
	WriteBuffer[1] = firstByte;
	return WriteRegisterFromInternalBuffer(address,2,countToRead);
}
i2c_error_t I2CManagerClass::WriteRegister(uint8_t address,uint8_t reg,uint8_t firstByte,uint8_t secondByte,uint8_t countToRead){
	WriteBuffer[0] = reg;
	WriteBuffer[1] = firstByte;
	WriteBuffer[2] = secondByte;
	return WriteRegisterFromInternalBuffer(address,3,countToRead);
}

/** Performs the following transfer sequence:
1. Send SLA+W, Data1
2. Send RepeatedStart, SLA+R, Read Data1, Read Data2
3. Send Stop

This transfer sequence is typically done to first write to the slave the address in
the slave to read from, thereafter to read N bytes from this address.
*/
i2c_error_t I2C_Module_do_transfer(uint8_t addr,uint8_t *data, uint8_t countToWrite,uint8_t countToRead)
{
	/* timeout is used to get out of twim_release, when there is no device connected to the bus*/
	uint16_t              timeout = I2C_TIMEOUT;
	transfer_descriptor_t readDescriptor       = {data, countToRead};

	while (I2C_BUSY == I2C_Module_open(addr) && --timeout)
	; // sit here until we get the bus..
	if (!timeout)
	return I2C_BUSY;
	
	// This callback specifies what to do after the first write operation has completed
	// The parameters to the callback are bundled together in the aggregate data type d.
	if(countToRead>0)
	I2C_Module_set_data_complete_callback(I2C_Module_read_handler, &readDescriptor);
	// If we get an address NACK, then try again by sending SLA+W
	I2C_Module_set_address_nack_callback(i2c_cb_restart_write, NULL);
	// Transmit specified number of bytes
	I2C_Module_set_buffer((void *)data, countToWrite);
	// Start a Write operation
	I2C_Module_master_operation(false);
	timeout = I2C_TIMEOUT;
	
	while (I2C_BUSY == I2C_Module_close() && --timeout)
	; // sit here until finished.
	if (!timeout)
	return I2C_FAIL;
	for(uint8_t i = 0;i<8;i++)
	{
		I2CManager.ReadBuffer[i]	 = I2CManager.WriteBuffer[i];
	}
	return I2C_NOERR;
}

i2c_error_t I2CManagerClass::WriteRegisterFromInternalBuffer(uint8_t address, uint8_t countToWrite,uint8_t countToRead){
	
	//i2c_m_sync_set_slaveaddr(&I2C_0,address,I2C_M_SEVEN);
	//return io_write(&I2C_0.io,WriteBuffer,count+1);
	
	i2c_error_t returnVal = I2C_Module_do_transfer(address,WriteBuffer,countToWrite,countToRead);
	for(uint8_t i = 0;i<8;i++)
	{
		ReadBuffer[i] = WriteBuffer[i];
	}
	return returnVal;
	
	
	
	
	//uint16_t              timeout = I2C_TIMEOUT;
	//while (I2C_BUSY == I2C_Module_open(address) && --timeout)	; // sit here until we get the bus..
	//if (!timeout)
	//return I2C_BUSY;
	
}

static i2c_operations_t I2C_Module_rd2RegCompleteHandler(void *p)
{
	I2C_Module_set_buffer(p, 2);
	I2C_Module_set_data_complete_callback(NULL, NULL);
	return i2c_restart_read;
}


i2c_error_t I2CManagerClass::ReadAmount(uint8_t address, uint8_t amount)
{
	//WriteRegister(address,reg);
	//ReadBuffer[0] = WriteBuffer[0];
	//ReadBuffer[1] = WriteBuffer[1];
	//for(uint8_t i = 0;i<amount;i++)
	//{
	//
	//}
	//return I2C_NOERR;
	
	//I2C_Module_status.address = address;
	I2C_Module_open (address);
	I2C_Module_set_buffer(ReadBuffer, amount);
	I2C_Module_master_read();
	
	uint16_t timeout = I2C_TIMEOUT;
	while (I2C_BUSY == I2C_Module_close() && --timeout)
	; // sit here until finished.
	if (!timeout)
	{
		//serialPrintConst("Read failed");
		return I2C_FAIL;
	}

	return I2C_NOERR;
	
	
	
	
	//i2c_m_sync_set_slaveaddr(&I2C_0,address,I2C_M_SEVEN);
	//return i2c_m_sync_cmd_read(&I2C_0,reg,ReadBuffer,amount);
}

uint8_t I2CManagerClass::ReadOneByte(uint8_t address)
{
	ReadAmount(address,1);
	return ReadBuffer[0];
}

//uint16_t I2CManagerClass::Read16(uint8_t address,uint8_t reg)
//{
//ReadRegisterAmount(address,reg,2);
//return ((uint16_t)ReadBuffer[1] << 8) | ReadBuffer[0];
//}
//void I2CManagerClass::Write16(uint8_t address,uint8_t reg,uint16_t val)
//{
//WriteRegister(address,reg,val >> 8,(uint8_t)val);
//}

