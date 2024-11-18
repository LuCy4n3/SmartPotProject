/**
 * \file
 *
 * \brief I2C master driver.
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
 * \defgroup doc_driver_i2c_master I2C Master Driver
 * \ingroup doc_driver_i2c
 *
 * \section doc_driver_i2c_master_rev Revision History
 * - v0.0.0.1 Initial Commit
 *
 *@{
 */

#include <i2c_master.h>
#include <i2c_types.h>
#include <driver_init.h>
#include <util/delay.h>
#include <stdbool.h>
#include <stdlib.h>
// #include "timeout.h"  // TODO: Add timeout integration

/***************************************************************************/
// I2C STATES
typedef enum {
	I2C_IDLE = 0,
	I2C_SEND_ADR_READ,
	I2C_SEND_ADR_WRITE,
	I2C_TX,
	I2C_RX,
	I2C_TX_EMPTY,
	I2C_SEND_RESTART_READ,
	I2C_SEND_RESTART_WRITE,
	I2C_SEND_RESTART,
	I2C_SEND_STOP,
	I2C_RX_DO_ACK,
	I2C_TX_DO_ACK,
	I2C_RX_DO_NACK_STOP,
	I2C_RX_DO_NACK_RESTART,
	I2C_RESET,
	I2C_ADDRESS_NACK,
	I2C_BUS_COLLISION,
	I2C_BUS_ERROR
} i2c_fsm_states_t;

// I2C Event Callback List
typedef enum {
	i2c_dataComplete = 0,
	i2c_writeCollision,
	i2c_addressNACK,
	i2c_dataNACK,
	i2c_timeOut,
	i2c_NULL
} i2c_callback_index;

// I2C Status Structure
typedef struct {
	unsigned         busy : 1;
	unsigned         inUse : 1;
	unsigned         bufferFree : 1;
	unsigned         addressNACKCheck : 1;
	i2c_address_t    address;       /// The I2C Address
	uint8_t *        data_ptr;      /// pointer to a data buffer
	size_t           data_length;   /// Bytes in the data buffer
	uint16_t         timeout;       /// I2C Timeout Counter between I2C Events.
	uint16_t         timeout_value; /// Reload value for the timeouts
	i2c_fsm_states_t state;         /// Driver State
	i2c_error_t      error;
	/*if timeoutDriverEnabled
	timerStruct_t timeout;
	*/
	i2c_callback callbackTable[6];
	void *       callbackPayload[6]; ///  each callback can have a payload
} i2c_status_t;

typedef i2c_fsm_states_t(stateHandlerFunction)(void);

i2c_status_t SHT31_MOD_I2C_0_status = {0};

static void             SHT31_MOD_I2C_0_set_callback(i2c_callback_index idx, i2c_callback cb, void *p);
static i2c_operations_t SHT31_MOD_I2C_0_return_stop(void *p);
static i2c_operations_t SHT31_MOD_I2C_0_return_reset(void *p);
static i2c_fsm_states_t SHT31_MOD_I2C_0_do_I2C_SEND_ADR_READ(void);
static i2c_fsm_states_t SHT31_MOD_I2C_0_do_I2C_SEND_ADR_WRITE(void);
static void             SHT31_MOD_I2C_0_master_isr(void);

/*if timeoutDriverEnabled>
ABSOLUTETIME_t ${i2cMasterFunctions["timeoutHandler"]}(void *p);

// place this function someplace in a periodic interrupt
ABSOLUTETIME_t ${i2cMasterFunctions["timeoutHandler"]}(void *p)
{
    ${msspI2cFunctions["disableIRQ"]}();
    ${i2cMasterFunctions["status"]}.state = I2C_RESET; // Jump to the Timeout state
    ${msspI2cFunctions["enableIRQ"]}();
    ${msspI2cFunctions["setIRQ"]}(); // force an interrupt to handle the timeout
    return 0;
}
*/
/**
 * \brief Set callback to be called when all specifed data has been transferred.
 *
 * \param[in] cb Pointer to callback function
 * \param[in] p  Pointer to the callback function's parameters
 *
 * \return Nothing
 */
void SHT31_MOD_I2C_0_set_data_complete_callback(i2c_callback cb, void *p)
{
	SHT31_MOD_I2C_0_set_callback(i2c_dataComplete, cb, p);
}

/**
 * \brief Set callback to be called when there has been a bus collision and arbitration was lost.
 *
 * \param[in] cb Pointer to callback function
 * \param[in] p  Pointer to the callback function's parameters
 *
 * \return Nothing
 */
void SHT31_MOD_I2C_0_set_write_collision_callback(i2c_callback cb, void *p)
{
	SHT31_MOD_I2C_0_set_callback(i2c_writeCollision, cb, p);
}

/**
 * \brief Set callback to be called when the transmitted address was NACK'ed.
 *
 * \param[in] cb Pointer to callback function
 * \param[in] p  Pointer to the callback function's parameters
 *
 * \return Nothing
 */
void SHT31_MOD_I2C_0_set_address_nack_callback(i2c_callback cb, void *p)
{
	SHT31_MOD_I2C_0_set_callback(i2c_addressNACK, cb, p);
}

/**
 * \brief Set callback to be called when the transmitted data was NACK'ed.
 *
 * \param[in] cb Pointer to callback function
 * \param[in] p  Pointer to the callback function's parameters
 *
 * \return Nothing
 */
void SHT31_MOD_I2C_0_set_data_nack_callback(i2c_callback cb, void *p)
{
	SHT31_MOD_I2C_0_set_callback(i2c_dataNACK, cb, p);
}

/**
 * \brief Set callback to be called when there was a bus timeout.
 *
 * \param[in] cb Pointer to callback function
 * \param[in] p  Pointer to the callback function's parameters
 *
 * \return Nothing
 */
void SHT31_MOD_I2C_0_set_timeout_callback(i2c_callback cb, void *p)
{
	SHT31_MOD_I2C_0_set_callback(i2c_timeOut, cb, p);
}

/**
 * \brief Initialize I2C interface
 * If module is configured to disabled state, the clock to the I2C is disabled
 * if this is supported by the device's clock system.
 *
 * \return Initialization status.
 * \retval 0 the init was successful
 * \retval 1 the init was not successful
 */
void SHT31_MOD_I2C_0_init()
{

	// TWI0.CTRLA = 0 << TWI_FMPEN_bp /* FM Plus Enable: disabled */
	//		 | TWI_SDAHOLD_OFF_gc /* SDA hold time off */
	//		 | TWI_SDASETUP_4CYC_gc; /* SDA setup time is 4 clock cycles */

	// TWI0.DBGCTRL = 0 << TWI_DBGRUN_bp; /* Debug Run: disabled */

	TWI0.MBAUD = (uint8_t)TWI0_BAUD(100000, 0); /* set MBAUD register */

	TWI0.MCTRLA = 1 << TWI_ENABLE_bp        /* Enable TWI Master: enabled */
	              | 0 << TWI_QCEN_bp        /* Quick Command Enable: disabled */
	              | 1 << TWI_RIEN_bp        /* Read Interrupt Enable: enabled */
	              | 0 << TWI_SMEN_bp        /* Smart Mode Enable: disabled */
	              | TWI_TIMEOUT_DISABLED_gc /* Bus Timeout Disabled */
	              | 1 << TWI_WIEN_bp;       /* Write Interrupt Enable: enabled */
}

