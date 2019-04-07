#include "protocol.h"
/*
12Mhz晶振
总控地址为0x01
控制信号不一定由主机产生
每次信号的格式为（八比特为一个字节）：帧头总数据字节数（含校验位）+ 目标地址 + n*数据 + 校验和
SDA和SCL两根线，平时都为高电平
SCL拉低至少5ms表示传输开始
以后SCL每拉低一次便在SDA传输1bit数据，MSB先传，SCL低电平至少保持50us
接收到数据后，若校验正确，从机拉低SDA 3~8ms，若错误则拉低 13~18ms
*/


/*
发送数据函数，数据为dat，总共为n字节，目标地址为addr
因此本次数据总共发送了n+3 个字节
*/
//sbit led = P3^6;

uint8 BusSend(uint8* dat, uint8 addr, uint8 n)
{
    uint8 i = 0;
    uint8 k = 0;
    uint8 sum = 0;
    uint8 temp = 0;

    initProtocal();
    Delay10ms();
    Delay5ms();

    SCL = 0;
    Delay10ms();  // 拉低SCL 10ms以发送开始传输信号
    SCL = 1;
    Delay1ms();

    temp = n + 3;
    sum += temp;
    for (i = 0; i <8; i++)  // 传输帧头
    {
        //SDA = temp & 0x80; // 高位先出
        if((temp & 0x80) > 0) SDA = 1;
        else SDA = 0;
        temp <<= 1;
        SCL = 0;
        Delay100us();
        SCL = 1;
        Delay50us();
    }

    temp = addr;
    sum += temp;
    for (i = 0; i <8; i++)  // 传输目的地
    {
        //SDA = temp & 0x80; // 高位先出
        if((temp & 0x80) > 0) SDA = 1;
        else SDA = 0;
        temp <<= 1;
        SCL = 0;
        Delay100us();
        SCL = 1;
        Delay50us();
    }

    for (k = 0; k < n; k++)  // 传输数据
    {
        temp = dat[k];
        sum += temp;
        for (i = 0; i <8; i++)
        {
            //SDA = temp & 0x80; // 高位先出
            if((temp & 0x80) > 0) SDA = 1;
            else SDA = 0;
            temp <<= 1;
            SCL = 0;
            Delay100us();
            SCL = 1;
            Delay50us();
        }
    }

    temp = sum;
    //sum += temp;
    for (i = 0; i <8; i++)  // 传输校验和
    {
        //SDA = temp & 0x80; // 高位先出
        if((temp & 0x80) > 0) SDA = 1;
        else SDA = 0;
        temp <<= 1;
        SCL = 0;
        Delay100us();
        SCL = 1;
        Delay50us();
    }

    initProtocal();  // 复位
    
    return recieveAck();
}

/*
接收数据函数
timeout为等待多少毫秒
dat储存接收的数据，addr为本机地址，返回接收到的字节数，如果没有收到数据则返回0
当收到广播消息，即消息地址为0x00时，将n的最高位标记为1
*/
uint8 BusRecieve(uint8* dat, uint8 addr, uint16 timeout)
{
    uint8 i = 0;
    uint16 times = 0;
    uint8 temp;
    uint8 n;
    uint8 ad;
    uint8 k;
    uint8 sum;

    times = 0;
check:
    while(SCL == 1)
    {
        if(++times > timeout) return 0;
        Delay1ms();
    }

    Delay5ms();
    if (SCL == 1) return 0;

    while(SCL == 0);  // 接收起始信号
    //LED = 1;

    n = 0;
    for(i = 0; i<8; i++)  // 接收第一字节数据
    {
        n <<= 1;
        while(SCL == 1);
        Delay50us();
        n |= SDA;
        while(SCL == 0);
    }

    ad = 0;
    for(i = 0; i<8; i++)  // 接收地址数据
    {
        ad <<= 1;
        while(SCL == 1);
        Delay50us();
        ad |= SDA;
        while(SCL == 0);
    }
    //if (ad == 0x00) led = 1;
    //LED = 1;

    for(k = 0; k < n - 3; k++)  // 接收数据
    {
        dat[k] = 0;
        for(i = 0; i<8; i++)  
        {
            dat[k] <<= 1;
            while(SCL == 1);
            Delay50us();
            dat[k] |= SDA;
            while(SCL == 0);
        }
    }
    //LED = 1;

    sum = 0;
    for(i = 0; i<8; i++)  // 接收校验和
    {
        sum <<= 1;
        while(SCL == 1);
        Delay50us();
        sum |= SDA;
        while(SCL == 0);
    }

    temp = 0;
    temp += ad;
    temp += n;
    //LED = 1;
    if (ad != addr && ad != 0x00) 
    {
        if (times < timeout) goto check;
        return 0;
    }
    for (k = 0; k < n - 3; k++) temp += dat[k];
    if(temp == sum) 
    {
        //LED = 1;
        sendAck(1);
        //LED = 1;
        //led = 0;
        if (ad == 0x00) return (n - 3)|BROADCAST;
        else return n - 3;
    }
    else
    {
        //LED = 1;
        sendAck(0);
        //LED = 1;
        return 0;
    } 
}

/*
收到数据后发送响应，k为0时发送错误响应，否则为争取
*/
void sendAck(uint8 k)
{
    Delay1ms();
    if(k == 0)
    {
        SDA = 0;
        Delay10ms();
        Delay5ms();
        SDA = 1;
    }
    else
    {
        SDA = 0;
        Delay5ms();
        SDA = 1;
    }
    initProtocal();
    //Delay1ms();
}

/*
收到正确回复则返回1，否则返回0
*/
uint8 recieveAck()
{
    uint8 flag = 0;
    uint8 i = 0;

    Delay1ms();
    while(SDA == 1)
    {
        if(++i > 10) return 0;  // 设置timeout
        Delay1ms();
    }
    Delay1ms();
    if (SDA == 0) flag = 1;
    Delay10ms();
    if (SDA == 0) flag = 0;
    return flag;
}

void initProtocal()
{
    SCL = 1;
    SDA = 1;
    //LED = 0;
    //led = 1;
    Delay1ms();
}

void Delay10ms()		//@12.000MHz
{
	unsigned char i, j;

	i = 117;
	j = 184;
	do
	{
		while (--j);
	} while (--i);
}

void Delay5ms()		//@12.000MHz
{
	unsigned char i, j;

	i = 59;
	j = 90;
	do
	{
		while (--j);
	} while (--i);
}

void Delay1ms()		//@12.000MHz
{
	unsigned char i, j;

	i = 12;
	j = 169;
	do
	{
		while (--j);
	} while (--i);
}

void Delay100us()		//@12.000MHz
{
	unsigned char i, j;

	i = 2;
	j = 39;
	do
	{
		while (--j);
	} while (--i);
}

void Delay50us()		//@12.000MHz
{
	unsigned char i, j;

	i = 1;
	j = 146;
	do
	{
		while (--j);
	} while (--i);
}
