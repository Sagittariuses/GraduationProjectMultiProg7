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

namespace Multiprog7.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageCharts.xaml
    /// </summary>
    public partial class PageCharts : Page
    {
        List<string> ColorsHEX = new List<string>()
        {
            "#EA5D5F",
            "#E5A119",
            "#27AE60",
            "#E6E3E2",

            //errors hex
            "#9BB1DB",
            "#1D499F",
            "#BB6BD9",
            "#2D9CDB",
            "#515151",
            "#C81862",
            "#4774CB",
        };
        public PageMain pageMain;
        public static ChartsCodes currentChart;

        int Errors, Outdates, Actuals;

        public PageCharts(ObservableCollection<FirmwareAnalysis> data)
        {
            InitializeComponent();
            Errors = data.Count(p => p.StatusCode == FirmwareStatus.Error);
            Outdates = data.Count(p => p.StatusCode == FirmwareStatus.Outdated);
            Actuals = data.Count(p => p.StatusCode == FirmwareStatus.Actual);

            FullCollection = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "Errors",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(Errors) },
                    Fill = (Brush) new BrushConverter().ConvertFrom(ColorsHEX[0]),
                    Margin = new Thickness(-15 - 15 -15 -15),
                    StrokeThickness = 0,
                    Stroke = (Brush)new BrushConverter().ConvertFrom(ColorsHEX[0]),
                },

                new PieSeries
                {
                    Title = "OutdatedFw ",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(Outdates) },
                    Fill = (Brush) new BrushConverter().ConvertFrom(ColorsHEX[1]),
                    Margin = new Thickness(-15 - 15 -15 -15),
                    StrokeThickness = 0,
                    Stroke = (Brush)new BrushConverter().ConvertFrom(ColorsHEX[1]),
                },
                new PieSeries
                {
                    Title = "ActualFw",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(Actuals) },
                    Fill = (Brush) new BrushConverter().ConvertFrom(ColorsHEX[2]),
                    Margin = new Thickness(-15 - 15 -15 -15),
                    StrokeThickness = 0,
                    Stroke = (Brush)new BrushConverter().ConvertFrom(ColorsHEX[2]),
                },
                
            };

            ErrorsCollection = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "Errors",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(Errors) },
                    Fill = (Brush) new BrushConverter().ConvertFrom(ColorsHEX[0]),
                    Margin = new Thickness(-15 - 15 -15 -15),
                    StrokeThickness = 0,
                    Stroke = (Brush)new BrushConverter().ConvertFrom(ColorsHEX[0]),
                },

                new PieSeries
                {
                    Title = "Fill ",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(data.Count()) },
                    Fill = (Brush) new BrushConverter().ConvertFrom(ColorsHEX[3]),
                    Margin = new Thickness(-15 - 15 -15 -15),
                    StrokeThickness = 0,
                    Stroke = (Brush)new BrushConverter().ConvertFrom(ColorsHEX[3]),
                },

            };
            OutdatedFwCollection = new SeriesCollection
            {
             

                new PieSeries
                {
                    Title = "OutdatedFw ",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(Outdates) },
                    Fill = (Brush) new BrushConverter().ConvertFrom(ColorsHEX[1]),
                    Margin = new Thickness(-15 - 15 -15 -15),
                    StrokeThickness = 0,
                    Stroke = (Brush)new BrushConverter().ConvertFrom(ColorsHEX[1]),
                },

                new PieSeries
                {
                    Title = "Fill ",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(data.Count()) },
                    Fill = (Brush) new BrushConverter().ConvertFrom(ColorsHEX[3]),
                    Margin = new Thickness(-15 - 15 -15 -15),
                    StrokeThickness = 0,
                    Stroke = (Brush)new BrushConverter().ConvertFrom(ColorsHEX[3]),
                },
            };
            ActualFwCollection = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "ActualFw",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(Actuals) },
                    Fill = (Brush)new BrushConverter().ConvertFrom(ColorsHEX[2]),
                    Margin = new Thickness(-15 - 15 - 15 - 15),
                    StrokeThickness = 0,
                    Stroke = (Brush)new BrushConverter().ConvertFrom(ColorsHEX[2]),
                },

                new PieSeries
                {
                    Title = "Fill ",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(data.Count()) },
                    Fill = (Brush) new BrushConverter().ConvertFrom(ColorsHEX[3]),
                    Margin = new Thickness(-15 - 15 -15 -15),
                    StrokeThickness = 0,
                    Stroke = (Brush)new BrushConverter().ConvertFrom(ColorsHEX[3]),
                },
            };
            UpdateData();
        }

        public SeriesCollection FullCollection { get; set; }
        public SeriesCollection ErrorsCollection { get; set; }
        public SeriesCollection OutdatedFwCollection { get; set; }
        public SeriesCollection ActualFwCollection { get; set; }

        private void BtnChartByType_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PageChartsByType());
        }

      

        private void BtnFullChart_Click(object sender, RoutedEventArgs e)
        {
            currentChart = ChartsCodes.Full;
            pageMain.UpdateAnalysisData(PageMain.currentFilt, currentChart);
            ChangeCurrentChart(currentChart);
        }

        private void BtnErrorsChart_Click(object sender, RoutedEventArgs e)
        {
            currentChart = ChartsCodes.Error;
            pageMain.UpdateAnalysisData(PageMain.currentFilt, currentChart);
            ChangeCurrentChart(currentChart);
        }

        private void BtnOutdatedsChart_Click(object sender, RoutedEventArgs e)
        {
            currentChart = ChartsCodes.Outdate;
            pageMain.UpdateAnalysisData(PageMain.currentFilt, currentChart);
            ChangeCurrentChart(currentChart);
        }

        private void BtnActualsChart_Click(object sender, RoutedEventArgs e)
        {
            currentChart = ChartsCodes.Actual;
            pageMain.UpdateAnalysisData(PageMain.currentFilt, currentChart);
            ChangeCurrentChart(currentChart);
        }
        private void UpdateData()
        {
            ChartFull.Series = FullCollection;
            ChartErrors.Series = ErrorsCollection;
            ChartOutdatedFw.Series = OutdatedFwCollection;
            ChartActualFw.Series = ActualFwCollection;

            LbActualsFull.Content = Actuals;
            LbActuals.Content = Actuals;
            LbErrorsFull.Content = Errors;
            LbErrors.Content = Errors;
            LbOutdatesFull.Content = Outdates;
            LbOutdates.Content = Actuals;

        }
        private void ChangeCurrentChart(ChartsCodes code)
        {
            if (code == ChartsCodes.Full)
            {
                BorderFullChart.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#486FEF");
                BorderErrorsChart.BorderBrush = null;
                BorderOutdatesChart.BorderBrush = null;
                BorderActualsChart.BorderBrush = null;
            }
            else if (code == ChartsCodes.Error)
            {
                BorderFullChart.BorderBrush = null;
                BorderErrorsChart.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#486FEF");
                BorderOutdatesChart.BorderBrush = null;
                BorderActualsChart.BorderBrush = null;
            }
            else if (code == ChartsCodes.Outdate)
            {
                BorderFullChart.BorderBrush = null;
                BorderErrorsChart.BorderBrush = null;
                BorderOutdatesChart.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#486FEF");
                BorderActualsChart.BorderBrush = null;
            } 
            else
            {                                     
                BorderFullChart.BorderBrush = null;
                BorderErrorsChart.BorderBrush = null;
                BorderOutdatesChart.BorderBrush = null;
                BorderActualsChart.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#486FEF");
            }
        }

    }
}
