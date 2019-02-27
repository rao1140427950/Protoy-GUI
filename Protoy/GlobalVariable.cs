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
        public static string port = "";
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
        public const UInt16 Rec_While = 3;
        public const UInt16 Rec_Delayms = 4;

        // The ZIndex to ensure the dragging item is on the top
        public const Int32 ZIndex_OnDragging = 1;
        // The number of anchors
        public const Int32 AnchorNum = 3;
    }

    // Bias for making anchor
    public static class Bias
    {
        // Bias for vertical margin between two rectangle
        public const Double VerticalMargin = 5;
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
    }
}
