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
#define INDEX 4
#define TYPE 2

static uint8_t *bufferToSend;
static uint8_t *bufferToRead;

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

void enableSPI_soft()
{
	PORTA_set_pin_dir(4,PORT_DIR_OUT);	// MOSI
	PORTA_set_pin_dir(5,PORT_DIR_IN);	// MISO
	PORTA_set_pin_dir(6,PORT_DIR_OUT);	// SCK
	PORTA_set_pin_dir(7,PORT_DIR_OUT);	// !SS
	
	PORTA_set_pin_level(4,false);		// set the MOSI low
	PORTA_set_pin_level(5,false);		// set the MISO low
	PORTA_set_pin_level(6,false);		// set the starting position of the clock to low
	PORTA_set_pin_level(7,true);		// set the SS high
	
}
bool converToBool(uint8_t number) {
	return number >= 1 ? true : false;
}
void newClock(uint8_t pin)
{
	PORTA_set_pin_level(6,converToBool(!PORTA_get_pin_level(6)));
	_delay_ms(100);
	PORTA_set_pin_level(6,converToBool(!PORTA_get_pin_level(6)));
	_delay_ms(100);
}
uint8_t readSpiADC(uint8_t channel)
{
	uint8_t readData = 0;
	bool setState;
	//channel = channel << 1; // make sure the actual data that can be between 0 and 3, is placed in the bits that are of interest
	//write
	PORTA_set_pin_level(6,false);
	PORTA_set_pin_level(7,false);
	PORTA_set_pin_level(4,false);
	//_delay_ms(100);
	//PORTA_set_pin_level(6,true);
	//_delay_ms(100);
	//PORTA_set_pin_level(6,false);
	//_delay_ms(100);
	//PORTA_set_pin_level(6,true);
	//_delay_ms(100);
	newClock(6);
	//newClock(6);
	//writeOneByte(0xA);
	for(uint8_t i = 7; i > 0; i--)
	{
		
		//PORTA_toggle_pin_level(6);
		PORTA_set_pin_level(4,converToBool(channel&(1<<i)));
		_delay_ms(20);
		newClock(6);
		//PORTA_toggle_pin_level(4);
		
		//setState = i<=2 ? true : false;
		//PORTA_set_pin_level(4,setState);
		//writeOneByte(converToBool(channel&(1<<aux)) == true? 1 : 0);
		readData = (readData<<1) | PORTA_get_pin_level(5);
		//writeOneByte(readData);
		//writeOneByte(PORTA_get_pin_level(5));
		//_delay_ms(100);
	}
	//_delay_ms(100);
	PORTA_set_pin_level(4,false);
	
	//writeOneByte(0xC);
	
	//readData = PORTA_get_pin_level(5);
	//read
	//_delay_ms(200);
	for(uint8_t i = 0; i < 4; i++)
	{
		//PORTA_toggle_pin_level(6);
		newClock(6);
		readData = (readData<<1) | PORTA_get_pin_level(5);
		//writeOneByte(PORTA_get_pin_level(5));
		//writeOneByte(readData);
		//_delay_ms(100);
	}
	//writeOneByte(0xA);
	PORTA_set_pin_level(7,true);
	return readData;
	
}
void headerHandler(uint8_t utilityId)
{
	writeOneByte(15);
	writeOneByte(INDEX);
	writeOneByte(TYPE);
	writeOneByte(utilityId);
}
void sendTemp(int32_t readTemp,uint8_t utilityId)
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

SHT31Class SHT31;

int main(void)
{
	/* Initializes MCU, drivers and middleware */
	atmel_start_init();
	PWM_LED_init();
	PWM_LED_enable();
	PWM_0_init();
	PWM_0_enable();
	PWM_0_load_top(0xff);
	PWM_0_enable_output_ch0();
	USART_RADIO_enable();
	initCB();
	I2C_Module_init();
	
	_delay_us(50);
	int32_t temp,hum;
	uint16_t errorCode;
	
	//serialWrite((uint8_t*)temp,4);
	USART_RADIO_set_ISR_cb(&newCbTx,UDRE_CB);
	USART_RADIO_set_ISR_cb(&newCbRx,RX_CB);
	//errorCode = SHT31.readBoth(temp,hum);
	writeOneByte(0xA);
	enableSPI_soft();
	/* Replace with your application code */
	PORTF_set_pin_dir(5,PORT_DIR_OUT);
	PORTF_set_pin_dir(3,PORT_DIR_OUT);
	PORTA_set_pin_dir(1,PORT_DIR_OUT);
	PORTA_set_pin_level(1,false);
	PWM_LED_load_duty_cycle_ch0(0x0);
	PWM_0_load_duty_cycle_ch0(0x0);
	//PWM_0_load_duty_cycle_ch0(0xf);
	errorCode = SHT31.begin(0x44);	
	//TIMER_0_enable();
	while (1) {
		PORTF_set_pin_level(5,true);
		errorCode = SHT31.begin(0x44);
		//writeOneByte(errorCode<<8);
		//writeOneByte(errorCode);
		errorCode = SHT31.readBoth(&temp,&hum);
		sendTemp(temp,1);
		_delay_ms(1000);
		sendHum(hum,2);
		//PORTF_set_pin_level(3,true);
		//writeOneByte(0xB);
		//readSpiADC(0);
		_delay_ms(1000);
		PORTF_set_pin_level(5,false);
		//PORTF_set_pin_level(3,false);
		_delay_ms(1000);
		//writeOneByte(readSpiADC(0xFF));
	}
}


