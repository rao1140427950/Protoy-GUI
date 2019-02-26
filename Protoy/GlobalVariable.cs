using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protoy
{
    // Global variables for USB serial port and C51 compiler configeration
    public static class Config
    {
        public static string C51_path = "";
        public static string C51_diskname = "";
        public static string C51_folder = "";
        public static string port = "";
    }

    // Define types and numbers of each type for drag rectangles
    public static class Define
    {
        public const ushort MAX_TYPENUM = 256;

        public static ushort[] Counter = new ushort[MAX_TYPENUM];

        public const ushort Rec_Initial = 1;
        public const ushort Rec_Loop = 2;
        public const ushort Rec_While = 3;
        public const ushort Rec_Delayms = 4;
    }
}
