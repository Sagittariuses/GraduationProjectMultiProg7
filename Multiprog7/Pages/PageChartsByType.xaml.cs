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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Multiprog7.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageChartsByType.xaml
    /// </summary>
    public partial class PageChartsByType : Page
    {
        public PageChartsByType()
        {
            InitializeComponent();
        }

        private void BtnChartsByType_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
