/*
 * SmartPotV4_Cpp.cpp
 *
 * Created: 1/5/2025 10:15:57 PM
 * Author : simon
 */ 

#include <avr/io.h>
#include <atmel_start.h>
#include <util/delay.h>
#include <smartpot/CallbackFun.h>
#include "MyCode/Sensors/SHT31.h"

static uint8_t *bufferToSend;
static uint8_t *bufferToRead;

uint8_t SPI_ADC_test_spi(void)
{

	// Register callback function releasing SS when transfer is complete
	SPI_ADC_register_callback(drive_slave_select_high_custom);
	writeOneByte(5);

	// SPI Basic driver is in IRQ-mode, enable global interrupts.
	//ENABLE_INTERRUPTS();

	// Test driver, assume that the SPI MISO and MOSI pins have been looped back

	//drive_slave_select_low_custom();
	//PORTA_set_pin_level(7,false);
	//writeOneByte(6);
	//*bufferToSend = 0;
	uint8_t data = 0;
	drive_slave_select_high_custom();
	//TIMER_0_enable();
	return SPI_ADC_exchange_byte(data);
	//return bufferToSend; 
	 // Wait for the transfer to complete

	// Check that the correct data was receive
}
void serialWrite(uint8_t* data,uint8_t length)
{
	for(uint8_t i = 0;i<length;i++)
	{
		writeOneByte(data[i]);
	}
}

void serialPrint(uint8_t* data)
{
	uint8_t idx = 0;
	while(data[idx]!=0)
	{
		writeOneByte(data[idx]);
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
		USART_RADIO_write(*pData);
		pData++;
		//while(!USART1_IsTxReady());
	}
}
void headerHandler(uint8_t utilityId)
{
	writeOneByte(15);
	//WriteOneByte(INDEX);
	//WriteOneByte(TYPE);
	writeOneByte(utilityId);
}
SHT31Class SHT31;
int main(void)
{
	/* Initializes MCU, drivers and middleware */
	atmel_start_init();
	PWM_LED_enable();
	USART_RADIO_enable();
	initCB();
	USART_RADIO_set_ISR_cb(&newCbTx,UDRE_CB);
	USART_RADIO_set_ISR_cb(&newCbRx,RX_CB);
	//writeOneByte(0xB);
	I2C_Module_init();
	TIMER_0_init();
	TIMER_0_disable();
	SPI_ADC_enable();
	PORTA_set_pin_dir(7,PORT_DIR_OUT);// for spi
	PORTA_set_pin_level(7,true);
	SPI_ADC_register_callback(drive_slave_select_high_custom);
	uint8_t stat =  SHT31.begin(0x44);
	//writeOneByte(stat);
	stat = SHT31.readStatus();
	/* Replace with your application code */
	PORTF_set_pin_dir(5,PORT_DIR_OUT);
	PWM_LED_load_duty_cycle_ch0(0x0);
	//TIMER_0_enable();
	int32_t *hum,*temp;
	uint8_t aux;
	//float test = SHT31.readHumidity();
	//aux = test;
	//writeOneByte(aux);
	//SHT31.readBoth(temp,hum);
	//aux = (uint8_t*)temp;
	while (1) {
		PORTF_set_pin_level(5,true);
		//writeOneByte(0x15);
		//writeOneByte(SPI_ADC_test_spi());
		
		//writeOneByte(aux[3]);
		//writeOneByte(aux[4]);
		//serialPrint(aux);
		_delay_ms(1000);
		PORTF_set_pin_level(5,false);
		_delay_ms(1000);
	}
}


