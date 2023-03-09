using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Multiprog7
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string LKDSFrameworkName = "LKDSFramework.dll";
        bool SimpleFlag = false;
        public MainWindow()
        {
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + LKDSFrameworkName))
            {
                MessageBox.Show($"Не найдена библиотека {LKDSFrameworkName}. \nПоместите {LKDSFrameworkName} рядом с исполняемым файлом и повторите попытку.");
                Process.GetCurrentProcess().Kill();
            }
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.Args.Length != 0)
            {
                for (int i = 1; i < App.Args.Length; i++)
                {
                    if (App.Args[i].Contains("-can"))
                    {
                        try
                        {
                            int CheckCanID = Convert.ToInt32(App.Args[i].Substring(App.Args[i].Length - 1));
                        }
                        catch
                        {
                            break;
                        }


                        //FrameMain.Content = new Pages.PageMain();
                        SimpleFlag = true;
                        break;
                    }
                }
                if (!SimpleFlag)
                {
                    FrameMain.Content = new Pages.PageMain();
                }
            }
            else
            {
                FrameMain.Content = new Pages.PageConnect();
            }
        }

        private void BtnMinimezeBox_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void BtnCloseBox_Click(object sender, RoutedEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }
    }
}