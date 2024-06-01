#include <atmel_start.h>
#include <util/delay.h>
#include "SpecialLibraries/SHT31.h"


 void WriteOneByte(uint8_t dataByte)
 {
	 USART1_Write(dataByte);
	 //while(!USART1_IsTxReady());
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

int main(void)
{
	/* Initializes MCU, drivers and middleware */
	atmel_start_init();
	
	PORTA_set_pin_dir(4,PORT_DIR_OUT);
	USART_1_init();//in principiu asta se face deja in functia atmel_start_init()
	USART_1_enable();//in principiu asta se face deja in functia atmel_start_init()
	USART_1_enable_tx();//in principiu asta se face deja in functia atmel_start_init()
	
	
	//as incepe prin a face debug la primele comenzi pe care le trimiti la SHT31. De exemplu as returna diverse numere din functia de begin din fisierul SHT31 si apoi as trimite numerele pe HC12 pt debug
	//sa folosesti  serialPrintNumber de mai sus pentru asta si astfel poti sa faci debug...
	
	/* Replace with your application code */
	while (1) {
		USART_1_write('a');
		PORTA_set_pin_level(4,1);
		_delay_ms(500);
		USART_1_write(testFunct('b'));
		PORTA_set_pin_level(4,0);
		_delay_ms(500);
	}
}
