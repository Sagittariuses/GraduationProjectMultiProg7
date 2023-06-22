using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using static LKDSFramework.VirtualDeviceV7;

namespace Multiprog7.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageChartsByType.xaml
    /// </summary>
    public partial class PageChartsByType : Page
    {

        string FillColor = "#E6E3E2";

        string RedColor = "#EA5D5F";
        string GreenColor = "#27AE60";
        string YellowColor = "#E5A119";

        List<string> ColorsHEX = new List<string>()
        {
            "#9BB1DB",
            "#1D499F",
            "#BB6BD9",
            "#2D9CDB",
            "#515151",
            "#C81862",
            "#4774CB",
        };

        public PageChartsByType()
        {
            InitializeComponent();
            ChildUniformGrids = new List<ChartsByTypes>();
            Collection = new SeriesCollection();
            var DevsByFwStatus = PageMain.OcAllDevFwData;

            if (PageCharts.currentChart == ChartsCodes.Error)
            {
                DevsByFwStatus = new ObservableCollection<FirmwareAnalysis>(DevsByFwStatus.Where(p => p.StatusCode == FirmwareErrorType.Error));
                ChartType = "Ошибки";
                LabelColor = RedColor;
            }
            else if (PageCharts.currentChart == ChartsCodes.Actual)
            {
                DevsByFwStatus = new ObservableCollection<FirmwareAnalysis>(DevsByFwStatus.Where(p => p.StatusCode == FirmwareErrorType.Actual));
                ChartType = "Актуальные\nпрошивки";
                LabelColor = GreenColor;
            }
            else if (PageCharts.currentChart == ChartsCodes.Outdate)
            {
                DevsByFwStatus = new ObservableCollection<FirmwareAnalysis>(DevsByFwStatus.Where(p => p.StatusCode == FirmwareErrorType.Outdated));
                ChartType = "Устаревшие\nпрошивки";
                LabelColor = YellowColor;
            }

            List<string> list = new List<string>();

            foreach (var item in DevsByFwStatus)
                list.Add(item.DevType.ToString());

            list = list.Distinct().ToList();

            int i = 0;

            foreach (var item in list)
            {
                var count = DevsByFwStatus.Count(p => p.DevType.ToString() == item);

                ChartsByTypes chartsByTypes = new ChartsByTypes()
                {
                    SeriesCol = new SeriesCollection
                    {
                        new PieSeries
                        {
                            Title = $"{PageCharts.currentChart}",
                            Values = new ChartValues<ObservableValue> { new ObservableValue(count) },
                            Fill = (Brush)new BrushConverter().ConvertFrom(ColorsHEX[i]),
                            Margin = new Thickness(-15 - 15 - 15 - 15),
                            StrokeThickness = 0,
                            Stroke = (Brush)new BrushConverter().ConvertFrom(ColorsHEX[i]),
                            PushOut = 0
                        },

                        new PieSeries
                        {
                            Title = "Fill",
                            Values = new ChartValues<ObservableValue> { new ObservableValue(DevsByFwStatus.Count - count) },
                            Fill = (Brush)new BrushConverter().ConvertFrom(FillColor),
                            Margin = new Thickness(-15 - 15 - 15 - 15),
                            StrokeThickness = 0,
                            Stroke = (Brush)new BrushConverter().ConvertFrom(FillColor),
                        },
                    },
                    DevType = item != "0055" ? item : "ЛБ",
                    DevCount = count,
                    DevColour = ColorsHEX[i]
                };


                PieSeries pieSeries = new PieSeries()
                {
                    Title = $"{PageCharts.currentChart}",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(count) },
                    Fill = (Brush)new BrushConverter().ConvertFrom(ColorsHEX[i]),
                    Margin = new Thickness(-15 - 15 - 15 - 15),
                    StrokeThickness = 0,
                    Stroke = (Brush)new BrushConverter().ConvertFrom(ColorsHEX[i]),
                    PushOut = 0
                };

                ChildUniformGrids.Add(chartsByTypes);
                Collection.Add(pieSeries);

                if (i == ColorsHEX.Count - 1)
                    i = 0;
                i++;
            }

            ChildUniformGrids = ChildUniformGrids.OrderByDescending(p => p.DevCount).ToList();

            Count = DevsByFwStatus.Count();
            DataContext = this;
        }

        private void BtnChartsByType_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        public List<ChartsByTypes> ChildUniformGrids { get; set; }
        public SeriesCollection Collection { get; set; }

        public string ChartType { get; set; }
        public string LabelColor { get; set; }
        public int Count { get; set; }

    }
}
