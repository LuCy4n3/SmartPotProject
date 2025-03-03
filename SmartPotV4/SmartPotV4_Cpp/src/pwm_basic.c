/**
 * \file
 *
 * \brief PWM Basic driver implementation.
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
 * \defgroup doc_driver_pwm_basic PWM Basic Driver
 * \ingroup doc_driver_pwm
 *
 * \section doc_driver_pwm_normal_rev Revision History
 * - v0.0.0.1 Initial Commit
 *
 *@{
 */
#include <pwm_basic.h>

/** Function pointer to callback function called by IRQ.
    NULL=default value: No callback function is to be used.
*/
pwm_irq_cb_t PWM_LED_cb = NULL;

/**
 * \brief Initialize PWM
 * If module is configured to disabled state, the clock to the PWM is disabled
 * if this is supported by the device's clock system.
 *
 * \return Initialization status.
 */
int8_t PWM_LED_init()
{

	TCA0.SINGLE.CMP0 = 0xfff; /* Setting: 0xfff */

	// TCA0.SINGLE.CMP1 = 0x0; /* Setting: 0x0 */

	// TCA0.SINGLE.CMP2 = 0x0; /* Setting: 0x0 */

	// TCA0.SINGLE.CNT = 0x0; /* Count: 0x0 */

	TCA0.SINGLE.CTRLB = 0 << TCA_SINGLE_ALUPD_bp            /* Auto Lock Update: disabled */
	                    | 1 << TCA_SINGLE_CMP0EN_bp         /* Setting: enabled */
	                    | 0 << TCA_SINGLE_CMP1EN_bp         /* Setting: disabled */
	                    | 0 << TCA_SINGLE_CMP2EN_bp         /* Setting: disabled */
	                    | TCA_SINGLE_WGMODE_SINGLESLOPE_gc; /*  */

	// TCA0.SINGLE.CTRLC = 0 << TCA_SINGLE_CMP0OV_bp /* Setting: disabled */
	//		 | 0 << TCA_SINGLE_CMP1OV_bp /* Setting: disabled */
	//		 | 0 << TCA_SINGLE_CMP2OV_bp; /* Setting: disabled */

	// TCA0.SINGLE.DBGCTRL = 0 << TCA_SINGLE_DBGRUN_bp; /* Debug Run: disabled */

	// TCA0.SINGLE.INTCTRL = 0 << TCA_SINGLE_CMP0_bp /* Compare 0 Interrupt: disabled */
	//		 | 0 << TCA_SINGLE_CMP1_bp /* Compare 1 Interrupt: disabled */
	//		 | 0 << TCA_SINGLE_CMP2_bp /* Compare 2 Interrupt: disabled */
	//		 | 0 << TCA_SINGLE_OVF_bp; /* Overflow Interrupt Enable: disabled */

	// TCA0.SINGLE.PER = 0xffff; /* Top Value: 0xffff */

	TCA0.SINGLE.CTRLA = TCA_SINGLE_CLKSEL_DIV1_gc /* System Clock */
	                    | 1 << TCA_SINGLE_ENABLE_bp /* Module Enable: enabled */;

	return 0;
}

/**
 * \brief Enable PWM_LED
 * 1. If supported by the clock system, enables the clock to the PWM
 * 2. Enables the PWM module by setting the enable-bit in the PWM control register
 *
 * \return Nothing
 */
void PWM_LED_enable()
{
	TCA0.SINGLE.CTRLA |= TCA_SINGLE_ENABLE_bm;
}

/**
 * \brief Disable PWM_LED
 * 1. Disables the PWM module by clearing the enable-bit in the PWM control register
 * 2. If supported by the clock system, disables the clock to the PWM
 *
 * \return Nothing
 */
void PWM_LED_disable()
{
	TCA0.SINGLE.CTRLA &= ~TCA_SINGLE_ENABLE_bm;
}

/**
 * \brief Enable PWM output on channel 0
 *
 * \return Nothing
 */
void PWM_LED_enable_output_ch0()
{
	TCA0.SINGLE.CTRLB |= (1 << TCA_SINGLE_CMP0EN_bp);
}

