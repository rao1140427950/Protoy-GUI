// Controller
// @ 11.0592MHz
#include "stc15fwxxx.h"
#include "protocol.h"
#include "eeprom.h"
#include "intrins.h"
#include "dht11.h"
#include "servo.h"
#include "led.h"
#include "bmp180.h"
//#include "instructions.h"

//typedef unsigned int WORD;

#define FOSC 11059200L //系统频率
#define BAUD 115200    //串口波特率

#define NONE_PARITY 0  //无校验
#define ODD_PARITY 1   //奇校验
#define EVEN_PARITY 2  //偶校验
#define MARK_PARITY 3  //标记校验
#define SPACE_PARITY 4 //空白校验

#define PARITYBIT EVEN_PARITY //定义校验位

#define S1_S0 0x40 //P_SW1.6
#define S1_S1 0x80 //P_SW1.7

#define MAX_PERIPHERALS 128 // This must be a bug...
#define MAX_CHECKID 10      // This must be a bug...
#define MAX_INSTRUCTIONS 10 // This must be a bug...

bit busy;
bit sndper = 0;

uint8 xdata peripherals[MAX_PERIPHERALS];
uint8 n = 0;
uint8 dat[MAX_INSTRUCTIONS];
uint8 myAddr = CONTROLLER_ADDR;

uint16 a, b, c, x, y, z;

void SendByte(uint8 dat);
void SendString(char *s);
void SendArray(uint8 *dat, uint8 n);
void InitSystem();
void ReloadPeripherals();
void CheckPeripherals();
void RefreshEEPROM();
void BusDecode(uint8 *dat, uint8 n);
void MountPeripheral(uint8 label);
void Delay1000ms();
void Delay_ms(uint16 n);

void main()
{
    uint8 temp;

    InitSystem();
    //SendArray(peripherals, MAX_CHECKID);

    while (1)
    {
        if (sndper)
        {
            SendArray(peripherals, MAX_CHECKID);
            sndper = 0;
        }
        n = BusRecieve(dat, myAddr, 10);
        if (n > 0 && n < BROADCAST) // 如果收到非广播信号
        {
            //SendArray(dat, n);
            BusDecode(dat, n);
            //SendArray(peripherals, MAX_CHECKID);
            //SendArray(dat, n);
        }
        a =
            rd_temp(4);
        if (a > 20)
        {
            led_on(2);
        }
        else
        {
            led_off(2);
        }
        Delay_ms(2000);
    }
}

void InitSystem()
{
    P0M0 = 0x00;
    P0M1 = 0x00;
    P1M0 = 0x00;
    P1M1 = 0x00;
    P2M0 = 0x00;
    P2M1 = 0x00;
    P3M0 = 0x00;
    P3M1 = 0x00;
    P4M0 = 0x00;
    P4M1 = 0x00;
    P5M0 = 0x00;
    P5M1 = 0x00;
    P6M0 = 0x00;
    P6M1 = 0x00;
    P7M0 = 0x00;
    P7M1 = 0x00;

    ACC = P_SW1;
    ACC &= ~(S1_S0 | S1_S1); //S1_S0=0 S1_S1=0
    P_SW1 = ACC;             //(P3.0/RxD, P3.1/TxD)

    //  ACC = P_SW1;
    //  ACC &= ~(S1_S0 | S1_S1);    //S1_S0=1 S1_S1=0
    //  ACC |= S1_S0;               //(P3.6/RxD_2, P3.7/TxD_2)
    //  P_SW1 = ACC;
    //
    //  ACC = P_SW1;
    //  ACC &= ~(S1_S0 | S1_S1);    //S1_S0=0 S1_S1=1
    //  ACC |= S1_S1;               //(P1.6/RxD_3, P1.7/TxD_3)
    //  P_SW1 = ACC;

#if (PARITYBIT == NONE_PARITY)
    SCON = 0x50; //8位可变波特率
#elif (PARITYBIT == ODD_PARITY) || (PARITYBIT == EVEN_PARITY) || (PARITYBIT == MARK_PARITY)
    SCON = 0xda; //9位可变波特率,校验位初始为1
#elif (PARITYBIT == SPACE_PARITY)
    SCON = 0xd2; //9位可变波特率,校验位初始为0
#endif

    T2L = (65536 - (FOSC / 4 / BAUD)); //设置波特率重装值
    T2H = (65536 - (FOSC / 4 / BAUD)) >> 8;
    AUXR = 0x14;  //T2为1T模式, 并启动定时器2
    AUXR |= 0x01; //选择定时器2为串口1的波特率发生器
    ES = 1;       //使能串口1中断
    EA = 1;
    sndper = 0;

    //SendString("Boot system...\r\n");
    //SendString("Initiate protocol...\r\n");
    initProtocal();
    Delay10ms();
    //SendString("Reload peripherals info...\r\n");
    ReloadPeripherals();
    //SendString("Check recorded peripherals...\r\n");
    //SendArray(peripherals, MAX_CHECKID);
    CheckPeripherals();
    //SendString("System ready!\r\n");
}

