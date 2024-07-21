#include <atmel_start.h>
#include <util/delay.h>

void SerialWrite(uint8_t* data)
{
	uint8_t* pData = data;
	while(*pData!=0)
	{
		USART_0_write(*pData);
		pData++;
		//while(!USART1_IsTxReady());
	}
}
int main(void)
{
	/* Initializes MCU, drivers and middleware */
	atmel_start_init();
	ENABLE_INTERRUPTS();
	PORTA_set_pin_dir(4,PORT_DIR_OUT);
	/* Replace with your application code */
	while (1) {
		SerialWrite("Started 1");
		//USART_0_write('a');
		PORTA_set_pin_level(4,true);
		_delay_ms(1000);
		PORTA_set_pin_level(4,false);
		_delay_ms(1000);
	}
}
