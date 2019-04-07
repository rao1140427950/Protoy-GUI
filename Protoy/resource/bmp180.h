#ifndef _BMP180_H_
#define _BMP180_H_

#include "typedef.h"

#define BMP180_LABEL 0x05
#define BMP180_RDPRESS 0x01

uint32 rd_pres(uint8 addr);

#endif