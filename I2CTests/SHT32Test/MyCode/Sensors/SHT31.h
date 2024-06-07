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
class SHT31Class {
public:
  uint16_t begin(uint8_t i2caddr = SHT31_DEFAULT_ADDR);
  float readTemperature(void);
  float readHumidity(void);
  uint16_t readBoth(int32_t *temperature_out, int32_t *humidity_out);
  uint16_t readStatus(void);
  uint16_t reset(void);
  //void heater(bool h);
  //bool isHeaterEnabled();
	
private:
  /**
   * Placeholder to track humidity internally.
   */
  int32_t humidity;

  /**
   * Placeholder to track temperature internally.
   */
  int32_t temp;
  uint8_t i2cAddr;
  uint16_t readTempHum(void);
  uint16_t writeCommand(uint16_t cmd,uint8_t expectedResponseBytes);

};
#endif
