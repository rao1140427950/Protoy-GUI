#include "servo.h"
#include "protocol.h"
#include "typedef.h"

void drive_servo(uint8 addr, uint8 angle)
{
    uint8 dat[3];

    dat[0] = SEND_ORDERS;
    dat[1] = SERVO_ROTATE;
    dat[2] = angle;
    BusSend(dat, addr, 3);
}