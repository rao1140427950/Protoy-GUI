using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.RegularExpressions;

namespace Protoy
{
    // Inherit Canvas
    // Add property and method
    class NewCanvas : Canvas
    {
        public ushort Index { get; set; }  // Indicate the index of the object in its type
        public SolidColorBrush ThemeColor { get; set; }  // Theme coler, for backgroud and text
        public int AnchorPointType { get; set; }  // Anchor point type

        public string Code_Prefix { get; set; }
        public string Code_Suffix { get; set; }
        public ushort Device_Addr { get; set; }

        public NewCanvas()
        {
            Device_Addr = 0;
        }

        public string GetCode()
        {
            NewTextBox newTextBox = new NewTextBox();
            string Code = "";
            //newTextBox.Text = "";

            for (int i = 0; i < this.Children.Count; i++)
            {
                if (this.Children[i] is NewTextBox)
                {
                    newTextBox = (NewTextBox)this.Children[i];
                    break;
                }
            }

            if (Device_Addr > 0)
            {
                if (newTextBox.TextAvailable())
                    Code = Code_Prefix + " " + Device_Addr.ToString() + "," + newTextBox.Text + " " + Code_Suffix + "\r\n";
                else
                    Code = Code_Prefix + " " + Device_Addr.ToString() + " " + Code_Suffix + "\r\n";
            }
            else
            {
                if (newTextBox.TextAvailable())
                    Code = Code_Prefix + " " + newTextBox.Text + " " + Code_Suffix + "\r\n";
                else
                    Code = Code_Prefix + " " + Code_Suffix + "\r\n";
            }

            if (AnchorPointType == Anchors.AnchorType_IndentRight)
                Code = "{\r\n" + Code;
            else if (AnchorPointType == Anchors.AnchorType_IndentLeft)
                Code = "\r\n}" + Code;

            return Code;
        }
            