/**
 * \brief Open the I2C for communication
 *
 * \param[in] address The slave address to use in the transfer
 *
 * \return Initialization status.
 * \retval I2C_NOERR The I2C open was successful
 * \retval I2C_BUSY  The I2C open failed because the interface is busy
 * \retval I2C_FAIL  The I2C open failed with an error
 */
i2c_error_t SHT31_MOD_I2C_0_open(i2c_address_t address)
{
	i2c_error_t ret = I2C_BUSY;

	if (!SHT31_MOD_I2C_0_status.inUse) {
		SHT31_MOD_I2C_0_status.address          = address;
		SHT31_MOD_I2C_0_status.busy             = 0;
		SHT31_MOD_I2C_0_status.inUse            = 1;
		SHT31_MOD_I2C_0_status.addressNACKCheck = 0;
		SHT31_MOD_I2C_0_status.state            = I2C_RESET;
		SHT31_MOD_I2C_0_status.timeout_value    = 500; // MCC should determine a reasonable starting value here.
		SHT31_MOD_I2C_0_status.bufferFree       = 1;
		/*
		        <#if timeoutDriverEnabled>
		        SHT31_MOD_I2C_0_status.timeout.callbackPtr = ${i2cMasterFunctions["timeoutHandler"]};
		        </#if>
		*/
		// set all the call backs to a default of sending stop
		SHT31_MOD_I2C_0_status.callbackTable[i2c_dataComplete]     = SHT31_MOD_I2C_0_return_stop;
		SHT31_MOD_I2C_0_status.callbackPayload[i2c_dataComplete]   = NULL;
		SHT31_MOD_I2C_0_status.callbackTable[i2c_writeCollision]   = SHT31_MOD_I2C_0_return_stop;
		SHT31_MOD_I2C_0_status.callbackPayload[i2c_writeCollision] = NULL;
		SHT31_MOD_I2C_0_status.callbackTable[i2c_addressNACK]      = SHT31_MOD_I2C_0_return_stop;
		SHT31_MOD_I2C_0_status.callbackPayload[i2c_addressNACK]    = NULL;
		SHT31_MOD_I2C_0_status.callbackTable[i2c_dataNACK]         = SHT31_MOD_I2C_0_return_stop;
		SHT31_MOD_I2C_0_status.callbackPayload[i2c_dataNACK]       = NULL;
		SHT31_MOD_I2C_0_status.callbackTable[i2c_timeOut]          = SHT31_MOD_I2C_0_return_reset;
		SHT31_MOD_I2C_0_status.callbackPayload[i2c_timeOut]        = NULL;

		TWI0.MCTRLB |= TWI_FLUSH_bm;
		TWI0.MSTATUS |= TWI_BUSSTATE_IDLE_gc;
		// Reset module
		TWI0.MSTATUS |= (TWI_RIF_bm | TWI_WIF_bm);

		// uncomment the IRQ enable for an interrupt driven driver.
		TWI0.MCTRLA |= (TWI_RIEN_bm | TWI_WIEN_bm);

		ret = I2C_NOERR;
	}
	return ret;
}

void SHT31_MOD_I2C_0_set_address(i2c_address_t address)
{
	SHT31_MOD_I2C_0_status.address = address;
}

/**
 * \brief Close the I2C interface
 *
 * \return Status of close operation.
 * \retval I2C_NOERR The I2C close was successful
 * \retval I2C_BUSY  The I2C close failed because the interface is busy
 * \retval I2C_FAIL  The I2C close failed with an error
 */
i2c_error_t SHT31_MOD_I2C_0_close(void)
{
	i2c_error_t ret = I2C_BUSY;
	// Bus is in error state, reset I2C hardware and report error
	if (TWI0.MSTATUS & TWI_BUSERR_bm) {
		SHT31_MOD_I2C_0_status.busy  = false;
		SHT31_MOD_I2C_0_status.error = I2C_FAIL;
	}
	if (!SHT31_MOD_I2C_0_status.busy) {
		SHT31_MOD_I2C_0_status.inUse = 0;
		// close it down
		SHT31_MOD_I2C_0_status.address = 0xff; // 8-bit address is invalid so this is FREE
		TWI0.MSTATUS |= (TWI_RIF_bm | TWI_WIF_bm);
		TWI0.MCTRLA &= ~(TWI_RIEN_bm | TWI_WIEN_bm);
		ret = SHT31_MOD_I2C_0_status.error;
	}
	return ret;
}

/**
 * \brief Set timeout to be used for I2C operations. Uses the Timeout driver.
 *
 * \param[in] to Timeout in ticks
 *
 * \return Nothing
 */
void SHT31_MOD_I2C_0_set_timeout(uint8_t to)
{
	TWI0.MCTRLA &= ~(TWI_RIEN_bm | TWI_WIEN_bm);
	SHT31_MOD_I2C_0_status.timeout_value = to;
	TWI0.MCTRLA |= (TWI_RIEN_bm | TWI_WIEN_bm);
}

/**
 * \brief Set baud rate to be used for I2C operations.
 *
 * \param[in] baud to set the transfer speed
 *
 * \return Nothing
 */
void SHT31_MOD_I2C_0_set_baud_rate(uint32_t baud)
{
	TWI0.MBAUD = (uint8_t)TWI0_BAUD(baud, 0); /* set MBAUD register */
}

/**
 * \brief Sets up the data buffer to use, and number of bytes to transfer
 *
 * \param[in] buffer Pointer to data buffer to use for read or write data
 * \param[in] bufferSize Number of bytes to read or write from slave
 *
 * \return Nothing
 */
void SHT31_MOD_I2C_0_set_buffer(void *buffer, size_t bufferSize)
{
	if (SHT31_MOD_I2C_0_status.bufferFree) {
		SHT31_MOD_I2C_0_status.data_ptr    = buffer;
		SHT31_MOD_I2C_0_status.data_length = bufferSize;
		SHT31_MOD_I2C_0_status.bufferFree  = false;
	}
}

/**
 * \brief Start an operation on an opened I2C interface
 *
 * \param[in] read Set to true for read, false for write
 *
 * \return Status of operation
 * \retval I2C_NOERR The I2C open was successful
 * \retval I2C_BUSY  The I2C open failed because the interface is busy
 * \retval I2C_FAIL  The I2C open failed with an error
 */
i2c_error_t SHT31_MOD_I2C_0_master_operation(bool read)
{
	i2c_error_t ret = I2C_BUSY;
	if (!SHT31_MOD_I2C_0_status.busy) {
		SHT31_MOD_I2C_0_status.busy = true;
		ret                         = I2C_NOERR;

		if (read) {
			SHT31_MOD_I2C_0_status.state = I2C_SEND_ADR_READ;
		} else {
			SHT31_MOD_I2C_0_status.state = I2C_SEND_ADR_WRITE;
		}
		SHT31_MOD_I2C_0_master_isr();
	}
	return ret;
}