/**
 * \brief Disable PWM output on channel 0
 *
 * \return Nothing
 */
void PWM_LED_disable_output_ch0()
{
	TCA0.SINGLE.CTRLB &= ~(1 << TCA_SINGLE_CMP0EN_bp);
}

/**
 * \brief Enable PWM output on channel 1
 *
 * \return Nothing
 */
void PWM_LED_enable_output_ch1()
{
	TCA0.SINGLE.CTRLB |= (1 << TCA_SINGLE_CMP1EN_bp);
}

/**
 * \brief Disable PWM output on channel 1
 *
 * \return Nothing
 */
void PWM_LED_disable_output_ch1()
{
	TCA0.SINGLE.CTRLB &= ~(1 << TCA_SINGLE_CMP1EN_bp);
}

/**
 * \brief Enable PWM output on channel 2
 *
 * \return Nothing
 */
void PWM_LED_enable_output_ch2()
{
	TCA0.SINGLE.CTRLB |= (1 << TCA_SINGLE_CMP2EN_bp);
}

/**
 * \brief Disable PWM output on channel 2
 *
 * \return Nothing
 */
void PWM_LED_disable_output_ch2()
{
	TCA0.SINGLE.CTRLB &= ~(1 << TCA_SINGLE_CMP2EN_bp);
}

/**
 * \brief Load COUNTER register in PWM_LED
 *
 * \param[in] counter_value The value to load into COUNTER
 *
 * \return Nothing
 */
void PWM_LED_load_counter(PWM_LED_register_t counter_value)
{
	TCA0.SINGLE.CNT = counter_value;
}

/**
 * \brief Load TOP register in PWM_LED.
 * The physical register may different names, depending on the hardware and module mode.
 *
 * \param[in] counter_value The value to load into TOP.
 *
 * \return Nothing
 */
void PWM_LED_load_top(PWM_LED_register_t top_value)
{
	TCA0.SINGLE.PERBUF = top_value;
}

/**
 * \brief Load duty cycle register in for channel 0.
 * The physical register may have different names, depending on the hardware.
 * This is not the duty cycle as percentage of the whole period, but the actual
 * counter compare value.
 *
 * \param[in] counter_value The value to load into the duty cycle register.
 *
 * \return Nothing
 */
void PWM_LED_load_duty_cycle_ch0(PWM_LED_register_t duty_value)
{
	TCA0.SINGLE.CMP0BUF = duty_value;
}

/**
 * \brief Load duty cycle register in for channel 1.
 * The physical register may have different names, depending on the hardware.
 * This is not the duty cycle as percentage of the whole period, but the actual
 * counter compare value.
 *
 * \param[in] counter_value The value to load into the duty cycle register.
 *
 * \return Nothing
 */
void PWM_LED_load_duty_cycle_ch1(PWM_LED_register_t duty_value)
{
	TCA0.SINGLE.CMP1BUF = duty_value;
}

/**
 * \brief Load duty cycle register in for channel 2.
 * The physical register may have different names, depending on the hardware.
 * This is not the duty cycle as percentage of the whole period, but the actual
 * counter compare value.
 *
 * \param[in] counter_value The value to load into the duty cycle register.
 *
 * \return Nothing
 */
void PWM_LED_load_duty_cycle_ch2(PWM_LED_register_t duty_value)
{
	TCA0.SINGLE.CMP2BUF = duty_value;
}

/**
 * \brief Register a callback function to be called at the end of the overflow ISR.
 *
 * \param[in] f Pointer to function to be called
 *
 * \return Nothing.
 */
void PWM_LED_register_callback(pwm_irq_cb_t f)
{
	PWM_LED_cb = f;
}

ISR(TCA0_OVF_vect)
{
	static volatile uint8_t callback_count = 0;

	// Clear the interrupt flag
	TCA0.SINGLE.INTFLAGS = TCA_SINGLE_OVF_bm;

	// callback function - called every 0 passes
	if ((++callback_count >= PWM_LED_INTERRUPT_CB_RATE) && (PWM_LED_INTERRUPT_CB_RATE != 0)) {
		if (PWM_LED_cb != NULL) {
			PWM_LED_cb();
		}
	}
}
