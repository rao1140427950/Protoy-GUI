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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Nav_Settings_C51_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog openFileDialog = new FolderBrowserDialog();
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                GlobalVariable.C51_path = openFileDialog.SelectedPath;
                GlobalVariable.C51_diskname = GlobalVariable.C51_path.Split(':')[0];
                GlobalVariable.C51_folder = GlobalVariable.C51_path.Split(':')[1].Remove(0, 1);
                System.Windows.MessageBox.Show(GlobalVariable.C51_diskname+'\n'+GlobalVariable.C51_folder);
            }
        }
    }
}
