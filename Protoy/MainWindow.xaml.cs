using System;
using System.IO;
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
using System.Diagnostics;

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
        private Point[] Anchor = new Point[Define.AnchorNum];
        // Indent Counter
        // private UInt16 IndentCounter = 0;
        // Serial port
        private Serial serial;
        // list to record NewCanvas
        private List<NewCanvas> NewCanvasList;

        public MainWindow()
        {
            InitializeComponent();
            InitializeVariables();
        }

        private void InitializeVariables()
        {
            // Initialize anchor point
            Anchor[0] = new Point(0, 0);
            Anchor[1] = new Point(0, 0);
            Anchor[2] = new Point(0, 0);
            Anchor[3] = new Point(0, 0);
            // Initialize RecPos
            RecPos = new Point(0, 0);
            // Initial pos delta

            // Initialize serial
            serial = new Serial();

            // Initialize NewCanvasList
            NewCanvasList = new List<NewCanvas>();
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
                //System.Windows.MessageBox.Show(Config.C51_diskname+'\n'+Config.C51_folder);
            }
        }

        // Choose port for IDE
        private void Nav_Settings_Port_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as System.Windows.Controls.MenuItem;

            Config.Port = menuItem.Header.ToString();
            serial.Close();
            serial.PortName = Config.Port;
            Console.WriteLine(DateTime.Now.ToString() + " : Port <" + Config.Port + "> selected");
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
            //var grid = canvas.Parent as Border;
            RecPos.X = (Double)DraggedItem.GetValue(Canvas.LeftProperty) +
                (Double)canvas.Parent.GetValue(Canvas.LeftProperty) + (Double)Canvas1.GetValue(Canvas.LeftProperty);
            RecPos.Y = (Double)DraggedItem.GetValue(Canvas.TopProperty) +
                (Double)canvas.Parent.GetValue(Canvas.TopProperty) + (Double)Canvas1.GetValue(Canvas.TopProperty) + Define.YPosOffset_OnDragging;
            DraggedItem.SetValue(Canvas.TopProperty, RecPos.Y);
            DraggedItem.SetValue(Canvas.LeftProperty, RecPos.X);
            //Console.WriteLine("(x, y) = (" + RecPos.X + ", " + RecPos.Y + ")");
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
            else IsInArea = false;
            
            // Position is legal or not
            if (IsInArea && (IsEmpty || IsAnchored))
            {
                // Add to Canvas2
                if (IsEmpty)
                {
                    IsEmpty = false;
                    DraggedItem.AnchorPointType = Anchors.AnchorType_FirstBlock;
                }
                else
                {
                    DraggedItem.AnchorPointType = AnchorIndex;
                }
                RootCanvas.Children.Remove(DraggedItem);
                Canvas2.Children.Add(DraggedItem);
                Console.WriteLine(DateTime.Now.ToString() + " : Move <" + DraggedItem.Name + "> to <Canvas2>");

                NewCanvasList.Add(DraggedItem);
                Console.WriteLine(DateTime.Now.ToString() + " : Add <" + DraggedItem.Name + "> to List");

                // If is not anchored at the right anchor point
                if (AnchorIndex != (Define.AnchorNum - 1))
                {
                    // Update anchor point
                    Anchor[1].X = (Double)DraggedItem.GetValue(Canvas.LeftProperty) +
                        (Double)Canvas2.GetValue(Canvas.LeftProperty);
                    Anchor[1].Y = (Double)DraggedItem.GetValue(Canvas.TopProperty) +
                        (Double)Canvas2.GetValue(Canvas.TopProperty) + DraggedItem.Height + Anchors.VerticalMargin;
                    Anchor[0].X = Anchor[1].X - Anchors.Indent;
                    Anchor[0].Y = Anchor[1].Y;
                    Anchor[2].X = Anchor[1].X + Anchors.Indent;
                    Anchor[2].Y = Anchor[1].Y;
                    Anchor[3].X = (Double)DraggedItem.GetValue(Canvas.LeftProperty) +
                        (Double)Canvas2.GetValue(Canvas.LeftProperty) + DraggedItem.Width + Anchors.HorizontalMargin;
                    Anchor[3].Y = (Double)DraggedItem.GetValue(Canvas.TopProperty) +
                        (Double)Canvas2.GetValue(Canvas.TopProperty);
                    Console.WriteLine(DateTime.Now.ToString() + " : Update anchor points");
                }
                
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
                    if((Math.Pow(xPos - Anchor[i].X, 2) + Math.Pow(yPos - Anchor[i].Y, 2))
                        < Math.Pow(Anchors.AnchorRadius, 2))
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

        /*
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Serial serial = new Serial();

            serial.Open();
            serial.ReadTest();
            //string temp = serial.ReadTest();
            //Console.WriteLine(DateTime.Now.ToString() + " : " + temp);
        }
        */

        private void Btn_ScanPort_Click(object sender, RoutedEventArgs e)
        {
            string[] ports = serial.GetAvaiablePorts();

            Nav_Settings_Port.Items.Clear();
            foreach(string port in ports)
            {
                System.Windows.Controls.MenuItem menuItem = new System.Windows.Controls.MenuItem
                {
                    Header = port,
                    Background = MyColors.MenuColor
                };
                menuItem.Click += new RoutedEventHandler(Nav_Settings_Port_Click);
                Nav_Settings_Port.Items.Add(menuItem);
            }
            Console.WriteLine(DateTime.Now.ToString() + " : Update port list");
        }

        private void Btn_ClearCanvas2_Click(object sender, RoutedEventArgs e)
        {
            Canvas2.Children.Clear();
            IsEmpty = true;
            NewCanvasList.Clear();
            Anchor[0] = new Point(0, 0);
            Anchor[1] = new Point(0, 0);
            Anchor[2] = new Point(0, 0);
            Anchor[3] = new Point(0, 0);
            Console.WriteLine(DateTime.Now.ToString() + " : Clear Canvas2 and List");
        }

        private static void AddText(FileStream fs, string value)
        {
            byte[] info = new UTF8Encoding(true).GetBytes(value);
            fs.Write(info, 0, info.Length);
        }

        private void Btn_GenerateHex_Click(object sender, RoutedEventArgs e)
        {
            string path = Define.Output_Path + "main.txt";
            string prefix_path = Define.Output_Path + "prefix.txt";
            string suffix_path = Define.Output_Path + "suffix.txt";
            string output_path = Define.Output_Path + "main.c";
            string obj_path = "";
            string a, b, c;

            // Delete the file if it exists.
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            if (!File.Exists(prefix_path) || !File.Exists(suffix_path))
            {
                Console.WriteLine(DateTime.Now.ToString() + " : File not found.");
                return;
            }

            //Create the file.
            using (FileStream fs = File.Create(path))
            {
                foreach (NewCanvas temp in NewCanvasList)
                {
                    AddText(fs, temp.GetCode());
                    //Console.WriteLine(DateTime.Now.ToString() + " : " + temp.Name);
                }
            }

            a = File.ReadAllText(prefix_path);
            b = File.ReadAllText(path);
            c = File.ReadAllText(suffix_path);
            File.WriteAllText(output_path, a + b + c);

            obj_path += ("," + Define.Output_Path + "bmp180.obj");
            obj_path += ("," + Define.Output_Path + "dht11.obj");
            obj_path += ("," + Define.Output_Path + "eeprom.obj");
            obj_path += ("," + Define.Output_Path + "hcsr04.obj");
            obj_path += ("," + Define.Output_Path + "led.obj");
            obj_path += ("," + Define.Output_Path + "protocol.obj");
            obj_path += ("," + Define.Output_Path + "servo.obj");
            obj_path += ("," + Define.Output_Path + "motor.obj");


            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe"; //命令
            p.StartInfo.UseShellExecute = false; //不启用shell启动进程
            p.StartInfo.RedirectStandardInput = true; // 重定向输入
            p.StartInfo.RedirectStandardOutput = true; // 重定向标准输出
            p.StartInfo.RedirectStandardError = true; // 重定向错误输出 
            p.StartInfo.CreateNoWindow = true; // 不创建新窗口
            p.Start();

            Console.WriteLine(Config.C51_diskname);
            Console.WriteLine(Config.C51_folder);
            Console.WriteLine(Config.C51_path);
            p.StandardInput.WriteLine(Config.C51_diskname + ":");
            p.StandardInput.WriteLine("cd " + Config.C51_folder);
            p.StandardInput.WriteLine("C51.EXE " + Define.Output_Path + "main.c");
            //Console.WriteLine(p.StandardOutput.ReadToEnd());
            p.StandardInput.WriteLine("BL51.EXE " + Define.Output_Path + "main.obj" + obj_path + " TO " + Define.Output_Path + "main");
            //Console.WriteLine(p.StandardOutput.ReadToEnd());
            p.StandardInput.WriteLine("OH51.EXE " + Define.Output_Path + "main");
            //Console.WriteLine(p.StandardOutput.ReadToEnd());
            //p.StandardInput.WriteLine(command); //cmd执行的语句
            //Console.WriteLine(p.StandardOutput.ReadToEnd()); //读取命令执行信息
            p.StandardInput.WriteLine("exit"); //退出

            Console.WriteLine(DateTime.Now.ToString() + " : Source code file created");
        }

        private void Btn_ScanSerial_Click(object sender, RoutedEventArgs e)
        {
            Byte[] buffer = new byte[Define.MAX_PERIPHERAL];

            serial.Open();
            buffer[0] = 0x30;
            // Get peripheral list
            serial.WriteBytes(buffer, 1);
            serial.ReadBytes(buffer, Define.MAX_PERIPHERAL);
            
            LED_Border.Opacity = 1;
            DHT11_Border.Opacity = 1;
            Servo_Border.Opacity = 1;
            Pressure_Border.Opacity = 1;
            Motor_Border.Opacity = 1;
            
            
            /*
            LED_Border.Opacity = Define.Disable_Opacity;
            DHT11_Border.Opacity = Define.Disable_Opacity;
            Servo_Border.Opacity = Define.Disable_Opacity;
            Pressure_Border.Opacity = Define.Disable_Opacity;
            Motor_Border.Opacity = Define.Disable_Opacity;
            for(int i = 0; i < Define.MAX_PERIPHERAL; i++)
            {
                switch(buffer[i])
                {
                    case Labels.LED_Label:
                        LED_Border.Opacity = 1;
                        foreach(var newCanvas in Group_LED.Children)
                        {
                            if (newCanvas is NewCanvas)
                                ((NewCanvas)newCanvas).Device_Addr = (ushort)i;
                        }
                        break;
                    case Labels.DHT11_Label:
                        DHT11_Border.Opacity = 1;
                        foreach (var newCanvas in Group_DHT11.Children)
                        {
                            if (newCanvas is NewCanvas)
                                ((NewCanvas)newCanvas).Device_Addr = (ushort)i;
                        }
                        break;
                    case Labels.Servo_Label:
                        Servo_Border.Opacity = 1;
                        foreach (var newCanvas in Group_Servo.Children)
                        {
                            if (newCanvas is NewCanvas)
                                ((NewCanvas)newCanvas).Device_Addr = (ushort)i;
                        }
                        break;
                    case Labels.BMP180_Label:
                        Pressure_Border.Opacity = 1;
                        foreach (var newCanvas in Group_Pressure.Children)
                        {
                            if (newCanvas is NewCanvas)
                                ((NewCanvas)newCanvas).Device_Addr = (ushort)i;
                        }
                        break;
                    case Labels.Motor_Label:
                        Motor_Border.Opacity = 1;
                        foreach (var newCanvas in Group_Motor.Children)
                        {
                            if (newCanvas is NewCanvas)
                                ((NewCanvas)newCanvas).Device_Addr = (ushort)i;
                        }
                        break;
                    default:
                        break;
                    }
            }
            */
        }
    }

}

