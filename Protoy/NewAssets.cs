using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace Protoy
{
    // Inherit Canvas
    // Add property and method
    class NewCanvas : Canvas
    {
        public ushort Index { get; set; }  // Indicate the index of the object in its type

        // Generate deep copies
        public NewCanvas Clone()
        {
            NewCanvas temp = new NewCanvas();


            return temp;
        }
    }

    // Inherit Label
    // Add property and method
    class NewLabel : Label
    {
        public ushort Index { get; set; }  // Indicate the index of the object in its type

        // Generate deep copies
        public NewLabel Clone()
        {
            NewLabel temp = new NewLabel();

            return temp;
        }
    }
}
