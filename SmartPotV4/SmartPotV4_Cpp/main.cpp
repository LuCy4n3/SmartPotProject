/*
 * SmartPotV4_Cpp.cpp
 *
 * Created: 11/17/2024 11:22:39 PM
 * Author : simon
 */ 

#include <avr/io.h>
#include <util/delay.h>
#include <atmel_start.h>

int main(void)
{
	/* Initializes MCU, drivers and middleware */
	atmel_start_init();
	
	PORTF_set_pin_dir(5,PORT_DIR_OUT);
	PORTF_set_pin_level(5,true);
	PORTA_set_pin_dir(0,PORT_DIR_OUT);
	PORTA_set_pin_dir(1,PORT_DIR_OUT);
	
	PORTA_set_pin_level(0,false);
	PORTA_set_pin_level(1,true);
	/* Replace with your application code */
	while (1) {
		PORTF_set_pin_level(5,true);
		_delay_ms(1500);
		PORTF_set_pin_level(5,false);
		_delay_ms(1500);
	}
}

