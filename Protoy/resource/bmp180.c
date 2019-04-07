#include "bmp180.h"
#include "typedef.h"
#include "protocol.h"

uint32 rd_pres(uint8 addr)
{
    uint8 dat[5];
    uint8 n;
    uint32 temp = 0;
    uint32 pressure = 0;

    dat[0] = SEND_ORDERS;
    dat[1] = BMP180_RDPRESS;
    BusSend(dat, addr, 2);
    n = BusRecieve(dat, CONTROLLER_ADDR, 500);
    if (n == 0 || dat[0] != BMP180_RDPRESS) return 0;
    else 
    {
        temp = dat[1];
        pressure |= (temp << 32);
        temp = dat[2];
        pressure |= (temp << 16);
        temp = dat[3];
        pressure |= (temp << 8);
        temp = dat[4];
        pressure |= temp;
    }
    return pressure;
}