void CheckPeripherals()
{
    uint8 i;

    //Delay10ms();
    for (i = 2; i < MAX_CHECKID; i++) // 检查外设是否online
    {
        if (peripherals[i] < ONLINE_DIV)
        {
            dat[0] = CHECK_ONLINE;
            dat[1] = peripherals[i];
            BusSend(dat, i, 2);
            //SendByte(i);
            n = BusRecieve(dat, myAddr, 50);
            //SendByte(i);
            //SendArray(dat, n);
            if (n != 1 || dat[0] != CHECK_ONLINE)
                peripherals[i] |= ONLINE_DIV; // 如果该外设已经凉了
        }
    }
    RefreshEEPROM();
}

void ReloadPeripherals() // 从EEPROM读取储存的peripherals信息
{
    uint16 eepromAddr;

    IapDelay(5);
    for (eepromAddr = EEPROM_START_ADDR; eepromAddr < MAX_PERIPHERALS; eepromAddr++)
    {
        peripherals[eepromAddr] = IapReadByte(eepromAddr);
    }
}

void RefreshEEPROM() // 将当前的peripherals信息存入EEPROM中
{
    uint8 i;

    IapEraseSector(EEPROM_START_ADDR);
    IapDelay(10);
    for (i = 0; i < MAX_PERIPHERALS; i++)
    {
        IapProgramByte(EEPROM_START_ADDR + i, peripherals[i]);
    }
}

void MountPeripheral(uint8 label) // 为新接入的外设分配ID
{
    uint8 i;

    for (i = 2; i < MAX_PERIPHERALS; i++)
    {
        if (peripherals[i] >= ONLINE_DIV)
        {
            peripherals[i] = label;
            dat[0] = MOUNT_PERIPHERAL;
            dat[1] = i;
            if (BusSend(dat, 0x00, 2) == 1)
                RefreshEEPROM(); // 使用保留地址作广播，收到ACK则更新外设库
            else
                peripherals[i] |= ONLINE_DIV; // 没有收到ACK则unmount
            break;
        }
    }
}

void BusDecode(uint8 *dat, uint8 n)
{
    switch (dat[0])
    {
    case MOUNT_PERIPHERAL:
        MountPeripheral(dat[1]);
        break;

    default:
        break;
    }
}

//UART 中断服务程序
void Uart() interrupt 4
{
    if (RI)
    {
        RI = 0; //清除RI位
        //P0 = SBUF;              //P0显示串口数据
        //P22 = RB8;              //P2.2显示校验位
        if (SBUF == 0x30)
            sndper = 1;
    }
    if (TI)
    {
        TI = 0;   //清除TI位
        busy = 0; //清忙标志
    }
}

//发送串口数据
void SendByte(uint8 dat)
{
    while (busy)
        ;      //等待前面的数据发送完成
    ACC = dat; //获取校验位P (PSW.0)
    if (P)     //根据P来设置校验位
    {
#if (PARITYBIT == ODD_PARITY)
        TB8 = 0; //设置校验位为0
#elif (PARITYBIT == EVEN_PARITY)
        TB8 = 1; //设置校验位为1
#endif
    }
    else
    {
#if (PARITYBIT == ODD_PARITY)
        TB8 = 1; //设置校验位为1
#elif (PARITYBIT == EVEN_PARITY)
        TB8 = 0; //设置校验位为0
#endif
    }
    busy = 1;
    SBUF = ACC; //写数据到UART数据寄存器
}

//发送字符串
void SendString(char *s)
{
    while (*s) //检测字符串结束标志
    {
        SendByte(*s++); //发送当前字符
    }
}

void SendArray(uint8 *dat, uint8 n)
{
    uint8 i = 0;

    for (i = 0; i < n; i++)
        SendByte(dat[i]);
}

void Delay1000ms() //@11.0592MHz
{
    unsigned char i, j, k;

    _nop_();
    _nop_();
    i = 43;
    j = 6;
    k = 203;
    do
    {
        do
        {
            while (--k)
                ;
        } while (--j);
    } while (--i);
}

void Delay_ms(uint16 n)
{
    unsigned int i, j;
    for (j = n; j > 0; j--)
        for (i = 112; i > 0; i--)
            ;
}