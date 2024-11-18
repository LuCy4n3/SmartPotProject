/**
 * \file
 *
 * \brief USART basic driver.
 *
 (c) 2020 Microchip Technology Inc. and its subsidiaries.

    Subject to your compliance with these terms,you may use this software and
    any derivatives exclusively with Microchip products.It is your responsibility
    to comply with third party license terms applicable to your use of third party
    software (including open source software) that may accompany Microchip software.

    THIS SOFTWARE IS SUPPLIED BY MICROCHIP "AS IS". NO WARRANTIES, WHETHER
    EXPRESS, IMPLIED OR STATUTORY, APPLY TO THIS SOFTWARE, INCLUDING ANY IMPLIED
    WARRANTIES OF NON-INFRINGEMENT, MERCHANTABILITY, AND FITNESS FOR A
    PARTICULAR PURPOSE.

    IN NO EVENT WILL MICROCHIP BE LIABLE FOR ANY INDIRECT, SPECIAL, PUNITIVE,
    INCIDENTAL OR CONSEQUENTIAL LOSS, DAMAGE, COST OR EXPENSE OF ANY KIND
    WHATSOEVER RELATED TO THE SOFTWARE, HOWEVER CAUSED, EVEN IF MICROCHIP HAS
    BEEN ADVISED OF THE POSSIBILITY OR THE DAMAGES ARE FORESEEABLE. TO THE
    FULLEST EXTENT ALLOWED BY LAW, MICROCHIP'S TOTAL LIABILITY ON ALL CLAIMS IN
    ANY WAY RELATED TO THIS SOFTWARE WILL NOT EXCEED THE AMOUNT OF FEES, IF ANY,
    THAT YOU HAVE PAID DIRECTLY TO MICROCHIP FOR THIS SOFTWARE.
 *
 */

/**
 * \defgroup doc_driver_usart_basic USART Basic
 * \ingroup doc_driver_usart
 *
 * \section doc_driver_usart_basic_rev Revision History
 * - v0.0.0.1 Initial Commit
 *
 *@{
 */
#include <compiler.h>
#include <clock_config.h>
#include <usart_basic.h>
#include <atomic.h>

/* Static Variables holding the ringbuffer used in IRQ mode */
static uint8_t          HC_12_USART_2_rxbuf[HC_12_USART_2_RX_BUFFER_SIZE];
static volatile uint8_t HC_12_USART_2_rx_head;
static volatile uint8_t HC_12_USART_2_rx_tail;
static volatile uint8_t HC_12_USART_2_rx_elements;
static uint8_t          HC_12_USART_2_txbuf[HC_12_USART_2_TX_BUFFER_SIZE];
static volatile uint8_t HC_12_USART_2_tx_head;
static volatile uint8_t HC_12_USART_2_tx_tail;
static volatile uint8_t HC_12_USART_2_tx_elements;

void HC_12_USART_2_default_rx_isr_cb(void);
void (*HC_12_USART_2_rx_isr_cb)(void) = &HC_12_USART_2_default_rx_isr_cb;
void HC_12_USART_2_default_udre_isr_cb(void);
void (*HC_12_USART_2_udre_isr_cb)(void) = &HC_12_USART_2_default_udre_isr_cb;

void HC_12_USART_2_default_rx_isr_cb(void)
{
	uint8_t data;
	uint8_t tmphead;

	/* Read the received data */
	data = USART2.RXDATAL;
	/* Calculate buffer index */
	tmphead = (HC_12_USART_2_rx_head + 1) & HC_12_USART_2_RX_BUFFER_MASK;

	if (tmphead == HC_12_USART_2_rx_tail) {
		/* ERROR! Receive buffer overflow */
	} else {
		/* Store new index */
		HC_12_USART_2_rx_head = tmphead;

		/* Store received data in buffer */
		HC_12_USART_2_rxbuf[tmphead] = data;
		HC_12_USART_2_rx_elements++;
	}
}

