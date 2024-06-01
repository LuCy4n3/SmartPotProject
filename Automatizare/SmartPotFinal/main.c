#include <atmel_start.h>
#include <util/delay.h>
#include "SpecialLibraries/SHT31.h"

int main(void)
{
	/* Initializes MCU, drivers and middleware */
	atmel_start_init();
	
	PORTA_set_pin_dir(4,PORT_DIR_OUT);
	USART_1_init();
	USART_1_enable();
	USART_1_enable_tx();
	
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
