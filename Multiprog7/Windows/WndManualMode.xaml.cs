using Multiprog7.Model;
using Multiprog7.Pages;
using LKDSFramework.Packs.DataDirect.IAPService;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
    /// Логика взаимодействия для WndManualMode.xaml
    /// </summary>
    public partial class WndManualMode : Window
    {

        private static int HeightFirst = 201;
        private static int HeightSecond = 273;
        public static bool IsApplied = false;

        #region Wnd events
        public WndManualMode()
        {
            InitializeComponent();
            Height = HeightFirst;
            GridChooseFwFirst.Visibility = Visibility.Visible;
            GridChooseFwSecond.Visibility = Visibility.Visible;
            GridChooseFwThird.Visibility = Visibility.Visible;
            GridFwFirst.Visibility = Visibility.Hidden;
            GridFwSecond.Visibility = Visibility.Hidden;
            GridFwThird.Visibility = Visibility.Hidden;
        }

        private void BtnMinimezeBox_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void BtnCloseBox_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion

         #region First FW events
        private void BtnChooseFwFilePlusFirst_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog opd = new OpenFileDialog();
            //opd.Filter = $"Файлы прошивки ({PageMain.FileExt}) | {PageMain.FileExt}";
            opd.Title = "Выбор прошивки";

            Nullable<bool> result = opd.ShowDialog();


            if (result == true)
            {

                string FWVer = "";
                char[] FWName = opd.SafeFileName.Substring(0, opd.SafeFileName.Length-4).ToCharArray();
                for (int i = FWName.Length - 1; i > 0; i--)
                {
                    if (Char.IsDigit(FWName[i]) || FWName[i].Equals('0'))
                    {
                        FWVer += FWName[i];
                    }
                    else
                    {
                        try
                        {
                            if (Convert.ToInt32(FWName[i]).Equals(32))
                            {
                                continue;
                            }
                            else
                            {
                                break;
                            }
                        }
                        catch
                        {
                            break;
                        }
                    }
                }
                FWName = FWVer.Reverse().ToArray();
                FWVer = "";
                foreach (char ch in FWName)
                {
                    FWVer += ch + ".";

                }
                try
                {
                    FWVer = FWVer.Substring(0, FWVer.Length - 1).Trim();
                    Height = HeightSecond;
                    GridChooseFwFirst.Visibility = Visibility.Hidden;
                    GridFwFirst.Visibility = Visibility.Visible;
                    LbFwFilenameFirst.Content = opd.SafeFileName;
                    LbFwVerFirst.Content = FWVer;
                    LbFwDateFirst.Content = File.GetCreationTime(opd.FileName).ToShortDateString();
                    PageMain.FwFromManualMode = new Multiprog7.Classes.FWForDevice(Convert.ToByte(FWVer.Replace(".", "")), null, opd.FileName, opd.SafeFileName);
                }
                catch { }
               
            }
        }

        private void BtnClearFirmwareFirst_Click(object sender, RoutedEventArgs e)
        {
            PageMain.FwFromManualMode = null;
            GridChooseFwFirst.Visibility = Visibility.Visible;
            GridFwFirst.Visibility = Visibility.Hidden;
            ChangeHeight();
        }
        #endregion

        #region Second FW events
        private void BtnChooseFwFilePlusSecond_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog opd = new OpenFileDialog();
            //opd.Filter = PageMain.FileExt;

            Nullable<bool> result = opd.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                //PageMain.FileFW = opd.FileName;

                string FWVer = "";
                char[] FWName = opd.SafeFileName.ToCharArray();
                for (int i = FWName.Length - 1; i > 0; i--)
                {
                    if (Char.IsDigit(FWName[i]) || FWName[i].Equals('0'))
                    {
                        FWVer += FWName[i];
                    }
                    else
                    {
                        try
                        {
                            int a = (int)FWName[i];
                            if (a.Equals(32))
                            {
                                continue;
                            }
                            else
                            {
                                break;
                            }
                        }
                        catch
                        {
                            break;
                        }
                    }
                }
                FWName = FWVer.Reverse().ToArray();
                FWVer = "";

                foreach (char ch in FWName)
                {
                    FWVer += ch + ".";
                }
                FWVer = FWVer.Trim();

                GridChooseFwSecond.Visibility = Visibility.Hidden;
                GridFwSecond.Visibility = Visibility.Visible;
                LbFwFilenameSecond.Content = opd.SafeFileName;
                LbFwVerSecond.Content = FWVer;
                LbFwDateSecond.Content = File.GetCreationTime(opd.FileName);


            }
        }

        private void BtnClearFirmwareSecond_Click(object sender, RoutedEventArgs e)
        {
            PageMain.FileFW = null;
            GridChooseFwSecond.Visibility = Visibility.Visible;
            GridFwSecond.Visibility = Visibility.Hidden;
            ChangeHeight();
        }
        #endregion

        #region Third FW events
        private void BtnChooseFwFilePlusThird_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog opd = new OpenFileDialog();
            //opd.Filter = $"Файлы прошивки ({PageMain.FileExt}) | {PageMain.FileExt}";
            opd.Title = "Выбор прошивки";

            Nullable<bool> result = opd.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                //PageMain.FileFW = opd.FileName;

                string FWVer = "";
                char[] FWName = opd.SafeFileName.ToCharArray();
                for (int i = FWName.Length - 1; i > 0; i--)
                {
                    if (Char.IsDigit(FWName[i]) || FWName[i].Equals('0'))
                    {
                        FWVer += FWName[i];
                    }
                    else
                    {
                        try
                        {
                            int a = (int)FWName[i];
                            if (a.Equals(32))
                            {
                                continue;
                            }
                            else
                            {
                                break;
                            }
                        }
                        catch
                        {
                            break;
                        }
                    }
                }
                FWName = FWVer.Reverse().ToArray();
                FWVer = "";

                foreach (char ch in FWName)
                {
                    FWVer += ch + ".";
                }
                FWVer = FWVer.Trim();

                Height = HeightSecond;
                GridChooseFwThird.Visibility = Visibility.Hidden;
                GridFwThird.Visibility = Visibility.Visible;
                LbFwFilenameThird.Content = opd.SafeFileName.Substring(0,opd.SafeFileName.Length-4);
                LbFwVerThird.Content = FWVer;
                LbFwDateThird.Content = File.GetCreationTime(opd.FileName);


            }
        }

        private void BtnClearFirmwareThird_Click(object sender, RoutedEventArgs e)
        {
            PageMain.FileFW = null;
            GridChooseFwThird.Visibility = Visibility.Visible;
            GridFwThird.Visibility = Visibility.Hidden;
            ChangeHeight();
        }

        #endregion

        private void BtnApply_Click(object sender, RoutedEventArgs e)
        {
            IsApplied = true;
            Close();
        }


        void ChangeHeight()
        {
            if (GridChooseFwFirst.Visibility == Visibility.Visible
                && GridChooseFwSecond.Visibility == Visibility.Visible
                && GridChooseFwThird.Visibility == Visibility.Visible)
            {
                Height = HeightFirst;
            }
        }

        private void dragMe(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch { }
        }
    }
}
