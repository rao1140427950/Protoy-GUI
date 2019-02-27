﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Protoy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Record dragged item
        private NewCanvas DraggedItem;
        // Whether dragging is started 
        private bool IsDragging = false;
        // Whether Canvas2 is empty
        private bool IsEmpty = true;
        // Whether the item is anchored
        private bool IsAnchored = false;
        // Whether is in Canvas2
        private bool IsInArea = false;
        // Which anchor
        private int AnchorIndex = -1;
        // Variable for dragging
        private Point MousePos;
        private Point RecPos;
        // PosDelta = RecPos - MousePos
        private Point PosDelta;
        // Anchor points
        private Point[] Anchor = new Point[3];
        // Indent Counter
        // private UInt16 IndentCounter = 0;

        public MainWindow()
        {
            InitializeComponent();
            InitializeVariables();
        }

        private void InitializeVariables()
        {
            // Initial anchor point
            Anchor[0] = new Point(0, 0);
            Anchor[1] = new Point(0, 0);
            Anchor[2] = new Point(0, 0);
            // Initial RecPos
            RecPos = new Point(0, 0);
            // Initial pos delta
        }

        // Choose path for C51 compiler 
        private void Nav_Settings_C51_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog openFileDialog = new FolderBrowserDialog();
            // Find and split C51 compiler path
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Config.C51_path = openFileDialog.SelectedPath;
                Config.C51_diskname = Config.C51_path.Split(':')[0];
                Config.C51_folder = Config.C51_path.Split(':')[1].Remove(0, 1);
                System.Windows.MessageBox.Show(Config.C51_diskname+'\n'+Config.C51_folder);
            }
        }

        private void Rec_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            NewCanvas temp = sender as NewCanvas;
            // Clone selected item
            DraggedItem = temp.Clone();
            // Add event handler
            DraggedItem.MouseLeftButtonUp += new MouseButtonEventHandler(Rec_MouseLeftButtonUp);
            DraggedItem.MouseMove += new System.Windows.Input.MouseEventHandler(Rec_MouseMove);
            // Add to parent element
            //Console.WriteLine(DraggedItem.Parent);

            // Move clone to RootCanvas
            // Group_Basic.Children.Add(DraggedItem);
            var canvas = temp.Parent as Canvas;
            RecPos.X = (Double)DraggedItem.GetValue(Canvas.LeftProperty) +
                (Double)canvas.Parent.GetValue(Canvas.LeftProperty) + (Double)Canvas1.GetValue(Canvas.LeftProperty);
            RecPos.Y = (Double)DraggedItem.GetValue(Canvas.TopProperty) +
                (Double)canvas.Parent.GetValue(Canvas.TopProperty) + (Double)Canvas1.GetValue(Canvas.TopProperty);
            DraggedItem.SetValue(Canvas.TopProperty, RecPos.Y);
            DraggedItem.SetValue(Canvas.LeftProperty, RecPos.X);
            RootCanvas.Children.Add(DraggedItem);
            // drag setup
            MousePos = e.GetPosition(null);
            PosDelta.X = RecPos.X - MousePos.X;
            PosDelta.Y = RecPos.Y - MousePos.Y;
            IsDragging = true;
            
            // Update log
            Console.WriteLine(DateTime.Now.ToString() + " : Start dragging <" + DraggedItem.Name + ">");
        }

        private void Rec_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!IsDragging) return;

            // Get the position on Canvas2
            DraggedItem.SetValue(Canvas.LeftProperty, (Double)DraggedItem.GetValue(Canvas.LeftProperty) -
                (Double)Canvas2.GetValue(Canvas.LeftProperty));
            DraggedItem.SetValue(Canvas.TopProperty, (Double)DraggedItem.GetValue(Canvas.TopProperty) -
                (Double)Canvas2.GetValue(Canvas.TopProperty));

            // Item is on Canvas2 or not
            if ((Double)DraggedItem.GetValue(Canvas.LeftProperty) > 0 && 
                (Double)DraggedItem.GetValue(Canvas.TopProperty) > 0)
                IsInArea = true;
            
            // Position is legal or not
            if (IsInArea && (IsEmpty || IsAnchored))
            {
                // Add to Canvas2
                if (IsEmpty) IsEmpty = false;
                RootCanvas.Children.Remove(DraggedItem);
                Canvas2.Children.Add(DraggedItem);

                Console.WriteLine(DateTime.Now.ToString() + " : Move <" + DraggedItem.Name + "> to <Canvas2>");

                // Update anchor point
                Anchor[1].X = (Double)DraggedItem.GetValue(Canvas.LeftProperty) + 
                    (Double)Canvas2.GetValue(Canvas.LeftProperty);
                Anchor[1].Y = (Double)DraggedItem.GetValue(Canvas.TopProperty) +
                    (Double)Canvas2.GetValue(Canvas.TopProperty) + DraggedItem.Height + Bias.VerticalMargin;
                Anchor[0].X = Anchor[1].X - Bias.Indent;
                Anchor[0].Y = Anchor[1].Y;
                Anchor[2].X = Anchor[1].X + Bias.Indent;
                Anchor[2].Y = Anchor[1].Y;
                Console.WriteLine(DateTime.Now.ToString() + " : Update anchor points");
            }
            else
            {
                // Delete illegal item
                Console.WriteLine(DateTime.Now.ToString() + " : Illegal placement. Delete <" + DraggedItem.Name + ">");
                RootCanvas.Children.Remove(DraggedItem);
                DraggedItem = null;
            }

            IsDragging = false;

            // Update log
            Console.WriteLine(DateTime.Now.ToString() + " : Finish dragging");
        }

        private void Rec_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if(IsDragging)
            {
                // Change position
                // double xPos = e.GetPosition(null).X - pos.X + (double)DraggedItem.GetValue(Canvas.LeftProperty);
                // double yPos = e.GetPosition(null).Y - pos.Y + (double)DraggedItem.GetValue(Canvas.TopProperty);
                // double xPos = e.GetPosition(null).X - MousePos.X + RecPos.X;
                // double yPos = e.GetPosition(null).Y - MousePos.Y + RecPos.Y;
                double xPos = e.GetPosition(null).X + PosDelta.X;
                double yPos = e.GetPosition(null).Y + PosDelta.Y;

                // Match anchor point
                IsAnchored = false;
                for (int i = 0; i < Define.AnchorNum; i++)
                {
                    if((Math.Pow(xPos - Anchor[i].X,2)+Math.Pow(yPos-Anchor[i].Y,2))<Math.Pow(Bias.AnchorRadius,2))
                    {
                        IsAnchored = true;
                        AnchorIndex = i;
                        break;
                    }
                }

                // Set new position
                if (IsAnchored)
                {
                    DraggedItem.SetValue(Canvas.LeftProperty, Anchor[AnchorIndex].X);
                    DraggedItem.SetValue(Canvas.TopProperty, Anchor[AnchorIndex].Y);
                }
                else
                {
                    DraggedItem.SetValue(Canvas.LeftProperty, xPos);
                    DraggedItem.SetValue(Canvas.TopProperty, yPos);
                }

                //pos = e.GetPosition(null);
            }
        }

    }

}