/**
 * \brief Identical to SHT31_MOD_I2C_0_master_operation(true);
 */
i2c_error_t SHT31_MOD_I2C_0_master_read(void)
{
	return SHT31_MOD_I2C_0_master_operation(true);
}

/**
 * \brief Identical to SHT31_MOD_I2C_0_master_operation(false);
 */
i2c_error_t SHT31_MOD_I2C_0_master_write(void)
{
	return SHT31_MOD_I2C_0_master_operation(false);
}

/************************************************************************/
/* Helper Functions                                                     */
/************************************************************************/

static i2c_fsm_states_t SHT31_MOD_I2C_0_do_I2C_RESET(void)
{
	TWI0.MCTRLB |= TWI_FLUSH_bm;
	TWI0.MSTATUS |= TWI_BUSSTATE_IDLE_gc;
	SHT31_MOD_I2C_0_status.busy  = false; // Bus Free
	SHT31_MOD_I2C_0_status.error = I2C_NOERR;
	return I2C_RESET; // park the FSM on reset
}

static i2c_fsm_states_t SHT31_MOD_I2C_0_do_I2C_IDLE(void)
{
	SHT31_MOD_I2C_0_status.busy  = false; // Bus Free
	SHT31_MOD_I2C_0_status.error = I2C_NOERR;
	return I2C_IDLE; // park the FSM on IDLE
}

static i2c_fsm_states_t SHT31_MOD_I2C_0_do_I2C_SEND_RESTART_READ(void)
{
	return SHT31_MOD_I2C_0_do_I2C_SEND_ADR_READ();
}

static i2c_fsm_states_t SHT31_MOD_I2C_0_do_I2C_SEND_RESTART_WRITE(void)
{
	return SHT31_MOD_I2C_0_do_I2C_SEND_ADR_WRITE();
}

static i2c_fsm_states_t SHT31_MOD_I2C_0_do_I2C_SEND_RESTART(void)
{
	return SHT31_MOD_I2C_0_do_I2C_SEND_ADR_READ();
}

static i2c_fsm_states_t SHT31_MOD_I2C_0_do_I2C_SEND_STOP(void)
{
	TWI0.MCTRLB |= TWI_MCMD_STOP_gc;
	return SHT31_MOD_I2C_0_do_I2C_IDLE();
}

// TODO: probably need 2 addressNACK's one from read and one from write.
//       the do NACK before RESTART or STOP is a special case that a new state simplifies.
static i2c_fsm_states_t SHT31_MOD_I2C_0_do_I2C_DO_ADDRESS_NACK(void)
{
	SHT31_MOD_I2C_0_status.addressNACKCheck = 0;
	SHT31_MOD_I2C_0_status.error            = I2C_FAIL;
	switch (SHT31_MOD_I2C_0_status.callbackTable[i2c_addressNACK](
	    SHT31_MOD_I2C_0_status.callbackPayload[i2c_addressNACK])) {
	case i2c_restart_read:
		return SHT31_MOD_I2C_0_do_I2C_SEND_RESTART_READ();
	case i2c_restart_write:
		return SHT31_MOD_I2C_0_do_I2C_SEND_RESTART_WRITE();
	default:
		return SHT31_MOD_I2C_0_do_I2C_SEND_STOP();
	}
}

static i2c_fsm_states_t SHT31_MOD_I2C_0_do_I2C_SEND_ADR_READ(void)
{

	SHT31_MOD_I2C_0_status.addressNACKCheck = 1;
	TWI0.MADDR                              = SHT31_MOD_I2C_0_status.address << 1 | 1;
	return I2C_RX;
}

static i2c_fsm_states_t SHT31_MOD_I2C_0_do_I2C_SEND_ADR_WRITE(void)
{

	SHT31_MOD_I2C_0_status.addressNACKCheck = 1;
	TWI0.MADDR                              = SHT31_MOD_I2C_0_status.address << 1;
	return I2C_TX;
}

static i2c_fsm_states_t SHT31_MOD_I2C_0_do_I2C_RX_DO_ACK(void)
{
	TWI0.MCTRLB &= ~(1 << TWI_ACKACT_bp);
	return I2C_RX;
}

static i2c_fsm_states_t SHT31_MOD_I2C_0_do_I2C_TX_DO_ACK(void)
{
	TWI0.MCTRLB &= ~(1 << TWI_ACKACT_bp);
	return I2C_TX;
}

static i2c_fsm_states_t SHT31_MOD_I2C_0_do_I2C_DO_NACK_STOP(void)
{
	TWI0.MCTRLB |= TWI_ACKACT_NACK_gc;
	TWI0.MCTRLB |= TWI_MCMD_STOP_gc;
	return SHT31_MOD_I2C_0_do_I2C_IDLE();
}

static i2c_fsm_states_t SHT31_MOD_I2C_0_do_I2C_DO_NACK_RESTART(void)
{
	TWI0.MCTRLB |= TWI_ACKACT_NACK_gc;
	return I2C_SEND_RESTART;
}

static i2c_fsm_states_t SHT31_MOD_I2C_0_do_I2C_TX(void)
{
	if ((TWI0.MSTATUS & TWI_RXACK_bm)) // Slave replied with NACK
	{
		switch (
		    SHT31_MOD_I2C_0_status.callbackTable[i2c_dataNACK](SHT31_MOD_I2C_0_status.callbackPayload[i2c_dataNACK])) {
		case i2c_restart_read:
			return SHT31_MOD_I2C_0_do_I2C_SEND_RESTART_READ();
		case i2c_restart_write:
			return SHT31_MOD_I2C_0_do_I2C_SEND_RESTART_WRITE();
		default:
		case i2c_continue:
		case i2c_stop:
			return SHT31_MOD_I2C_0_do_I2C_SEND_STOP();
		}
	} else {
		SHT31_MOD_I2C_0_status.addressNACKCheck = 0;
		TWI0.MDATA                              = *SHT31_MOD_I2C_0_status.data_ptr++;
		return (--SHT31_MOD_I2C_0_status.data_length) ? I2C_TX : I2C_TX_EMPTY;
	}
}

