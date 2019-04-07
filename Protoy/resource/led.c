#include "led.h"
#include "protocol.h"
#include "typedef.h"

void led_on(uint8 addr)
{
    uint8 dat[2];

    dat[0] = SEND_ORDERS;
    dat[1] = LED_ON;
    BusSend(dat, addr, 2);
}

void led_off(uint8 addr)
{
    uint8 dat[2];

    dat[0] = SEND_ORDERS;
    dat[1] = LED_OFF;
    BusSend(dat, addr, 2);
}

void led_blink(uint8 addr, uint8 x)
{
    uint8 dat[3];

    dat[0] = SEND_ORDERS;
    dat[1] = LED_BLINK;
    dat[2] = x;
    BusSend(dat, addr, 3);
}