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

            ushort cnt = Define.Counter[this.Index]++;
            temp.Index = this.Index;
            temp.Name = this.Name + cnt;
            temp.Height = this.Height;
            temp.Width = this.Width;
            temp.Background = this.Background;
            temp.SetValue(Canvas.LeftProperty, this.GetValue(Canvas.LeftProperty));
            temp.SetValue(Canvas.TopProperty, this.GetValue(Canvas.TopProperty));
            for(int i = 0; i < this.Children.Count; i++)
            {
                if (this.Children[i] is NewLabel)
                {
                    NewLabel newLabel = (NewLabel)this.Children[i];
                    temp.Children.Add(newLabel.Clone());
                }
                else if (this.Children[i] is NewTextBox)
                {
                    NewTextBox newTextBox = (NewTextBox)this.Children[i];
                    temp.Children.Add(newTextBox.Clone());
                }
                // ===========================================
                // Add new elements here
                // ===========================================
            }
            // Write log to console
            Console.WriteLine(DateTime.Now.ToString() + " : Clone <" + this.Name + "> to <" + temp.Name + ">");
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

            temp.Content = this.Content;
            temp.FontSize = this.FontSize;
            temp.SetValue(Canvas.TopProperty, this.GetValue(Canvas.TopProperty));
            temp.SetValue(Canvas.LeftProperty, this.GetValue(Canvas.LeftProperty));
            // Write log to console
            Console.WriteLine(DateTime.Now.ToString() + @" : Clone label '" + this.Content + @"' for rectangle");
            return temp;
        }
    }

    // Inherit Label
    // Add property and method
    class NewTextBox : TextBox
    {
        public ushort Index { get; set; }  // Indicate the index of the object in its type

        // Generate deep copies
        public NewTextBox Clone()
        {
            NewTextBox temp = new NewTextBox();

            temp.Text = this.Text;
            temp.FontSize = this.FontSize;
            temp.SetValue(Canvas.TopProperty, this.GetValue(Canvas.TopProperty));
            temp.SetValue(Canvas.LeftProperty, this.GetValue(Canvas.LeftProperty));
            temp.TextWrapping = this.TextWrapping;
            // Write log to console
            Console.WriteLine(DateTime.Now.ToString() + @" : Clone TextBox '" + this.Text + @"' for rectangle");
            return temp;
        }
    }
}
