/*
 * CallbackFun.c
 *
 * Created: 12/20/2024 4:45:31 AM
 *  Author: simon
 */ 
#include "../CallbackFun.h"
#include <compiler.h>
#include <clock_config.h>
#include <usart_basic.h>
#include <atomic.h>

static uint8_t          USART_RADIO_rxbuf[USART_RADIO_RX_BUFFER_SIZE];
static volatile uint8_t USART_RADIO_rx_head;
static volatile uint8_t USART_RADIO_rx_tail;
static volatile uint8_t USART_RADIO_rx_elements;
static uint8_t          USART_RADIO_txbuf[USART_RADIO_TX_BUFFER_SIZE];
static volatile uint8_t USART_RADIO_tx_head;
static volatile uint8_t USART_RADIO_tx_tail;
static volatile uint8_t USART_RADIO_tx_elements;

void initCB(void)
{
	uint8_t x = 0;

	USART_RADIO_rx_tail     = x;
	USART_RADIO_rx_head     = x;
	USART_RADIO_rx_elements = x;
	USART_RADIO_tx_tail     = x;
	USART_RADIO_tx_head     = x;
	USART_RADIO_tx_elements = x;
}
void newCbTx(void)
{
	uint8_t tmptail;

	/* Check if all data is transmitted */
	if (USART_RADIO_tx_elements != 0) {
		/* Calculate buffer index */
		tmptail = (USART_RADIO_tx_tail + 1) & USART_RADIO_TX_BUFFER_MASK;
		/* Store new index */
		USART_RADIO_tx_tail = tmptail;
		/* Start transmission */
		USART2.TXDATAL = USART_RADIO_txbuf[tmptail];
		USART_RADIO_tx_elements--;
	}

	if (USART_RADIO_tx_elements == 0) {
		/* Disable UDRE interrupt */
		USART2.CTRLA &= ~(1 << USART_DREIE_bp);
	}
}
void newCbRx(void)
{
	{
		uint8_t data;
		uint8_t tmphead;

		/* Read the received data */
		data = USART2.RXDATAL;
		/* Calculate buffer index */
		tmphead = (USART_RADIO_rx_head + 1) & USART_RADIO_RX_BUFFER_MASK;

		 
		/* Store new index */
		USART_RADIO_rx_head = tmphead;

		/* Store received data in buffer */
		USART_RADIO_rxbuf[tmphead] = data;
		writeOneByte(data);
		writeOneByte(data);
		PWM_LED_load_duty_cycle_ch0(0);
		USART_RADIO_rx_elements++;
		
	}
}
void writeOneByte(const uint8_t data)
{
	uint8_t tmphead;

	/* Calculate buffer index */
	tmphead = (USART_RADIO_tx_head + 1) & USART_RADIO_TX_BUFFER_MASK;
	/* Wait for free space in buffer */
	while (USART_RADIO_tx_elements == USART_RADIO_TX_BUFFER_SIZE)
	;
	/* Store data in buffer */
	USART_RADIO_txbuf[tmphead] = data;
	/* Store new index */
	USART_RADIO_tx_head = tmphead;
	ENTER_CRITICAL(W);
	USART_RADIO_tx_elements++;
	EXIT_CRITICAL(W);
	/* Enable UDRE interrupt */
	USART2.CTRLA |= (1 << USART_DREIE_bp);
	//PWM_LED_load_duty_cycle_ch0(0x0);
}
void drive_slave_select_high_custom(void)
{
	PORTA_set_pin_level(7,true);
}
void drive_slave_select_low_custom(void)
{
	PORTA_set_pin_level(7,false);
}
