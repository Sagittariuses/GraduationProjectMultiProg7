using Multiprog7.Pages;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Multiprog7.Windows
{
    /// <summary>
    /// Логика взаимодействия для WndOfflineMode.xaml
    /// </summary>
    public partial class WndOfflineMode : Window
    {
        public WndOfflineMode()
        {
            InitializeComponent();
        }

        private void BtnMinimezeBox_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void BtnCloseBox_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnUpload_Click(object sender, RoutedEventArgs e)
        {
            // Разархивировать их куда-то, хз куда
        }

        private void BtnOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog opd = new OpenFileDialog();
            opd.Filter = $"Файлы прошивки ({PageMain.FileExt}) | {PageMain.FileExt}";
            opd.Title = "Выбор прошивки";

            Nullable<bool> result = opd.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                LbArchivePath.Content = opd.FileName;
                PageMain.FwArchivePath= opd.FileName;




            }
        }
    }
}
