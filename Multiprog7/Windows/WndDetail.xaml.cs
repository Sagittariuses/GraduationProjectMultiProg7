using Multiprog7.Classes;
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
        public WndDetail(ObservableCollection<FirmwareAnalysis> data)
        {
            InitializeComponent();

            LvUpdateInfo.ItemsSource = data;
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
    }
}
