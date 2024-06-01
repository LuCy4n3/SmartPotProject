/*!
 *  @file _SHT31.h
 *
 *  This is a library for the SHT31 Digital Humidity & Temp Sensor
 *
 *  Designed specifically to work with the  Digital Humidity & Temp Sensor
 *  -----> https://www..com/product/2857
 *
 *  These sensors use I2C to communicate, 2 pins are required to interface
 *
 *   invests time and resources providing this open source code,
 *  please support  andopen-source hardware by purchasing products
 *  from !
 *
 *  Limor Fried/Ladyada ( Industries).
 *
 *  BSD license, all text above must be included in any redistribution
 */

#ifndef SHT31_H
#define SHT31_H
#include <atmel_start.h>
#include <include/i2c_master.h>

//#include <stdbool.h>

#ifdef __cplusplus
extern "C" {
	#endif

#define SHT31_DEFAULT_ADDR 0x44 /**< SHT31 Default Address */
#define SHT31_MEAS_HIGHREP_STRETCH                                             \
  0x2C06 /**< Measurement High Repeatability with Clock Stretch Enabled */
#define SHT31_MEAS_MEDREP_STRETCH                                              \
  0x2C0D /**< Measurement Medium Repeatability with Clock Stretch Enabled */
#define SHT31_MEAS_LOWREP_STRETCH                                              \
  0x2C10 /**< Measurement Low Repeatability with Clock Stretch Enabled*/
#define SHT31_MEAS_HIGHREP                                                     \
  0x2400 /**< Measurement High Repeatability with Clock Stretch Disabled */
#define SHT31_MEAS_MEDREP                                                      \
  0x240B /**< Measurement Medium Repeatability with Clock Stretch Disabled */
#define SHT31_MEAS_LOWREP                                                      \
  0x2416 /**< Measurement Low Repeatability with Clock Stretch Disabled */
#define SHT31_READSTATUS 0xF32D   /**< Read Out of Status Register */
#define SHT31_CLEARSTATUS 0x3041  /**< Clear Status */
#define SHT31_SOFTRESET 0x30A2    /**< Soft Reset */
#define SHT31_HEATEREN 0x306D     /**< Heater Enable */
#define SHT31_HEATERDIS 0x3066    /**< Heater Disable */
#define SHT31_REG_HEATER_BIT 0x0d /**< Status Register Heater Bit */



/**
 * Driver for the  SHT31-D Temperature and Humidity breakout board.
 */

  /**
   * Placeholder to track temperature internally.
   */
	uint8_t testFunct(uint8_t test);
	uint16_t readTempHum(void);
class testclass
{
	public:
	uint8_t testfunct(uint8_t test);
	protected:
	private:
};


#endif