static i2c_fsm_states_t SHT31_MOD_I2C_0_do_I2C_RX(void)
{
	SHT31_MOD_I2C_0_status.addressNACKCheck = 0;

	if (SHT31_MOD_I2C_0_status.data_length == 1)
		TWI0.MCTRLB |= TWI_ACKACT_NACK_gc; // Next byte will be last to be received, setup NACK
	else
		TWI0.MCTRLB &= ~(1 << TWI_ACKACT_bp); // More bytes to receive, setup ACK

	if (--SHT31_MOD_I2C_0_status.data_length) {
		*SHT31_MOD_I2C_0_status.data_ptr = TWI0.MDATA;
		SHT31_MOD_I2C_0_status.data_ptr++;
		TWI0.MCTRLB |= TWI_MCMD_RECVTRANS_gc;
		return I2C_RX;
	} else {
		*SHT31_MOD_I2C_0_status.data_ptr = TWI0.MDATA;
		SHT31_MOD_I2C_0_status.data_ptr++;
		SHT31_MOD_I2C_0_status.bufferFree = true;
		switch (SHT31_MOD_I2C_0_status.callbackTable[i2c_dataComplete](
		    SHT31_MOD_I2C_0_status.callbackPayload[i2c_dataComplete])) {
		case i2c_restart_write:
		case i2c_restart_read:
			return SHT31_MOD_I2C_0_do_I2C_DO_NACK_RESTART();
		default:
		case i2c_continue:
		case i2c_stop:
			return SHT31_MOD_I2C_0_do_I2C_DO_NACK_STOP();
		}
	}
}

static i2c_fsm_states_t SHT31_MOD_I2C_0_do_I2C_TX_EMPTY(void)
{
	if ((TWI0.MSTATUS & TWI_RXACK_bm)) // Slave replied with NACK
	{
		switch (
		    SHT31_MOD_I2C_0_status.callbackTable[i2c_dataNACK](SHT31_MOD_I2C_0_status.callbackPayload[i2c_dataNACK])) {
		case i2c_restart_read:
			return SHT31_MOD_I2C_0_do_I2C_SEND_RESTART_READ();
		case i2c_restart_write:
			return SHT31_MOD_I2C_0_do_I2C_SEND_RESTART_WRITE();
		default:
		case i2c_continue:
		case i2c_stop:
			return SHT31_MOD_I2C_0_do_I2C_SEND_STOP();
		}
	} else {
		SHT31_MOD_I2C_0_status.bufferFree = true;
		switch (SHT31_MOD_I2C_0_status.callbackTable[i2c_dataComplete](
		    SHT31_MOD_I2C_0_status.callbackPayload[i2c_dataComplete])) {
		case i2c_restart_read:
			return SHT31_MOD_I2C_0_do_I2C_SEND_RESTART_READ();
		case i2c_restart_write:
			return SHT31_MOD_I2C_0_do_I2C_SEND_RESTART_WRITE();
		case i2c_continue:
			return SHT31_MOD_I2C_0_do_I2C_TX();
		default:
		case i2c_stop:
			return SHT31_MOD_I2C_0_do_I2C_SEND_STOP();
		}
	}
}

static i2c_fsm_states_t SHT31_MOD_I2C_0_do_I2C_BUS_COLLISION(void)
{
	// Clear bus collision status flag
	TWI0.MSTATUS |= TWI_ARBLOST_bm;
	;
	SHT31_MOD_I2C_0_status.error = I2C_FAIL;
	switch (SHT31_MOD_I2C_0_status.callbackTable[i2c_writeCollision](
	    SHT31_MOD_I2C_0_status.callbackPayload[i2c_writeCollision])) {
	case i2c_restart_read:
		return SHT31_MOD_I2C_0_do_I2C_SEND_RESTART_READ();
	case i2c_restart_write:
		return SHT31_MOD_I2C_0_do_I2C_SEND_RESTART_WRITE();
	default:
		return SHT31_MOD_I2C_0_do_I2C_RESET();
	}
}

static i2c_fsm_states_t SHT31_MOD_I2C_0_do_I2C_BUS_ERROR(void)
{
	TWI0.MCTRLB |= TWI_FLUSH_bm;
	TWI0.MSTATUS |= TWI_BUSSTATE_IDLE_gc;
	SHT31_MOD_I2C_0_status.busy  = false;
	SHT31_MOD_I2C_0_status.error = I2C_FAIL;
	return I2C_RESET; // park the FSM on reset
}

stateHandlerFunction *SHT31_MOD_I2C_0_fsmStateTable[] = {
    SHT31_MOD_I2C_0_do_I2C_IDLE,               // I2C_IDLE
    SHT31_MOD_I2C_0_do_I2C_SEND_ADR_READ,      // I2C_SEND_ADR_READ
    SHT31_MOD_I2C_0_do_I2C_SEND_ADR_WRITE,     // I2C_SEND_ADR_WRITE
    SHT31_MOD_I2C_0_do_I2C_TX,                 // I2C_TX
    SHT31_MOD_I2C_0_do_I2C_RX,                 // I2C_RX
    SHT31_MOD_I2C_0_do_I2C_TX_EMPTY,           // I2C_TX_EMPTY
    SHT31_MOD_I2C_0_do_I2C_SEND_RESTART_READ,  // I2C_SEND_RESTART_READ
    SHT31_MOD_I2C_0_do_I2C_SEND_RESTART_WRITE, // I2C_SEND_RESTART_WRITE
    SHT31_MOD_I2C_0_do_I2C_SEND_RESTART,       // I2C_SEND_RESTART
    SHT31_MOD_I2C_0_do_I2C_SEND_STOP,          // I2C_SEND_STOP
    SHT31_MOD_I2C_0_do_I2C_RX_DO_ACK,          // I2C_RX_DO_ACK
    SHT31_MOD_I2C_0_do_I2C_TX_DO_ACK,          // I2C_TX_DO_ACK
    SHT31_MOD_I2C_0_do_I2C_DO_NACK_STOP,       // I2C_RX_DO_NACK_STOP
    SHT31_MOD_I2C_0_do_I2C_DO_NACK_RESTART,    // I2C_RX_DO_NACK_RESTART
    SHT31_MOD_I2C_0_do_I2C_RESET,              // I2C_RESET
    SHT31_MOD_I2C_0_do_I2C_DO_ADDRESS_NACK,    // I2C_ADDRESS_NACK
    SHT31_MOD_I2C_0_do_I2C_BUS_COLLISION,      // I2C_BUS_COLLISION
    SHT31_MOD_I2C_0_do_I2C_BUS_ERROR           // I2C_BUS_ERROR
};

ISR(TWI0_TWIM_vect)
{
	SHT31_MOD_I2C_0_master_isr();
}

void SHT31_MOD_I2C_0_master_isr(void)
{
	TWI0.MSTATUS |= (TWI_RIF_bm | TWI_WIF_bm);

	// NOTE: We are ignoring the Write Collision flag.

	// Address phase received NACK from slave, override next state
	if (SHT31_MOD_I2C_0_status.addressNACKCheck && (TWI0.MSTATUS & TWI_RXACK_bm)) {
		SHT31_MOD_I2C_0_status.state = I2C_ADDRESS_NACK; // State Override
	}

	// Bus arbitration lost to another master, override next state
	if (TWI0.MSTATUS & TWI_ARBLOST_bm) {
		SHT31_MOD_I2C_0_status.state = I2C_BUS_COLLISION; // State Override
	}

	// Bus error, override next state
	if (TWI0.MSTATUS & TWI_BUSERR_bm) {
		SHT31_MOD_I2C_0_status.state = I2C_BUS_ERROR; // State Override
	}

	SHT31_MOD_I2C_0_status.state = SHT31_MOD_I2C_0_fsmStateTable[SHT31_MOD_I2C_0_status.state]();
}

