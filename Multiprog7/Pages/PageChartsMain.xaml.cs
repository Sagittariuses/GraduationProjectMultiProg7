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
        };
        public PageMain pageMain;
        public static ChartsCodes currentChart;

        static int Errors, Outdates, Actuals;

        public PageCharts()
        {
            InitializeComponent();

            FullCollection = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "Errors",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(1) },
                    Fill = (Brush) new BrushConverter().ConvertFrom(ColorsHEX[0]),
                    Margin = new Thickness(-15 - 15 -15 -15),
                    StrokeThickness = 0,
                    Stroke = (Brush)new BrushConverter().ConvertFrom(ColorsHEX[0]),
                },

                new PieSeries
                {
                    Title = "OutdatedFw ",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(1) },
                    Fill = (Brush) new BrushConverter().ConvertFrom(ColorsHEX[1]),
                    Margin = new Thickness(-15 - 15 -15 -15),
                    StrokeThickness = 0,
                    Stroke = (Brush)new BrushConverter().ConvertFrom(ColorsHEX[1]),
                },
                new PieSeries
                {
                    Title = "ActualFw",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(1) },
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
                    Values = new ChartValues<ObservableValue> { new ObservableValue(1) },
                    Fill = (Brush) new BrushConverter().ConvertFrom(ColorsHEX[0]),
                    Margin = new Thickness(-15 - 15 -15 -15),
                    StrokeThickness = 0,
                    Stroke = (Brush)new BrushConverter().ConvertFrom(ColorsHEX[0]),
                },

                new PieSeries
                {
                    Title = "Fill ",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(1) },
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
                    Values = new ChartValues<ObservableValue> { new ObservableValue(1) },
                    Fill = (Brush) new BrushConverter().ConvertFrom(ColorsHEX[1]),
                    Margin = new Thickness(-15 - 15 -15 -15),
                    StrokeThickness = 0,
                    Stroke = (Brush)new BrushConverter().ConvertFrom(ColorsHEX[1]),
                },

                new PieSeries
                {
                    Title = "Fill ",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(1) },
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
                    Values = new ChartValues<ObservableValue> { new ObservableValue(1) },
                    Fill = (Brush)new BrushConverter().ConvertFrom(ColorsHEX[2]),
                    Margin = new Thickness(-15 - 15 - 15 - 15),
                    StrokeThickness = 0,
                    Stroke = (Brush)new BrushConverter().ConvertFrom(ColorsHEX[2]),
                },

                new PieSeries
                {
                    Title = "Fill ",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(1) },
                    Fill = (Brush) new BrushConverter().ConvertFrom(ColorsHEX[3]),
                    Margin = new Thickness(-15 - 15 -15 -15),
                    StrokeThickness = 0,
                    Stroke = (Brush)new BrushConverter().ConvertFrom(ColorsHEX[3]),
                },
            };
            UpdateData_FilterChart();
        }

        public SeriesCollection FullCollection { get; set; }
        public SeriesCollection ErrorsCollection { get; set; }
        public SeriesCollection OutdatedFwCollection { get; set; }
        public SeriesCollection ActualFwCollection { get; set; }

        private void BtnChartByType_Click(object sender, RoutedEventArgs e)
        {
            if (currentChart != ChartsCodes.Full)
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
        private void UpdateData_FilterChart()
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

        public void UpdateLabelData(int errros, int outdates, int actuals)
        {
            LbActuals.Content = actuals;
            LbActualsFull.Content = actuals;
            LbErrors.Content = errros;
            LbErrorsFull.Content = errros;
            LbOutdates.Content = outdates;
            LbOutdatesFull.Content = outdates;
        }

        public void UpdateActualData()
        {
            Errors = PageMain.OcAllDevFwData.Count(p => p.StatusCode == FirmwareStatus.Error);
            Outdates = PageMain.OcAllDevFwData.Count(p => p.StatusCode == FirmwareStatus.Outdated);
            Actuals = PageMain.OcAllDevFwData.Count(p => p.StatusCode == FirmwareStatus.Actual);

            try
            {

                Dispatcher.Invoke(() =>
                {
                    UpdateLabelData(Errors, Outdates, Actuals);
                    foreach (var series in FullCollection)
                        if (series.Title == "Errors")
                            foreach (var observable in series.Values.Cast<ObservableValue>())
                                observable.Value = Errors;
                        else if (series.Title == "OutdatedFw")
                            foreach (var observable in series.Values.Cast<ObservableValue>())
                                observable.Value = Outdates;
                        else
                            foreach (var observable in series.Values.Cast<ObservableValue>())
                                observable.Value = Actuals;

                    foreach (var series in ErrorsCollection)
                        if (series.Title == "Errors")
                            foreach (var observable in series.Values.Cast<ObservableValue>())
                                observable.Value = Errors;
                        else
                            foreach (var observable in series.Values.Cast<ObservableValue>())
                                observable.Value = PageMain.OcAllDevFwData.Count - Errors;

                    foreach (var series in OutdatedFwCollection)
                        if (series.Title == "OutdatedFw")
                            foreach (var observable in series.Values.Cast<ObservableValue>())
                                observable.Value = Outdates;
                        else
                            foreach (var observable in series.Values.Cast<ObservableValue>())
                                observable.Value = PageMain.OcAllDevFwData.Count - Outdates;

                    foreach (var series in ActualFwCollection)
                        if (series.Title == "ActualFw")
                            foreach (var observable in series.Values.Cast<ObservableValue>())
                                observable.Value = Actuals;
                        else
                            foreach (var observable in series.Values.Cast<ObservableValue>())
                                observable.Value = PageMain.OcAllDevFwData.Count - Actuals;
                });
            }   
            catch { }
           
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
