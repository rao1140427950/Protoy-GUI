#ifndef _EEPROM_H_
#define _EEPROM_H_

#include "typedef.h"

#define EEPROM_START_ADDR 0x0000

void IapDelay(uint8 n);
void IapIdle();
uint8 IapReadByte(uint16 addr);
void IapProgramByte(uint16 addr, uint8 dat);
void IapEraseSector(uint16 addr);

#endif