/************************************************************************/
/* Helper Functions                                                     */
/************************************************************************/
static i2c_operations_t SHT31_MOD_I2C_0_return_stop(void *p)
{
	return i2c_stop;
}

static i2c_operations_t SHT31_MOD_I2C_0_return_reset(void *p)
{
	return i2c_reset_link;
}

static void SHT31_MOD_I2C_0_set_callback(i2c_callback_index idx, i2c_callback cb, void *p)
{
	if (cb) {
		SHT31_MOD_I2C_0_status.callbackTable[idx]   = cb;
		SHT31_MOD_I2C_0_status.callbackPayload[idx] = p;
	} else {
		SHT31_MOD_I2C_0_status.callbackTable[idx]   = SHT31_MOD_I2C_0_return_stop;
		SHT31_MOD_I2C_0_status.callbackPayload[idx] = NULL;
	}
}

i2c_status_t SHT_31_PCB_I2C_1_status = {0};

static void             SHT_31_PCB_I2C_1_set_callback(i2c_callback_index idx, i2c_callback cb, void *p);
static i2c_operations_t SHT_31_PCB_I2C_1_return_stop(void *p);
static i2c_operations_t SHT_31_PCB_I2C_1_return_reset(void *p);
static i2c_fsm_states_t SHT_31_PCB_I2C_1_do_I2C_SEND_ADR_READ(void);
static i2c_fsm_states_t SHT_31_PCB_I2C_1_do_I2C_SEND_ADR_WRITE(void);
static void             SHT_31_PCB_I2C_1_master_isr(void);

/*if timeoutDriverEnabled>
ABSOLUTETIME_t ${i2cMasterFunctions["timeoutHandler"]}(void *p);

// place this function someplace in a periodic interrupt
ABSOLUTETIME_t ${i2cMasterFunctions["timeoutHandler"]}(void *p)
{
    ${msspI2cFunctions["disableIRQ"]}();
    ${i2cMasterFunctions["status"]}.state = I2C_RESET; // Jump to the Timeout state
    ${msspI2cFunctions["enableIRQ"]}();
    ${msspI2cFunctions["setIRQ"]}(); // force an interrupt to handle the timeout
    return 0;
}
*/
/**
 * \brief Set callback to be called when all specifed data has been transferred.
 *
 * \param[in] cb Pointer to callback function
 * \param[in] p  Pointer to the callback function's parameters
 *
 * \return Nothing
 */
void SHT_31_PCB_I2C_1_set_data_complete_callback(i2c_callback cb, void *p)
{
	SHT_31_PCB_I2C_1_set_callback(i2c_dataComplete, cb, p);
}

/**
 * \brief Set callback to be called when there has been a bus collision and arbitration was lost.
 *
 * \param[in] cb Pointer to callback function
 * \param[in] p  Pointer to the callback function's parameters
 *
 * \return Nothing
 */
void SHT_31_PCB_I2C_1_set_write_collision_callback(i2c_callback cb, void *p)
{
	SHT_31_PCB_I2C_1_set_callback(i2c_writeCollision, cb, p);
}

/**
 * \brief Set callback to be called when the transmitted address was NACK'ed.
 *
 * \param[in] cb Pointer to callback function
 * \param[in] p  Pointer to the callback function's parameters
 *
 * \return Nothing
 */
void SHT_31_PCB_I2C_1_set_address_nack_callback(i2c_callback cb, void *p)
{
	SHT_31_PCB_I2C_1_set_callback(i2c_addressNACK, cb, p);
}

/**
 * \brief Set callback to be called when the transmitted data was NACK'ed.
 *
 * \param[in] cb Pointer to callback function
 * \param[in] p  Pointer to the callback function's parameters
 *
 * \return Nothing
 */
void SHT_31_PCB_I2C_1_set_data_nack_callback(i2c_callback cb, void *p)
{
	SHT_31_PCB_I2C_1_set_callback(i2c_dataNACK, cb, p);
}

/**
 * \brief Set callback to be called when there was a bus timeout.
 *
 * \param[in] cb Pointer to callback function
 * \param[in] p  Pointer to the callback function's parameters
 *
 * \return Nothing
 */
void SHT_31_PCB_I2C_1_set_timeout_callback(i2c_callback cb, void *p)
{
	SHT_31_PCB_I2C_1_set_callback(i2c_timeOut, cb, p);
}

/**
 * \brief Initialize I2C interface
 * If module is configured to disabled state, the clock to the I2C is disabled
 * if this is supported by the device's clock system.
 *
 * \return Initialization status.
 * \retval 0 the init was successful
 * \retval 1 the init was not successful
 */
void SHT_31_PCB_I2C_1_init()
{

	// TWI1.CTRLA = 0 << TWI_FMPEN_bp /* FM Plus Enable: disabled */
	//		 | TWI_SDAHOLD_OFF_gc /* SDA hold time off */
	//		 | TWI_SDASETUP_4CYC_gc; /* SDA setup time is 4 clock cycles */

	// TWI1.DBGCTRL = 0 << TWI_DBGRUN_bp; /* Debug Run: disabled */

	TWI1.MBAUD = (uint8_t)TWI1_BAUD(100000, 0); /* set MBAUD register */

	TWI1.MCTRLA = 1 << TWI_ENABLE_bp        /* Enable TWI Master: enabled */
	              | 0 << TWI_QCEN_bp        /* Quick Command Enable: disabled */
	              | 1 << TWI_RIEN_bp        /* Read Interrupt Enable: enabled */
	              | 0 << TWI_SMEN_bp        /* Smart Mode Enable: disabled */
	              | TWI_TIMEOUT_DISABLED_gc /* Bus Timeout Disabled */
	              | 1 << TWI_WIEN_bp;       /* Write Interrupt Enable: enabled */
}

/**
 * \brief Open the I2C for communication
 *
 * \param[in] address The slave address to use in the transfer
 *
 * \return Initialization status.
 * \retval I2C_NOERR The I2C open was successful
 * \retval I2C_BUSY  The I2C open failed because the interface is busy
 * \retval I2C_FAIL  The I2C open failed with an error
 */
