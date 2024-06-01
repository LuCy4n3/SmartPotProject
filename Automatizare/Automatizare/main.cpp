#include <avr/io.h>
#include "mcc_generated_files/system/system.h"
#include "mcc_generated_files/timer/delay.h"

int main(void)
{
    /* Replace with your application code */
	
	SYSTEM_Initialize();
	USART1_Initialize();
	
	PORTA_set_pin_dir(4,PORT_DIR_OUT);
	PORTC_set_pin_dir(2,PORT_DIR_OUT);
	USART1_Enable();
	USART1_TransmitInterruptEnable();
	USART1_ReceiveInterruptEnable();
	
	
	PORTA_set_pin_level(4,true);
	PORTC_set_pin_level(2,true);
	
	PORTC_set_pin_level(2,true);
	DELAY_milliseconds(500);
	PORTC_set_pin_level(2,false);
	DELAY_milliseconds(500);
	
	//usart1TxBuffer[0]='a';
	
    while (1) 
    {
		PORTA_set_pin_level(4,true);
		USART1_TransmitISR();
		if(USART1_Read() == 'a')
				PORTC_set_pin_level(2,true);
		else if(USART1_Read() == 'b')
				PORTC_set_pin_level(2,false);
		DELAY_milliseconds(250);
		PORTA_set_pin_level(4,false);
		DELAY_milliseconds(250);
    }
}
}

