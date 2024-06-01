#include <atmel_start.h>
#include <avr/io.h>
#include <util/delay.h>

int main(void)
{
    /* Replace with your application code */
	atmel_start_init();

	PORTA_set_pin_dir(4,PORT_DIR_OUT);

    while (1) 
    {
		PORTA_set_pin_level(4,1);
		_delay_ms(500);
		PORTA_set_pin_level(4,1);
		_delay_ms(500);
    }
}