i2c_error_t SHT_31_PCB_I2C_1_open(i2c_address_t address)
{
	i2c_error_t ret = I2C_BUSY;

	if (!SHT_31_PCB_I2C_1_status.inUse) {
		SHT_31_PCB_I2C_1_status.address          = address;
		SHT_31_PCB_I2C_1_status.busy             = 0;
		SHT_31_PCB_I2C_1_status.inUse            = 1;
		SHT_31_PCB_I2C_1_status.addressNACKCheck = 0;
		SHT_31_PCB_I2C_1_status.state            = I2C_RESET;
		SHT_31_PCB_I2C_1_status.timeout_value    = 500; // MCC should determine a reasonable starting value here.
		SHT_31_PCB_I2C_1_status.bufferFree       = 1;
		/*
		        <#if timeoutDriverEnabled>
		        SHT_31_PCB_I2C_1_status.timeout.callbackPtr = ${i2cMasterFunctions["timeoutHandler"]};
		        </#if>
		*/
		// set all the call backs to a default of sending stop
		SHT_31_PCB_I2C_1_status.callbackTable[i2c_dataComplete]     = SHT_31_PCB_I2C_1_return_stop;
		SHT_31_PCB_I2C_1_status.callbackPayload[i2c_dataComplete]   = NULL;
		SHT_31_PCB_I2C_1_status.callbackTable[i2c_writeCollision]   = SHT_31_PCB_I2C_1_return_stop;
		SHT_31_PCB_I2C_1_status.callbackPayload[i2c_writeCollision] = NULL;
		SHT_31_PCB_I2C_1_status.callbackTable[i2c_addressNACK]      = SHT_31_PCB_I2C_1_return_stop;
		SHT_31_PCB_I2C_1_status.callbackPayload[i2c_addressNACK]    = NULL;
		SHT_31_PCB_I2C_1_status.callbackTable[i2c_dataNACK]         = SHT_31_PCB_I2C_1_return_stop;
		SHT_31_PCB_I2C_1_status.callbackPayload[i2c_dataNACK]       = NULL;
		SHT_31_PCB_I2C_1_status.callbackTable[i2c_timeOut]          = SHT_31_PCB_I2C_1_return_reset;
		SHT_31_PCB_I2C_1_status.callbackPayload[i2c_timeOut]        = NULL;

		TWI1.MCTRLB |= TWI_FLUSH_bm;
		TWI1.MSTATUS |= TWI_BUSSTATE_IDLE_gc;
		// Reset module
		TWI1.MSTATUS |= (TWI_RIF_bm | TWI_WIF_bm);

		// uncomment the IRQ enable for an interrupt driven driver.
		TWI1.MCTRLA |= (TWI_RIEN_bm | TWI_WIEN_bm);

		ret = I2C_NOERR;
	}
	return ret;
}

void SHT_31_PCB_I2C_1_set_address(i2c_address_t address)
{
	SHT_31_PCB_I2C_1_status.address = address;
}

/**
 * \brief Close the I2C interface
 *
 * \return Status of close operation.
 * \retval I2C_NOERR The I2C close was successful
 * \retval I2C_BUSY  The I2C close failed because the interface is busy
 * \retval I2C_FAIL  The I2C close failed with an error
 */
i2c_error_t SHT_31_PCB_I2C_1_close(void)
{
	i2c_error_t ret = I2C_BUSY;
	// Bus is in error state, reset I2C hardware and report error
	if (TWI1.MSTATUS & TWI_BUSERR_bm) {
		SHT_31_PCB_I2C_1_status.busy  = false;
		SHT_31_PCB_I2C_1_status.error = I2C_FAIL;
	}
	if (!SHT_31_PCB_I2C_1_status.busy) {
		SHT_31_PCB_I2C_1_status.inUse = 0;
		// close it down
		SHT_31_PCB_I2C_1_status.address = 0xff; // 8-bit address is invalid so this is FREE
		TWI1.MSTATUS |= (TWI_RIF_bm | TWI_WIF_bm);
		TWI1.MCTRLA &= ~(TWI_RIEN_bm | TWI_WIEN_bm);
		ret = SHT_31_PCB_I2C_1_status.error;
	}
	return ret;
}

/**
 * \brief Set timeout to be used for I2C operations. Uses the Timeout driver.
 *
 * \param[in] to Timeout in ticks
 *
 * \return Nothing
 */
void SHT_31_PCB_I2C_1_set_timeout(uint8_t to)
{
	TWI1.MCTRLA &= ~(TWI_RIEN_bm | TWI_WIEN_bm);
	SHT_31_PCB_I2C_1_status.timeout_value = to;
	TWI1.MCTRLA |= (TWI_RIEN_bm | TWI_WIEN_bm);
}

/**
 * \brief Set baud rate to be used for I2C operations.
 *
 * \param[in] baud to set the transfer speed
 *
 * \return Nothing
 */
void SHT_31_PCB_I2C_1_set_baud_rate(uint32_t baud)
{
	TWI1.MBAUD = (uint8_t)TWI1_BAUD(baud, 0); /* set MBAUD register */
}

/**
 * \brief Sets up the data buffer to use, and number of bytes to transfer
 *
 * \param[in] buffer Pointer to data buffer to use for read or write data
 * \param[in] bufferSize Number of bytes to read or write from slave
 *
 * \return Nothing
 */
void SHT_31_PCB_I2C_1_set_buffer(void *buffer, size_t bufferSize)
{
	if (SHT_31_PCB_I2C_1_status.bufferFree) {
		SHT_31_PCB_I2C_1_status.data_ptr    = buffer;
		SHT_31_PCB_I2C_1_status.data_length = bufferSize;
		SHT_31_PCB_I2C_1_status.bufferFree  = false;
	}
}

/**
 * \brief Start an operation on an opened I2C interface
 *
 * \param[in] read Set to true for read, false for write
 *
 * \return Status of operation
 * \retval I2C_NOERR The I2C open was successful
 * \retval I2C_BUSY  The I2C open failed because the interface is busy
 * \retval I2C_FAIL  The I2C open failed with an error
 */
i2c_error_t SHT_31_PCB_I2C_1_master_operation(bool read)
{
	i2c_error_t ret = I2C_BUSY;
	if (!SHT_31_PCB_I2C_1_status.busy) {
		SHT_31_PCB_I2C_1_status.busy = true;
		ret                          = I2C_NOERR;

		if (read) {
			SHT_31_PCB_I2C_1_status.state = I2C_SEND_ADR_READ;
		} else {
			SHT_31_PCB_I2C_1_status.state = I2C_SEND_ADR_WRITE;
		}
		SHT_31_PCB_I2C_1_master_isr();
	}
	return ret;
}

/**
 * \brief Identical to SHT_31_PCB_I2C_1_master_operation(true);
 */
i2c_error_t SHT_31_PCB_I2C_1_master_read(void)
{
	return SHT_31_PCB_I2C_1_master_operation(true);
}

/**
 * \brief Identical to SHT_31_PCB_I2C_1_master_operation(false);
 */
i2c_error_t SHT_31_PCB_I2C_1_master_write(void)
{
	return SHT_31_PCB_I2C_1_master_operation(false);
}

/************************************************************************/
/* Helper Functions                                                     */
/************************************************************************/

static i2c_fsm_states_t SHT_31_PCB_I2C_1_do_I2C_RESET(void)
{
	TWI1.MCTRLB |= TWI_FLUSH_bm;
	TWI1.MSTATUS |= TWI_BUSSTATE_IDLE_gc;
	SHT_31_PCB_I2C_1_status.busy  = false; // Bus Free
	SHT_31_PCB_I2C_1_status.error = I2C_NOERR;
	return I2C_RESET; // park the FSM on reset
}

