/**
 * \file
 *
 * \brief TCB related functionality implementation.
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
 * \addtogroup doc_driver_tcb
 *
 * \section doc_driver_tcb_rev Revision History
 * - v0.0.0.1 Initial Commit
 *
 *@{
 */
#include <tcb.h>

/**
 * \brief Initialize tcb interface
 *
 * \return Initialization status.
 */
int8_t TIMER_0_init()
{

	TCB1.CCMP = 0x0; /* Compare or Capture: 0xfff */

	// TCB1.CNT = 0x0; /* Count: 0x0 */

	// TCB1.CTRLB = 0 << TCB_ASYNC_bp /* Asynchronous Enable: disabled */
	//		 | 0 << TCB_CCMPEN_bp /* Pin Output Enable: disabled */
	//		 | 0 << TCB_CCMPINIT_bp /* Pin Initial State: disabled */
	//		 | TCB_CNTMODE_INT_gc; /* Periodic Interrupt */

	// TCB1.DBGCTRL = 0 << TCB_DBGRUN_bp; /* Debug Run: disabled */

	// TCB1.EVCTRL = 0 << TCB_CAPTEI_bp /* Event Input Enable: disabled */
	//		 | 0 << TCB_EDGE_bp /* Event Edge: disabled */
	//		 | 0 << TCB_FILTER_bp; /* Input Capture Noise Cancellation Filter: disabled */

	TCB1.INTCTRL = 1 << TCB_CAPT_bp   /* Capture or Timeout: enabled */
	               | 1 << TCB_OVF_bp; /* OverFlow Interrupt: enabled */

	TCB1.CTRLA = TCB_CLKSEL_DIV2_gc     /* CLK_PER/2 */
	             | 1 << TCB_ENABLE_bp   /* Enable: enabled */
	             | 0 << TCB_RUNSTDBY_bp /* Run Standby: disabled */
	             | 0 << TCB_SYNCUPD_bp  /* Synchronize Update: disabled */
	             | 0 << TCB_CASCADE_bp; /* Cascade Two Timer/Counters: disabled */

	return 0;
}

void TIMER_0_disable(void)
{
	TCB1.CTRLA &= ~TCB_ENABLE_bm;
}

void TIMER_0_enable(void)
{
	TCB1.CTRLA |= TCB_ENABLE_bm;
}