        // Generate deep copies
        public NewCanvas Clone()
        {
            NewCanvas temp = new NewCanvas();

            ushort cnt = Define.Counter[this.Index]++;
            temp.Index = this.Index;
            //temp.AnchorPointType = this.AnchorPointType;
            temp.Name = this.Name + cnt;
            temp.Height = this.Height;
            temp.Width = this.Width;
            temp.Cursor = this.Cursor;
            temp.ThemeColor = this.ThemeColor;
            temp.Code_Prefix = this.Code_Prefix;
            temp.Code_Suffix = this.Code_Suffix;
            temp.Device_Addr = this.Device_Addr;
            //temp.Background = this.Background;
            temp.SetValue(Canvas.LeftProperty, this.GetValue(Canvas.LeftProperty));
            temp.SetValue(Canvas.TopProperty, this.GetValue(Canvas.TopProperty));
            temp.SetValue(Canvas.ZIndexProperty, Define.ZIndex_OnDragging);
            for(int i = 0; i < this.Children.Count; i++)
            {
                if (this.Children[i] is NewLabel newLabel)
                    temp.Children.Add(newLabel.Clone());
                else if (this.Children[i] is NewTextBox newTextBox)
                    temp.Children.Add(newTextBox.Clone());
                else if (this.Children[i] is NewTextBlock newTextBlock)
                    temp.Children.Add(newTextBlock.Clone());
                else if (this.Children[i] is NewBorder newBorder)
                    temp.Children.Add(newBorder.Clone());
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
        // Generate deep copies
        public NewLabel Clone()
        {
            NewLabel temp = new NewLabel();

            temp.Content = this.Content;
            temp.FontSize = this.FontSize;
            temp.SetValue(Canvas.TopProperty, this.GetValue(Canvas.TopProperty));
            temp.SetValue(Canvas.LeftProperty, this.GetValue(Canvas.LeftProperty));
            temp.Height = this.Height;
            temp.MaxHeight = this.MaxHeight;
            temp.IsHitTestVisible = this.IsHitTestVisible;
            temp.Foreground = this.Foreground;
            // ===========================================
            // Add new elements here
            // ===========================================
            // Write log to console
            Console.WriteLine(DateTime.Now.ToString() + @" : Clone label " + this.Content + @" for rectangle");
            return temp;
        }
    }

    // Inherit Border
    // Add property and method
    class NewBorder : Border
    {
        // Generate deep copies
        public NewBorder Clone()
        {
            NewBorder temp = new NewBorder();

            temp.Style = this.Style;
            temp.Background = this.Background;

            //temp.Content = this.Content;
            //temp.FontSize = this.FontSize;
            //temp.SetValue(Canvas.TopProperty, this.GetValue(Canvas.TopProperty));
            //temp.SetValue(Canvas.LeftProperty, this.GetValue(Canvas.LeftProperty));
            //temp.Height = this.Height;
            //temp.MaxHeight = this.MaxHeight;
            //// ===========================================
            //// Add new elements here
            //// ===========================================
            //// Write log to console
            Console.WriteLine(DateTime.Now.ToString() + @" : Clone Border " + @" for rectangle");
            return temp;
        }
    }

    // Inherit TextBlock
    // Add property and method
    class NewTextBlock : TextBlock
    {
        // Generate deep copies
        public NewTextBlock Clone()
        {
            NewTextBlock temp = new NewTextBlock();

            temp.Style = this.Style;
            temp.IsHitTestVisible = this.IsHitTestVisible;
            temp.TextAlignment = this.TextAlignment;
            temp.FontSize = this.FontSize;
            temp.Foreground = this.Foreground;
            temp.SetValue(Canvas.TopProperty, this.GetValue(Canvas.TopProperty));
            temp.SetValue(Canvas.LeftProperty, this.GetValue(Canvas.LeftProperty));
            temp.Text = this.Text;

            //temp.Content = this.Content;
            //temp.FontSize = this.FontSize;
            //temp.SetValue(Canvas.TopProperty, this.GetValue(Canvas.TopProperty));
            //temp.SetValue(Canvas.LeftProperty, this.GetValue(Canvas.LeftProperty));
            //temp.Height = this.Height;
            //temp.MaxHeight = this.MaxHeight;
            //// ===========================================
            //// Add new elements here
            //// ===========================================
            //// Write log to console
            Console.WriteLine(DateTime.Now.ToString() + @" : Clone TextBlock '" + @"' for rectangle");
            return temp;
        }
    }

    // Inherit TextBox
    // Add property and method
    class NewTextBox : TextBox
    {
        private bool IsDefaultText;  // Whether the content text has been editted
        public string RegexString { get; set; }  // Define regular expression string
        public string DefaultText { get; set; }  // Default text of the TextBox

        public NewTextBox()
        {
            // Add individual event handler
            GotFocus += new RoutedEventHandler(NewTextBox_GotFocus);
            LostFocus += new RoutedEventHandler(NewTextBox_LostFocus);
            KeyDown += new KeyEventHandler(NewTextBox_KeyDown);
            // At first the text is default
            IsDefaultText = true;
            Foreground = MyColors.Gray;
            //Text = DefaultText;
        }

        public bool TextAvailable()
        {
            return !IsDefaultText;
        }

        // Generate deep copies
        public NewTextBox Clone()
        {
            NewTextBox temp = new NewTextBox();

            // Clone properties
            temp.Text = this.Text;
            temp.FontSize = this.FontSize;
            temp.SetValue(Canvas.TopProperty, this.GetValue(Canvas.TopProperty));
            temp.SetValue(Canvas.LeftProperty, this.GetValue(Canvas.LeftProperty));
            temp.TextWrapping = this.TextWrapping;
            temp.MinWidth = this.MinWidth;
            temp.MaxWidth = this.MaxWidth;
            temp.Height = this.Height;
            temp.MaxHeight = this.MaxHeight;
            temp.RegexString = this.RegexString;
            temp.DefaultText = this.DefaultText;
            temp.BorderBrush = this.BorderBrush;
            //temp.Width = this.Width;
            // ===========================================
            // Add new elements here
            // ===========================================
            // Write log to console
            Console.WriteLine(DateTime.Now.ToString() + @" : Clone TextBox '" + this.Text + @"' for rectangle");
            return temp;
        }

        private void NewTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            // Console.WriteLine(DateTime.Now.ToString() + @" : TextBox get focus");
            // Check whether is default text

            if (IsDefaultText)
            {
                // Clean default text
                Text = "";
                // Change text color to black
                Foreground = ((NewCanvas)this.Parent).ThemeColor;
            }

            // Update log
            Console.WriteLine(DateTime.Now.ToString() + @" : Edit TextBox in <" + 
                ((NewCanvas)this.Parent).Name + ">");
        }

        private void NewTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Regex regex = new Regex(RegexString);
            if(!regex.IsMatch(Text))
            {
                // Reset to default
                Text = DefaultText;
                Foreground = MyColors.Gray;
                IsDefaultText = true;
                Console.WriteLine(DateTime.Now.ToString() + @" : Illegal input in <" +
                    ((NewCanvas)this.Parent).Name + ">. Set Text to default");
            }
            else
            {
                IsDefaultText = false;
                // Update log
                Console.WriteLine(DateTime.Now.ToString() + @" : Finish editting TextBox in <" +
                    ((NewCanvas)this.Parent).Name + ">");
            }
        }

        private void NewTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox textBox = new TextBox{
                    Width = 0
                };
                ((NewCanvas)this.Parent).Children.Add(textBox);
                textBox.Focus();
                textBox = null;
            }
        }

    }
}