static i2c_fsm_states_t SHT_31_PCB_I2C_1_do_I2C_IDLE(void)
{
	SHT_31_PCB_I2C_1_status.busy  = false; // Bus Free
	SHT_31_PCB_I2C_1_status.error = I2C_NOERR;
	return I2C_IDLE; // park the FSM on IDLE
}

static i2c_fsm_states_t SHT_31_PCB_I2C_1_do_I2C_SEND_RESTART_READ(void)
{
	return SHT_31_PCB_I2C_1_do_I2C_SEND_ADR_READ();
}

static i2c_fsm_states_t SHT_31_PCB_I2C_1_do_I2C_SEND_RESTART_WRITE(void)
{
	return SHT_31_PCB_I2C_1_do_I2C_SEND_ADR_WRITE();
}

static i2c_fsm_states_t SHT_31_PCB_I2C_1_do_I2C_SEND_RESTART(void)
{
	return SHT_31_PCB_I2C_1_do_I2C_SEND_ADR_READ();
}

static i2c_fsm_states_t SHT_31_PCB_I2C_1_do_I2C_SEND_STOP(void)
{
	TWI1.MCTRLB |= TWI_MCMD_STOP_gc;
	return SHT_31_PCB_I2C_1_do_I2C_IDLE();
}

// TODO: probably need 2 addressNACK's one from read and one from write.
//       the do NACK before RESTART or STOP is a special case that a new state simplifies.
static i2c_fsm_states_t SHT_31_PCB_I2C_1_do_I2C_DO_ADDRESS_NACK(void)
{
	SHT_31_PCB_I2C_1_status.addressNACKCheck = 0;
	SHT_31_PCB_I2C_1_status.error            = I2C_FAIL;
	switch (SHT_31_PCB_I2C_1_status.callbackTable[i2c_addressNACK](
	    SHT_31_PCB_I2C_1_status.callbackPayload[i2c_addressNACK])) {
	case i2c_restart_read:
		return SHT_31_PCB_I2C_1_do_I2C_SEND_RESTART_READ();
	case i2c_restart_write:
		return SHT_31_PCB_I2C_1_do_I2C_SEND_RESTART_WRITE();
	default:
		return SHT_31_PCB_I2C_1_do_I2C_SEND_STOP();
	}
}

static i2c_fsm_states_t SHT_31_PCB_I2C_1_do_I2C_SEND_ADR_READ(void)
{

	SHT_31_PCB_I2C_1_status.addressNACKCheck = 1;
	TWI1.MADDR                               = SHT_31_PCB_I2C_1_status.address << 1 | 1;
	return I2C_RX;
}

static i2c_fsm_states_t SHT_31_PCB_I2C_1_do_I2C_SEND_ADR_WRITE(void)
{

	SHT_31_PCB_I2C_1_status.addressNACKCheck = 1;
	TWI1.MADDR                               = SHT_31_PCB_I2C_1_status.address << 1;
	return I2C_TX;
}

static i2c_fsm_states_t SHT_31_PCB_I2C_1_do_I2C_RX_DO_ACK(void)
{
	TWI1.MCTRLB &= ~(1 << TWI_ACKACT_bp);
	return I2C_RX;
}

static i2c_fsm_states_t SHT_31_PCB_I2C_1_do_I2C_TX_DO_ACK(void)
{
	TWI1.MCTRLB &= ~(1 << TWI_ACKACT_bp);
	return I2C_TX;
}

static i2c_fsm_states_t SHT_31_PCB_I2C_1_do_I2C_DO_NACK_STOP(void)
{
	TWI1.MCTRLB |= TWI_ACKACT_NACK_gc;
	TWI1.MCTRLB |= TWI_MCMD_STOP_gc;
	return SHT_31_PCB_I2C_1_do_I2C_IDLE();
}

static i2c_fsm_states_t SHT_31_PCB_I2C_1_do_I2C_DO_NACK_RESTART(void)
{
	TWI1.MCTRLB |= TWI_ACKACT_NACK_gc;
	return I2C_SEND_RESTART;
}

static i2c_fsm_states_t SHT_31_PCB_I2C_1_do_I2C_TX(void)
{
	if ((TWI1.MSTATUS & TWI_RXACK_bm)) // Slave replied with NACK
	{
		switch (SHT_31_PCB_I2C_1_status.callbackTable[i2c_dataNACK](
		    SHT_31_PCB_I2C_1_status.callbackPayload[i2c_dataNACK])) {
		case i2c_restart_read:
			return SHT_31_PCB_I2C_1_do_I2C_SEND_RESTART_READ();
		case i2c_restart_write:
			return SHT_31_PCB_I2C_1_do_I2C_SEND_RESTART_WRITE();
		default:
		case i2c_continue:
		case i2c_stop:
			return SHT_31_PCB_I2C_1_do_I2C_SEND_STOP();
		}
	} else {
		SHT_31_PCB_I2C_1_status.addressNACKCheck = 0;
		TWI1.MDATA                               = *SHT_31_PCB_I2C_1_status.data_ptr++;
		return (--SHT_31_PCB_I2C_1_status.data_length) ? I2C_TX : I2C_TX_EMPTY;
	}
}

static i2c_fsm_states_t SHT_31_PCB_I2C_1_do_I2C_RX(void)
{
	SHT_31_PCB_I2C_1_status.addressNACKCheck = 0;

	if (SHT_31_PCB_I2C_1_status.data_length == 1)
		TWI1.MCTRLB |= TWI_ACKACT_NACK_gc; // Next byte will be last to be received, setup NACK
	else
		TWI1.MCTRLB &= ~(1 << TWI_ACKACT_bp); // More bytes to receive, setup ACK

	if (--SHT_31_PCB_I2C_1_status.data_length) {
		*SHT_31_PCB_I2C_1_status.data_ptr = TWI1.MDATA;
		SHT_31_PCB_I2C_1_status.data_ptr++;
		TWI1.MCTRLB |= TWI_MCMD_RECVTRANS_gc;
		return I2C_RX;
	} else {
		*SHT_31_PCB_I2C_1_status.data_ptr = TWI1.MDATA;
		SHT_31_PCB_I2C_1_status.data_ptr++;
		SHT_31_PCB_I2C_1_status.bufferFree = true;
		switch (SHT_31_PCB_I2C_1_status.callbackTable[i2c_dataComplete](
		    SHT_31_PCB_I2C_1_status.callbackPayload[i2c_dataComplete])) {
		case i2c_restart_write:
		case i2c_restart_read:
			return SHT_31_PCB_I2C_1_do_I2C_DO_NACK_RESTART();
		default:
		case i2c_continue:
		case i2c_stop:
			return SHT_31_PCB_I2C_1_do_I2C_DO_NACK_STOP();
		}
	}
}

