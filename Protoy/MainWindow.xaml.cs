using System;
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
        private NewCanvas DraggedItem;
        private bool IsDragging = false;
        private Point pos;

        public MainWindow()
        {
            InitializeComponent();
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
            Console.WriteLine(DraggedItem.Parent);
            Group_Basic.Children.Add(DraggedItem);
            // drag setup
            pos = e.GetPosition(null);
            IsDragging = true;
        }

        private void Rec_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            IsDragging = false;
        }

        private void Rec_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if(IsDragging)
            {
                double xPos = e.GetPosition(null).X - pos.X + (double)DraggedItem.GetValue(Canvas.LeftProperty);
                double yPos = e.GetPosition(null).Y - pos.Y + (double)DraggedItem.GetValue(Canvas.TopProperty);
                DraggedItem.SetValue(Canvas.LeftProperty, xPos);
                DraggedItem.SetValue(Canvas.TopProperty, yPos);

                pos = e.GetPosition(null);
            }
        }

    }

}

