#include <atmel_start.h>
#include <util/delay.h>
#include <utils/atomic.h>
#include "MyCode/Sensors/SHT31.h"
#define INDEX 1
#define TYPE 2
uint8_t ReadOneByte()
{
	while(!USART_0_is_rx_ready());
	return USART_0_read();
}
void WriteOneByte(uint8_t dataByte)
{
	USART_0_write(dataByte);
	while(!USART_0_is_tx_ready());
}
void serialWrite(uint8_t* data,uint8_t length)
{
	for(uint8_t i = 0;i<length;i++)
	{
		WriteOneByte(data[i]);
	}
}

void serialPrint(uint8_t* data)
{
	uint8_t idx = 0;
	while(data[idx]!=0)
	{
		WriteOneByte(data[idx]);
		idx++;
	}
}

void serialPrintConst(const char* data)
{
	serialPrint((uint8_t*)data);
}

void serialPrintLn(const char* data)
{
	serialPrint((uint8_t*)data);
	serialPrint((uint8_t*)"\r\n");
}

void serialPrintNumber(int nr)
{
	char strBuff[8];
	for(uint8_t i = 0;i<8;i++)
	{
		strBuff[i] = 0;
	}
	itoa(nr,strBuff,10);
	serialPrint((uint8_t*)strBuff);
}
void SerialWrite(const char* data)
{
	uint8_t* pData = (uint8_t*)data;
	while(*pData!=0)
	{
		USART_0_write(*pData);
		pData++;
		//while(!USART1_IsTxReady());
	}
}
void headerHandler(uint8_t utilityId)
{
		WriteOneByte(15);
		WriteOneByte(INDEX);
		WriteOneByte(TYPE);
		WriteOneByte(utilityId);
}
void sendTemp(uint32_t readTemp,uint8_t utilityId)
{
	uint8_t* tempBytes = (uint8_t*)&readTemp;
	headerHandler(utilityId);
	serialPrint(tempBytes);
}
void sendHum(uint32_t readHum,uint8_t utilityId)
{
	uint8_t* humBytes = (uint8_t*)&readHum;
	headerHandler(utilityId);
	serialPrint(humBytes);
}
int32_t bme_sht31_Hummidity = 0;
int32_t mcpTemperature = 0;
uint32_t batteryVoltage = 0;
int32_t bme_sht31_Temperature = 0;
SHT31Class SHT31;
int main(void)
{
	/* Initializes MCU, drivers and middleware */
	ENABLE_INTERRUPTS();
	atmel_start_init();
	
	PORTF_set_pin_dir(5,PORT_DIR_OUT);
	PORTC_set_pin_dir(2,PORT_DIR_OUT);
	PORTC_set_pin_level(2,false);
	/* Replace with your application code */
	int sht31;
	sht31 = SHT31.begin(0x44);
	if(sht31!=0)
	{
		//serialPrintConst("SHT31 failed ");
		//serialPrintNumber(bme_Sht31_InitResult);
		//serialPrintConst(".");
	}
	
	while (1) {
		SHT31.begin(0x44);
		SHT31.readBoth(&bme_sht31_Temperature,&bme_sht31_Hummidity);
		sendTemp(bme_sht31_Temperature,1);
		//SerialWrite("\r\n");
		_delay_ms(5000);
		sendHum(bme_sht31_Hummidity,2);
		//uint8_t* humBytes = (uint8_t*)&bme_sht31_Hummidity;
		//if(ReadOneByte() == 1)
			//PORTC_set_pin_level(2,true);
		//serialPrint(humBytes);
		PORTF_set_pin_level(5,true);
		_delay_ms(3000);
		PORTF_set_pin_level(5,false);
		_delay_ms(3000);
	}
}