static i2c_fsm_states_t SHT_31_PCB_I2C_1_do_I2C_TX_EMPTY(void)
{
	if ((TWI1.MSTATUS & TWI_RXACK_bm)) // Slave replied with NACK
	{
		switch (SHT_31_PCB_I2C_1_status.callbackTable[i2c_dataNACK](
		    SHT_31_PCB_I2C_1_status.callbackPayload[i2c_dataNACK])) {
		case i2c_restart_read:
			return SHT_31_PCB_I2C_1_do_I2C_SEND_RESTART_READ();
		case i2c_restart_write:
			return SHT_31_PCB_I2C_1_do_I2C_SEND_RESTART_WRITE();
		default:
		case i2c_continue:
		case i2c_stop:
			return SHT_31_PCB_I2C_1_do_I2C_SEND_STOP();
		}
	} else {
		SHT_31_PCB_I2C_1_status.bufferFree = true;
		switch (SHT_31_PCB_I2C_1_status.callbackTable[i2c_dataComplete](
		    SHT_31_PCB_I2C_1_status.callbackPayload[i2c_dataComplete])) {
		case i2c_restart_read:
			return SHT_31_PCB_I2C_1_do_I2C_SEND_RESTART_READ();
		case i2c_restart_write:
			return SHT_31_PCB_I2C_1_do_I2C_SEND_RESTART_WRITE();
		case i2c_continue:
			return SHT_31_PCB_I2C_1_do_I2C_TX();
		default:
		case i2c_stop:
			return SHT_31_PCB_I2C_1_do_I2C_SEND_STOP();
		}
	}
}

static i2c_fsm_states_t SHT_31_PCB_I2C_1_do_I2C_BUS_COLLISION(void)
{
	// Clear bus collision status flag
	TWI1.MSTATUS |= TWI_ARBLOST_bm;
	;
	SHT_31_PCB_I2C_1_status.error = I2C_FAIL;
	switch (SHT_31_PCB_I2C_1_status.callbackTable[i2c_writeCollision](
	    SHT_31_PCB_I2C_1_status.callbackPayload[i2c_writeCollision])) {
	case i2c_restart_read:
		return SHT_31_PCB_I2C_1_do_I2C_SEND_RESTART_READ();
	case i2c_restart_write:
		return SHT_31_PCB_I2C_1_do_I2C_SEND_RESTART_WRITE();
	default:
		return SHT_31_PCB_I2C_1_do_I2C_RESET();
	}
}

static i2c_fsm_states_t SHT_31_PCB_I2C_1_do_I2C_BUS_ERROR(void)
{
	TWI1.MCTRLB |= TWI_FLUSH_bm;
	TWI1.MSTATUS |= TWI_BUSSTATE_IDLE_gc;
	SHT_31_PCB_I2C_1_status.busy  = false;
	SHT_31_PCB_I2C_1_status.error = I2C_FAIL;
	return I2C_RESET; // park the FSM on reset
}

stateHandlerFunction *SHT_31_PCB_I2C_1_fsmStateTable[] = {
    SHT_31_PCB_I2C_1_do_I2C_IDLE,               // I2C_IDLE
    SHT_31_PCB_I2C_1_do_I2C_SEND_ADR_READ,      // I2C_SEND_ADR_READ
    SHT_31_PCB_I2C_1_do_I2C_SEND_ADR_WRITE,     // I2C_SEND_ADR_WRITE
    SHT_31_PCB_I2C_1_do_I2C_TX,                 // I2C_TX
    SHT_31_PCB_I2C_1_do_I2C_RX,                 // I2C_RX
    SHT_31_PCB_I2C_1_do_I2C_TX_EMPTY,           // I2C_TX_EMPTY
    SHT_31_PCB_I2C_1_do_I2C_SEND_RESTART_READ,  // I2C_SEND_RESTART_READ
    SHT_31_PCB_I2C_1_do_I2C_SEND_RESTART_WRITE, // I2C_SEND_RESTART_WRITE
    SHT_31_PCB_I2C_1_do_I2C_SEND_RESTART,       // I2C_SEND_RESTART
    SHT_31_PCB_I2C_1_do_I2C_SEND_STOP,          // I2C_SEND_STOP
    SHT_31_PCB_I2C_1_do_I2C_RX_DO_ACK,          // I2C_RX_DO_ACK
    SHT_31_PCB_I2C_1_do_I2C_TX_DO_ACK,          // I2C_TX_DO_ACK
    SHT_31_PCB_I2C_1_do_I2C_DO_NACK_STOP,       // I2C_RX_DO_NACK_STOP
    SHT_31_PCB_I2C_1_do_I2C_DO_NACK_RESTART,    // I2C_RX_DO_NACK_RESTART
    SHT_31_PCB_I2C_1_do_I2C_RESET,              // I2C_RESET
    SHT_31_PCB_I2C_1_do_I2C_DO_ADDRESS_NACK,    // I2C_ADDRESS_NACK
    SHT_31_PCB_I2C_1_do_I2C_BUS_COLLISION,      // I2C_BUS_COLLISION
    SHT_31_PCB_I2C_1_do_I2C_BUS_ERROR           // I2C_BUS_ERROR
};

ISR(TWI1_TWIM_vect)
{
	SHT_31_PCB_I2C_1_master_isr();
}

void SHT_31_PCB_I2C_1_master_isr(void)
{
	TWI1.MSTATUS |= (TWI_RIF_bm | TWI_WIF_bm);

	// NOTE: We are ignoring the Write Collision flag.

	// Address phase received NACK from slave, override next state
	if (SHT_31_PCB_I2C_1_status.addressNACKCheck && (TWI1.MSTATUS & TWI_RXACK_bm)) {
		SHT_31_PCB_I2C_1_status.state = I2C_ADDRESS_NACK; // State Override
	}

	// Bus arbitration lost to another master, override next state
	if (TWI1.MSTATUS & TWI_ARBLOST_bm) {
		SHT_31_PCB_I2C_1_status.state = I2C_BUS_COLLISION; // State Override
	}

	// Bus error, override next state
	if (TWI1.MSTATUS & TWI_BUSERR_bm) {
		SHT_31_PCB_I2C_1_status.state = I2C_BUS_ERROR; // State Override
	}

	SHT_31_PCB_I2C_1_status.state = SHT_31_PCB_I2C_1_fsmStateTable[SHT_31_PCB_I2C_1_status.state]();
}

/************************************************************************/
/* Helper Functions                                                     */
/************************************************************************/
static i2c_operations_t SHT_31_PCB_I2C_1_return_stop(void *p)
{
	return i2c_stop;
}

static i2c_operations_t SHT_31_PCB_I2C_1_return_reset(void *p)
{
	return i2c_reset_link;
}

static void SHT_31_PCB_I2C_1_set_callback(i2c_callback_index idx, i2c_callback cb, void *p)
{
	if (cb) {
		SHT_31_PCB_I2C_1_status.callbackTable[idx]   = cb;
		SHT_31_PCB_I2C_1_status.callbackPayload[idx] = p;
	} else {
		SHT_31_PCB_I2C_1_status.callbackTable[idx]   = SHT_31_PCB_I2C_1_return_stop;
		SHT_31_PCB_I2C_1_status.callbackPayload[idx] = NULL;
	}
}
