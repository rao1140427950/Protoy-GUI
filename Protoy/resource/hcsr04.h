#ifndef _HCSR04_H_
#define _HCSR04_H_

#include "typedef.h"

#define HCSR04_LABEL 0x04
#define HCSR04_RDDIST 0x01

uint8 rd_distance(uint8 addr);

#endif