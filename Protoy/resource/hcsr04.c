#include "hcsr04.h"
#include "protocol.h"

uint8 rd_distance(uint8 addr)
{
    uint8 dat[3];
    uint8 n;

    dat[0] = SEND_ORDERS;
    dat[1] = HCSR04_RDDIST;
    BusSend(dat, addr, 2);
    n = BusRecieve(dat, addr, 100);
    if(n == 0 || dat[1] != HCSR04_RDDIST) return 0;
    else return dat[2];

    return 0;
}