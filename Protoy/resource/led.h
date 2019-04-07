#ifndef _LED_H_
#define _LED_H_

#define LED_LABEL 0x00
#define LED_ON 0x01
#define LED_OFF 0x02
#define LED_BLINK 0x03

void led_on(unsigned char addr);
void led_off(unsigned char addr);
void led_blink(unsigned char addr, unsigned char x);

#endif