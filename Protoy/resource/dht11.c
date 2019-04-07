#include "dht11.h"
#include "typedef.h"
#include "protocol.h"

uint8 rd_temp(uint8 addr)
{
    uint8 dat[3];
    uint8 n;

    dat[0] = SEND_ORDERS;
    dat[1] = DHT11_RDTEMP;
    BusSend(dat, addr, 2);
    n = BusRecieve(dat, CONTROLLER_ADDR, 500);
    if (n == 0 || dat[0] != DHT11_RDTEMP) return 0;
    else return dat[1];

    return 0;
}

uint8 rd_humi(uint8 addr)
{
    uint8 dat[3];
    uint8 n;

    dat[0] = SEND_ORDERS;
    dat[1] = DHT11_RDHUMI;
    BusSend(dat, addr, 2);
    n = BusRecieve(dat, CONTROLLER_ADDR, 500);
    if (n == 0 || dat[0] != DHT11_RDHUMI) return 0;
    else return dat[1];

    return 0;
}
