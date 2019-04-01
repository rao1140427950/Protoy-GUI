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
        public static string C51_path = "";
        public static string C51_diskname = "";
        public static string C51_folder = "";
        // Port name
        public static string Port = "COM1";
    }

    // Define types and numbers of each type for drag rectangles
    public static class Define
    {
        // Max number of type for drag rectangles
        public const ushort MAX_TYPENUM = 256;

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

        public const UInt16 Rec_LEDON = 8;
        public const UInt16 Rec_LEDOFF = 9;
        public const UInt16 Rec_LEDBlink = 10;

        public const UInt16 Rec_GetTemp = 11;
        public const UInt16 Rec_GetHumi = 12;

        public const UInt16 Rec_Servo = 13;

        public const UInt16 Rec_Radar = 14;

        // The ZIndex to ensure the dragging item is on the top
        public const Int32 ZIndex_OnDragging = 255;
        // The number of anchors
        public const Int32 AnchorNum = 4;

        // Ouput folder path
        public static string Output_Path = @"E:\Visual Studio 2017\Projects\Protoy\Protoy-GUI\Protoy\resource\";

        public const Double YPosOffset_OnDragging = 5;
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

    }

    // Colors
    public static class MyColors
    {
        public static SolidColorBrush Gray = new SolidColorBrush(Color.FromArgb(0xFF, 0xAB, 0xAB, 0xAB));
        public static SolidColorBrush Black = new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0x00, 0x00));
        public static SolidColorBrush MenuColor = new SolidColorBrush(Color.FromArgb(0xFF, 0xD8, 0xBF, 0xD8));
    }
}
