using LKDSFramework;
using Multiprog7.Classes;
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
        DriverV7Net Driver = new DriverV7Net();
        ArgsToConnect argsToConnect;
        protected void OnMyEvent()
        {
            if (this.MyEvent != null)
                this.MyEvent(this, EventArgs.Empty);
        }
        public PageConnect() => InitializeComponent();

        private void TBLU_PreviewTextInput(object sender, TextCompositionEventArgs e) => CheckDigit(e);

        private void TBCloud_PreviewTextInput(object sender, TextCompositionEventArgs e) => CheckDigit(e);

        private void TBCan_PreviewTextInput(object sender, TextCompositionEventArgs e) => CheckDigit(e);

        private void BtnConnectLB_Click(object sender, RoutedEventArgs e)
        {
            if (PboxFirst.Password == PboxSecond.Password)
                try
                {
                    argsToConnect = new ArgsToConnect(null,null,true,TBLU.Text,PboxFirst.Password, null,TBCan.Text, null);

                    for (int i = 0; i < argsToConnect.Args.Count; i++)
                        argsToConnect.Args[i] = argsToConnect.Args[i].Trim().Replace(" ", "");

                    Driver.OnDevChange += new DeviceV7.OnDevChangeDelegate(Driver_onDevChange);

                    if (!Driver.Init())
                        return;

                    var Devices = DeviceV7.FromArgs(argsToConnect.Args.ToArray());
                    Driver.AddDevice(ref Devices[0]);
                    Devices[0].WorkMode = DeviceV7.WorkModeType.LogExtra;
                }
                catch
                {
                    MessageBox.Show("Неверные параметры подключения");
                }
            else
                MessageBox.Show("Пароли не совпадают", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Driver_onDevChange(DeviceV7 dev)
        {
            LiftBlocksInfo liftBlocksInfo = new LiftBlocksInfo(dev, argsToConnect);
            
            Dispatcher.Invoke(() =>
            {
                PageAddLiftBlock.LiftBlocks.Add(liftBlocksInfo);
                this.OnMyEvent();
                try 
                {
                    NavigationService.GoBack();
                }
                catch { }
            });
            Driver.Close();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e) => NavigationService.GoBack();


        private void CheckDigit(TextCompositionEventArgs e) 
        {
            if (!Char.IsDigit(e.Text, 0))
                e.Handled = true;
        }
    }
}
