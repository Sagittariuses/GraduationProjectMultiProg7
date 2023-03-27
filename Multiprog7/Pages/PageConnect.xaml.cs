using LKDSFramework;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
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

namespace Multiprog7.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageConnect.xaml
    /// </summary>
    public partial class PageConnect : Page
    {

        public event EventHandler MyEvent;

        bool CancelFlag = false;
        int Counter = 0;
        char FirstNum = ' ';

        string FlagCloud = "-cloud", FlagLU = "-lu", FlagPass = "-pass";
        List<string> ParamsList = new List<string>();
        DriverV7 Driver = new DriverV7();


        protected void OnMyEvent()
        {
            if (this.MyEvent != null)
                this.MyEvent(this, EventArgs.Empty);
        }
        public PageConnect()
        {
            
            InitializeComponent();
        }

        private void TBLU_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckDigit(e);
        }

        private void MaskTbIP_TextChanged(object sender, TextChangedEventArgs e)
        {
            var ip = MaskTbIP.Text.Split(',');

            try
            {
                foreach (var item in ip)
                    if (Convert.ToInt32(item) > 255)
                    {
                        CancelFlag = true;
                        return;
                    }
            }
            catch { }
            

        }
        private void MaskTbIP_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {   
            if(CancelFlag)
            {
                e.Handled = true;
                CancelFlag = false;
            }

            CheckDigit(e);
        }
        

        private void TBCloud_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckDigit(e);

        }

        private void TBCan_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckDigit(e);
        }

        private void BtnConnectLB_Click(object sender, RoutedEventArgs e)
        {
            // Формировать параметры подключения

            PageAddLiftBlock.LiftBlocks.Add(new string[2] { "a", "b" });
            this.OnMyEvent();
            NavigationService.GoBack();
            return;

            if (PboxFirst.Password == PboxSecond.Password)
            {
                
                try
                {
                    var Devices = DeviceV7.FromArgs(App.Args);
                    Driver.AddDevice(ref Devices[0]);

                    NavigationService.GoBack();


                }
                catch
                {
                    MessageBox.Show("Невернные параметры подключения");
                }
            }
            else
                MessageBox.Show("Пароли не совпадают", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void CheckDigit(TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }
    }
}