void HC_12_USART_2_default_udre_isr_cb(void)
{
	uint8_t tmptail;

	/* Check if all data is transmitted */
	if (HC_12_USART_2_tx_elements != 0) {
		/* Calculate buffer index */
		tmptail = (HC_12_USART_2_tx_tail + 1) & HC_12_USART_2_TX_BUFFER_MASK;
		/* Store new index */
		HC_12_USART_2_tx_tail = tmptail;
		/* Start transmission */
		USART2.TXDATAL = HC_12_USART_2_txbuf[tmptail];
		HC_12_USART_2_tx_elements--;
	}

	if (HC_12_USART_2_tx_elements == 0) {
		/* Disable UDRE interrupt */
		USART2.CTRLA &= ~(1 << USART_DREIE_bp);
	}
}

/**
 * \brief Set call back function for HC_12_USART_2
 *
 * \param[in] cb The call back function to set
 *
 * \param[in] type The type of ISR to be set
 *
 * \return Nothing
 */
void HC_12_USART_2_set_ISR_cb(usart_cb_t cb, usart_cb_type_t type)
{
	switch (type) {
	case RX_CB:
		HC_12_USART_2_rx_isr_cb = cb;
		break;
	case UDRE_CB:
		HC_12_USART_2_udre_isr_cb = cb;
		break;
	default:
		// do nothing
		break;
	}
}

/* Interrupt service routine for RX complete */
ISR(USART2_RXC_vect)
{
	if (HC_12_USART_2_rx_isr_cb != NULL)
		(*HC_12_USART_2_rx_isr_cb)();
}

/* Interrupt service routine for Data Register Empty */
ISR(USART2_DRE_vect)
{
	if (HC_12_USART_2_udre_isr_cb != NULL)
		(*HC_12_USART_2_udre_isr_cb)();
}

bool HC_12_USART_2_is_tx_ready()
{
	return (HC_12_USART_2_tx_elements != HC_12_USART_2_TX_BUFFER_SIZE);
}

bool HC_12_USART_2_is_rx_ready()
{
	return (HC_12_USART_2_rx_elements != 0);
}

bool HC_12_USART_2_is_tx_busy()
{
	return (!(USART2.STATUS & USART_TXCIF_bm));
}

/**
 * \brief Read one character from HC_12_USART_2
 *
 * Function will block if a character is not available.
 *
 * \return Data read from the HC_12_USART_2 module
 */
uint8_t HC_12_USART_2_read(void)
{
	uint8_t tmptail;

	/* Wait for incoming data */
	while (HC_12_USART_2_rx_elements == 0)
		;
	/* Calculate buffer index */
	tmptail = (HC_12_USART_2_rx_tail + 1) & HC_12_USART_2_RX_BUFFER_MASK;
	/* Store new index */
	HC_12_USART_2_rx_tail = tmptail;
	ENTER_CRITICAL(R);
	HC_12_USART_2_rx_elements--;
	EXIT_CRITICAL(R);

	/* Return data */
	return HC_12_USART_2_rxbuf[tmptail];
}

/**
 * \brief Write one character to HC_12_USART_2
 *
 * Function will block until a character can be accepted.
 *
 * \param[in] data The character to write to the USART
 *
 * \return Nothing
 */
void HC_12_USART_2_write(const uint8_t data)
{
	uint8_t tmphead;

	/* Calculate buffer index */
	tmphead = (HC_12_USART_2_tx_head + 1) & HC_12_USART_2_TX_BUFFER_MASK;
	/* Wait for free space in buffer */
	while (HC_12_USART_2_tx_elements == HC_12_USART_2_TX_BUFFER_SIZE)
		;
	/* Store data in buffer */
	HC_12_USART_2_txbuf[tmphead] = data;
	/* Store new index */
	HC_12_USART_2_tx_head = tmphead;
	ENTER_CRITICAL(W);
	HC_12_USART_2_tx_elements++;
	EXIT_CRITICAL(W);
	/* Enable UDRE interrupt */
	USART2.CTRLA |= (1 << USART_DREIE_bp);
}

/**
 * \brief Initialize USART interface
 * If module is configured to disabled state, the clock to the USART is disabled
 * if this is supported by the device's clock system.
 *
 * \return Initialization status.
 * \retval 0 the USART init was successful
 * \retval 1 the USART init was not successful
 */
