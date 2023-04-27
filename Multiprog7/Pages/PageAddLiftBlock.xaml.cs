using Multiprog7.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Multiprog7.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageAddLiftBlock.xaml
    /// </summary>
    public partial class PageAddLiftBlock : Page
    {
        public static ObservableCollection<LiftBlocksInfo> LiftBlocks = new ObservableCollection<LiftBlocksInfo>();
        ResourceDictionary Styles = (ResourceDictionary)Application.LoadComponent(
            new Uri("/MultiProg7;component/ResourceDictionaries/Styles.xaml", UriKind.Relative));
        private string styleActiveMode = "BtnActiveMode";
        private string styleUpdateDisabled = "BtnUpdateDisabled";

        private string BatDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\Bats\\";

        public PageAddLiftBlock()
        {
            InitializeComponent();
            LViewLbForConnect.ItemsSource = LiftBlocks;
        }

        private void BtnAddLiftBlock_Click(object sender, RoutedEventArgs e)
        {
            PageConnect pageConnect = new PageConnect();
            pageConnect.MyEvent += new EventHandler(LViewLbForConnect_CollectionChanged);
            NavigationService.Navigate(pageConnect);

        }

        private void BtnSaveBat_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!Directory.Exists(BatDirectory))
                    Directory.CreateDirectory(BatDirectory);

                var path = Path.Combine(BatDirectory, $"LB_count{LiftBlocks.Count}.bat");

                LiftBlocksInfo.SaveFullBat(path, LiftBlocks);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PageMain());
        }

        private void BtnDeleteAll_Click(object sender, RoutedEventArgs e)
        {
            for (int i = LiftBlocks.Count-1; i >=0 ; i--)
                LiftBlocks.Remove(LiftBlocks[i]);

            FirstView();
            ((INotifyCollectionChanged)LViewLbForConnect.Items).CollectionChanged += LViewLbForConnect_CollectionChanged;
        }

        private void BtnSaveCurrentLBBat_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!Directory.Exists(BatDirectory))
                    Directory.CreateDirectory(BatDirectory);
                var selectedLiftBlock = LViewLbForConnect.SelectedItem as LiftBlocksInfo;

                var path = Path.Combine(BatDirectory, $"{selectedLiftBlock}_{selectedLiftBlock.UnitID}.bat");
                
                selectedLiftBlock.SaveCurrentLbBat(path, (LViewLbForConnect.SelectedItem as LiftBlocksInfo).Connect);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void BtnDeleteCurrentLB_Click(object sender, RoutedEventArgs e)
        {
            var item = LViewLbForConnect.SelectedItem as LiftBlocksInfo;
            LiftBlocks.Remove(item);
            
            if (LViewLbForConnect.Items.Count == 0)
                FirstView();
            else if (LViewLbForConnect.Items.Count > 1)
                UpdateView();

            ((INotifyCollectionChanged)LViewLbForConnect.Items).CollectionChanged += LViewLbForConnect_CollectionChanged;
        }

        private void LViewLbForConnect_CollectionChanged(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (LViewLbForConnect.Items.Count > 0)
                {
                    BtnConnect.Style = (Style)Styles[styleActiveMode];
                    BtnConnect.IsEnabled = true;

                    UpdateMargin();
                    if (LViewLbForConnect.Items.Count == 1)
                    {
                        SecondVew();
                        return;
                    }

                    if (MainBorder.Height != 565)
                        if (LViewLbForConnect.Items.Count == 5)
                        {
                            MainBorder.Height = 565;
                            RowSecond.Height = new GridLength(350);
                        }
                        else
                            UpdateView();
                }
                else
                {
                    BtnConnect.Style = (Style)Styles[styleUpdateDisabled];
                    BtnConnect.IsEnabled = false;
                }
            });

            
        }

        private void UpdateView()
        {
            UpdateMargin();
            if (LViewLbForConnect.Items.Count == 5)
            {
                MainBorder.Height = 565;
                RowSecond.Height = new GridLength(350);
            }
            else
            {
                MainBorder.Height = 323 + LViewLbForConnect.Items.Count * 80;
                RowSecond.Height = new GridLength(LViewLbForConnect.Items.Count * 85);
            }
           
        }
        private void UpdateMargin()
        {
            if (LViewLbForConnect.Items.Count > 5)
            {
                LViewLbForConnect.Margin = new Thickness(15, 6, 0, 0);
            }
            else
            {
                LViewLbForConnect.Margin = new Thickness(0, 6, 0, 0);
            }
        }
        private void FirstView()
        {
            PanelWithBtns.Visibility = Visibility.Collapsed;
            MainBorder.Height = 227;
            BtnConnect.Margin = new Thickness(0);
            RowSecond.Height = new GridLength(85);
        }
        private void SecondVew()
        {
            PanelWithBtns.Visibility = Visibility.Visible;
            MainBorder.Height = 323;
            RowSecond.Height = new GridLength(40 + LViewLbForConnect.Items.Count * 80);
            BtnConnect.Margin = new Thickness(0, 20, 0, 0);

        }
        
    }
}
