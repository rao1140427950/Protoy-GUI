#ifndef _PROTOCOL_H_
#define _PROTOCOL_H_

#include "stc15fwxxx.h"
#include "typedef.h"

sbit SCL = P3^2;
sbit SDA = P3^3;

#define CONTROLLER_ADDR 0x01

#define CHECK_ONLINE 0x01
#define MOUNT_PERIPHERAL 0x02
#define SEND_ORDERS 0x03

#define ONLINE_DIV  0x80

#define BROADCAST 0x80

//sbit LED = P3^6;

//typedef unsigned char uint8;
//typedef unsigned int uint16;

uint8 BusSend(uint8* dat, uint8 addr, uint8 n);
uint8 BusRecieve(uint8* dat, uint8 addr, uint16 timeout);
void initProtocal();
void sendAck(uint8 k);
uint8 recieveAck();
void Delay10ms();
void Delay5ms();
void Delay1ms();
void Delay100us();
void Delay50us();

#endif