#ifndef _SERVO_H_
#define _SERVO_H_

#include "typedef.h"

#define SERVO_LABEL 0x02
#define SERVO_ROTATE 0x01

void drive_servo(uint8 addr, uint8 angle);

#endif