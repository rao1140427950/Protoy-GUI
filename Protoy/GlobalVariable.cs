using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Protoy
{
    // Global variables for USB serial port and C51 compiler configeration
    public static class Config
    {
        // C51 complier path
        public static string C51_path = @"D:\Program Files (x86)\Keil\C51\BIN";
        public static string C51_diskname = "D";
        public static string C51_folder = @"Program Files (x86)\Keil\C51\BIN";
        // Port name
        public static string Port = "COM1";
    }

    // Labels for each peripheral
    public static class Labels
    {
        public const Byte LED_Label = 0;
        public const Byte Buzz_Label = 1;
        public const Byte Servo_Label = 2;
        public const Byte DHT11_Label = 3;
        public const Byte HCSR04_Label = 4;
        public const Byte BMP180_Label = 5;
        public const Byte Motor_Label = 6;
        
    }

    // Define types and numbers of each type for drag rectangles
    public static class Define
    {
        // Max number of type for drag rectangles
        public const ushort MAX_TYPENUM = 256;

        public const ushort MAX_PERIPHERAL = 10;

        // The number of each type created
        public static ushort[] Counter = new ushort[MAX_TYPENUM];

        // The serial number of each type
        public const UInt16 Rec_Initial = 1;
        public const UInt16 Rec_Loop = 2;
        public const UInt16 Rec_Delayms = 3;
        public const UInt16 Rec_While = 4;
        public const UInt16 Rec_Assign = 5;
        public const UInt16 Rec_If = 6;
        public const UInt16 Rec_Else = 7;
        public const UInt16 Rec_End = 15;

        public const UInt16 Rec_LEDON = 8;
        public const UInt16 Rec_LEDOFF = 9;
        public const UInt16 Rec_LEDBlink = 10;

        public const UInt16 Rec_GetTemp = 11;
        public const UInt16 Rec_GetHumi = 12;

        public const UInt16 Rec_Servo = 13;

        public const UInt16 Rec_Radar = 14;

        public const UInt16 Rec_Pressure = 16;

        public const UInt16 Rec_Motor = 17;

        // The ZIndex to ensure the dragging item is on the top
        public const Int32 ZIndex_OnDragging = 255;
        // The number of anchors
        public const Int32 AnchorNum = 4;

        // Ouput folder path
        public static string Output_Path = @"E:\temp\";

        public const Double YPosOffset_OnDragging = 5;

        public const Double Disable_Opacity = 0.3;
    }

    // Bias for making anchor
    public static class Anchors
    {
        // Bias for vertical margin between two rectangle
        public const Double VerticalMargin = 2;
        public const Double HorizontalMargin = 1;
        // Bias for indent
        public const Double Indent = 30;
        // Bias for anchor radius
        public const Double AnchorRadius = 10;

        // Anchor type
        public const ushort AnchorType_RightBelow = 1;  
        public const ushort AnchorType_IndentLeft = 0;  
        public const ushort AnchorType_IndentRight = 2;  
        public const ushort AnchorType_RightBack = 3;  
        public const ushort AnchorType_FirstBlock = 4;  
    }

    // Colors
    public static class MyColors
    {
        public static SolidColorBrush Gray = new SolidColorBrush(Color.FromArgb(0xFF, 0xAB, 0xAB, 0xAB));
        public static SolidColorBrush Black = new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0x00, 0x00));
        public static SolidColorBrush MenuColor = new SolidColorBrush(Color.FromArgb(0xFF, 0xD8, 0xBF, 0xD8));
    }
}
