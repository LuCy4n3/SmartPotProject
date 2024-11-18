#include <atmel_start.h>

int main(void)
{
	/* Initializes MCU, drivers and middleware */
	atmel_start_init();
	
	PORTF_set_pin_dir(5,PORT_DIR_OUT);
	PORTF_set_pin_level(5,true);
	
	/* Replace with your application code */
	while (1) {
	}
}
