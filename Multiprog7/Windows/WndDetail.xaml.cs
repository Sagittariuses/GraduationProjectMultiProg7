using LKDSFramework;
using Multiprog7.Classes;
using Multiprog7.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для WndDetail.xaml
    /// </summary>
    public partial class WndDetail : Window
    {
        public WndDetail()
        {
            InitializeComponent();
        }
        private void dragMe(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch { }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


        public void UpdateData(ref ObservableCollection<FirmwareAnalysis> data)
        {
            var SelectedUnitID = data[0].Unit;

            DeviceV7 dev = PageMain.LocalDeviceV7s.FirstOrDefault(p => (p as DeviceV7).UnitID == SelectedUnitID) as DeviceV7;

            LbLiftTitle.Content = $"{dev} {dev.IP} / {dev.UnitID}";
            LvAnalysisInfo.ItemsSource = data;
        }
    }
}
