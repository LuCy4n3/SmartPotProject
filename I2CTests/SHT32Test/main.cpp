#include <atmel_start.h>
#include <util/delay.h>
#include "MyCode/Sensors/SHT31.h"

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
int32_t bme_sht31_Hummidity = 0;
int32_t mcpTemperature = 0;
uint32_t batteryVoltage = 0;
int32_t bme_sht31_Temperature = 0;
SHT31Class SHT31;
int main(void)
{
	/* Initializes MCU, drivers and middleware */
	atmel_start_init();
	PORTA_set_pin_dir(4,PORT_DIR_OUT);
	/* Replace with your application code */
	int sht31;
	sht31 = SHT31.begin(0x44);
	if(sht31!=0)
	{
		//serialPrintConst("SHT31 failed ");
		//serialPrintNumber(bme_Sht31_InitResult);
		//serialPrintConst(".");
	}
	char intVal[10];
	while (1) {
		SHT31.begin(0x44);
		SHT31.readBoth(&bme_sht31_Temperature,&bme_sht31_Hummidity);
		itoa(bme_sht31_Temperature,intVal,10);
		SerialWrite("Temp:");
		SerialWrite(intVal);
		itoa(bme_sht31_Hummidity,intVal,10);
		SerialWrite("Humidity:");
		SerialWrite(intVal);
		SerialWrite("\r\n");
		//USART_0_write('a');
		PORTA_set_pin_level(4,true);
		_delay_ms(1000);
		PORTA_set_pin_level(4,false);
		_delay_ms(1000);
	}
}
