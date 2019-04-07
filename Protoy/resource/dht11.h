#ifndef _DHT11_H_
#define _DHT11_H_

#include "typedef.h"

#define DHT11_LABEL 0x03
#define DHT11_RDTEMP 0x01
#define DHT11_RDHUMI 0x02

uint8 rd_temp(uint8 addr);
uint8 rd_humi(uint8 addr);

#endif