int8_t HC_12_USART_2_init()
{

	USART2.BAUD = (uint16_t)USART2_BAUD_RATE(2400); /* set baud rate register */

	USART2.CTRLA = 0 << USART_ABEIE_bp      /* Auto-baud Error Interrupt Enable: disabled */
	               | 0 << USART_DREIE_bp    /* Data Register Empty Interrupt Enable: disabled */
	               | 0 << USART_LBME_bp     /* Loop-back Mode Enable: disabled */
	               | USART_RS485_DISABLE_gc /* RS485 Mode disabled */
	               | 1 << USART_RXCIE_bp    /* Receive Complete Interrupt Enable: enabled */
	               | 0 << USART_RXSIE_bp    /* Receiver Start Frame Interrupt Enable: disabled */
	               | 0 << USART_TXCIE_bp;   /* Transmit Complete Interrupt Enable: disabled */

	USART2.CTRLB = 0 << USART_MPCM_bp       /* Multi-processor Communication Mode: disabled */
	               | 0 << USART_ODME_bp     /* Open Drain Mode Enable: disabled */
	               | 1 << USART_RXEN_bp     /* Receiver Enable: enabled */
	               | USART_RXMODE_NORMAL_gc /* Normal mode */
	               | 0 << USART_SFDEN_bp    /* Start Frame Detection Enable: disabled */
	               | 1 << USART_TXEN_bp;    /* Transmitter Enable: enabled */

	// USART2.CTRLC = USART_CMODE_ASYNCHRONOUS_gc /* Asynchronous Mode */
	//		 | USART_CHSIZE_8BIT_gc /* Character size: 8 bit */
	//		 | USART_PMODE_DISABLED_gc /* No Parity */
	//		 | USART_SBMODE_1BIT_gc; /* 1 stop bit */

	// USART2.DBGCTRL = 0 << USART_DBGRUN_bp; /* Debug Run: disabled */

	// USART2.EVCTRL = 0 << USART_IREI_bp; /* IrDA Event Input Enable: disabled */

	// USART2.RXPLCTRL = 0x0 << USART_RXPL_gp; /* Receiver Pulse Length: 0x0 */

	// USART2.TXPLCTRL = 0x0 << USART_TXPL_gp; /* Transmit pulse length: 0x0 */

	uint8_t x;

	/* Initialize ringbuffers */
	x = 0;

	HC_12_USART_2_rx_tail     = x;
	HC_12_USART_2_rx_head     = x;
	HC_12_USART_2_rx_elements = x;
	HC_12_USART_2_tx_tail     = x;
	HC_12_USART_2_tx_head     = x;
	HC_12_USART_2_tx_elements = x;

	return 0;
}

/**
 * \brief Enable RX and TX in HC_12_USART_2
 * 1. If supported by the clock system, enables the clock to the USART
 * 2. Enables the USART module by setting the RX and TX enable-bits in the USART control register
 *
 * \return Nothing
 */
void HC_12_USART_2_enable()
{
	USART2.CTRLB |= USART_RXEN_bm | USART_TXEN_bm;
}

/**
 * \brief Enable RX in HC_12_USART_2
 * 1. If supported by the clock system, enables the clock to the USART
 * 2. Enables the USART module by setting the RX enable-bit in the USART control register
 *
 * \return Nothing
 */
void HC_12_USART_2_enable_rx()
{
	USART2.CTRLB |= USART_RXEN_bm;
}

/**
 * \brief Enable TX in HC_12_USART_2
 * 1. If supported by the clock system, enables the clock to the USART
 * 2. Enables the USART module by setting the TX enable-bit in the USART control register
 *
 * \return Nothing
 */
void HC_12_USART_2_enable_tx()
{
	USART2.CTRLB |= USART_TXEN_bm;
}

/**
 * \brief Disable HC_12_USART_2
 * 1. Disables the USART module by clearing the enable-bit(s) in the USART control register
 * 2. If supported by the clock system, disables the clock to the USART
 *
 * \return Nothing
 */
void HC_12_USART_2_disable()
{
	USART2.CTRLB &= ~(USART_RXEN_bm | USART_TXEN_bm);
}

/**
 * \brief Get recieved data from HC_12_USART_2
 *
 * \return Data register from HC_12_USART_2 module
 */
uint8_t HC_12_USART_2_get_data()
{
	return USART2.RXDATAL;
}
