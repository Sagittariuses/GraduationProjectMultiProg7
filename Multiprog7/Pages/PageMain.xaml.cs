using Multiprog7.Model;
using LKDSFramework;
using LKDSFramework.Packs;
using Multiprog7.Classes;
using Multiprog7.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using static LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPProcessorAns;
using System.Windows.Data;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using LKDSFramework.Packs.DataDirect;
using LKDSFramework.FWUpdate;
using LKDSFramework.Packs.DataDirect.IAPService;
using static LKDSFramework.FWUpdate.FWCheckForUpdate;

namespace Multiprog7.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageMain.xaml
    /// </summary>
    public partial class PageMain : Page
    {
        #region Vars

        #region Int type

        const int ExpLen = 4;
        const int PartSize = 928;
        int PageNum;
        int progressValue = 0;
        int CounterDevSubdev = 0;
        int Step = 0;
        int SelectedPageNum = 2;
        int ActualFWNum = 0;
        const int FirstFragmentNum = 0;
        int ActualPageNum;
        int PagesCount;
        #endregion

        #region Lkds types

        public static DriverV7Net Driver = new DriverV7Net();
        LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPWriteAns FirmwareLoadPackAns;
        SubDeviceV7 FocusedDev;
        VirtualDeviceV7 opt = null;
        #endregion

        #region Lists

        List<SubDeviceV7> SubDevices = new List<SubDeviceV7>();
        List<byte[]> FirmwareFragments = new List<byte[]>();
        List<string> FwOnPages = new List<string>();
        List<FWForDevice> FirmwaresToDownload = new List<FWForDevice>();
        #endregion

        #region String type

        public static string FileExt;
        public static string FileFW;
        public static string FwArchivePath;

        private string labelContentUpdate = "Обновление";
        private string labelContentPrepareFile = "Подгтовка файла";
        private string labelContentAnalyze = "Анализ";


        private string styleActiveMode = "BtnActiveMode";
        private string styleInactiveMode = "BtnInactiveMode";
        private string styleUpdateDisable = "BtnUpdateDisabled";
        private string styleUpdateEnable = "BtnUpdateEnabled";
        private string selectedStyle;


        private string FwZipPath = AppDomain.CurrentDomain.BaseDirectory + "Firmwares\\";
        const string FwZipName= "firmware.zip";
        const string FwXmlName = "firmware.zip";


        string FirmwareFile = null;

        #endregion

        #region Byte type

        byte SelectedDevOrSubDevCANID;
        byte CANID;
        List<List<byte[]>> ListOfFWList = new List<List<byte[]>>();
        #endregion

        #region Bool type

        bool FWGet = false;
        bool LBCheck = false;
        bool CheckAnalyze = false;
        bool SendLastFragFlag = false;
        bool OnlineActive = true;
        bool OfflineActive = false;
        bool ManualActive = false;
        private bool isEnableUpdate = false;
        bool GoNext = false;
        bool FinishFlag = false;
        bool EmptyPageFound = false;
        bool ActualFlag = false;
        bool FirstWriteFlag = true;
        const bool FlagForFirstFragment = false;
        bool RebootingTheDeviceFlag = false;

        bool IsUpdateFW = false;
        bool IsLiftBlock = false;
        bool UpdateListView = false;

        #endregion

        #region Long type

        private long time = 0;

        #endregion

        #region Other types


        ResourceDictionary Styles = (ResourceDictionary)Application.LoadComponent(
            new Uri("/MultiProg7;component/ResourceDictionaries/Styles.xaml", UriKind.Relative));

        public ObservableCollection<VMDevice> OcVMDev;


        public static FWForDevice FwFromManualMode = null;
        #endregion


        public static FilterAnalysis currentFilt;



        public static ObservableCollection<FirmwareAnalysis> OcAllDevFwData = new ObservableCollection<FirmwareAnalysis>();
        ObservableCollection<FirmwareAnalysis> OcLiftFwData = new ObservableCollection<FirmwareAnalysis>();

        List<SubDeviceV7> subDeviceV7s = new List<SubDeviceV7>();
        List<DeviceV7> deviceV7s = new List<DeviceV7>();
        public static List<VirtualDeviceV7> LocalDeviceV7s = new List<VirtualDeviceV7>();

        PageCharts pageCharts = new PageCharts();
        WndDetail wndDetail;
        bool DownloadFlag = false;
        bool CanAdd = true;
        bool LbSubdevUpdate = false;
        bool AnalyzeFlag = false;


        DateTime _downloadStartTime;
        DateTime _analyzeStartTime;

        FWCheckForUpdate checkForUpdate;
        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();

        int counter = 0;

        Thread ThForPreparation;

        #endregion

        #region Page Events
        public PageMain()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(FwZipPath))
                Directory.CreateDirectory(FwZipPath);

            try
            {

                var client = new WebClient();

                Uri link = new Uri("http://www.lkds.ru/upload/firmware/firmware.zip?20230414");
                //Uri link = new Uri("http://lkds.ru/upload/firmware/firmware.xml");
                var path = Path.Combine(FwZipPath, FwZipName);


                LbState.Content = "Скачивание файла с прошивками";
                client.DownloadFileCompleted += Client_DownloadFileCompleted; ;
                client.DownloadProgressChanged += Client_DownloadProgressChanged;

                _downloadStartTime = DateTime.Now;

                client.DownloadFileAsync(link, path);





                ///
                /// Если не нужно скачивать файл
                ///
                /*PbMain.Maximum = 1;
                PbMain.Value = 0;
                LbState.Content = "Анализ";
                LbProgressPercent.Content = "0%";

                checkForUpdate = new FWCheckForUpdate(new string[] { Path.Combine(FwZipPath, FwZipName) });


                Driver.OnDevChange += new DeviceV7.OnDevChangeDelegate(Driver_OnDevChange);
                Driver.OnReceiveData += Driver_OnReceiveData;
                if (!Driver.Init())
                    return;

                try
                {
                    if (PageAddLiftBlock.LiftBlocks.Count != 0)
                        foreach (var item in PageAddLiftBlock.LiftBlocks)
                        {
                            DeviceV7 dev = item;

                            Driver.AddDevice(ref dev);
                        }

                    else
                    {
                        var Devices = DeviceV7.FromArgs(App.Args);

                        List<string> args = new List<string>();




                        for (int i = 0; i < App.Args.Count(); i++)
                        {
                            if (!App.Args[i].Contains("+"))
                                args.Add(App.Args[i].Trim());
                            else
                            {
                                Devices = DeviceV7.FromArgs(args.ToArray());
                                Driver.AddDevice(ref Devices[0]);
                                args.Clear();
                            }
                            if (i == App.Args.Count() - 1)
                            {
                                if (args.Count == 0)
                                    break;
                                Devices = DeviceV7.FromArgs(args.ToArray());
                                Driver.AddDevice(ref Devices[0]);
                                args.Clear();
                            }

                        }

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }


                timer.Tick += new EventHandler(timerTick);
                timer.Interval = new TimeSpan(0, 0, 1);
                timer.Start();
                AnalyzeFlag = true;
                _analyzeStartTime = DateTime.Now;*/
            }
            catch
            {
                Console.WriteLine("Connection error");
            }

            pageCharts.pageMain = this;
            FrameChartsMain.Navigate(pageCharts);
            pageCharts.UpdateActualData();
            UpdateAnalysisData(currentFilt, PageCharts.currentChart);

        }        

        #endregion

        #region Modes
        private void BtnOnlineMode_Click(object sender, RoutedEventArgs e) => ChangeMode(ModesCodes.Online);

        private void BtnOfflineMode_Click(object sender, RoutedEventArgs e) => ChangeMode(ModesCodes.Offline);

        private void BtnManualMode_Click(object sender, RoutedEventArgs e) => ChangeMode(ModesCodes.Manual);
        #endregion


        #region WebClient Events

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            var elapsedTime = (DateTime.Now - _downloadStartTime).TotalSeconds;

            PbMain.Maximum = e.TotalBytesToReceive;
            PbMain.Value = e.BytesReceived;
            var allTimeFordownloading = elapsedTime * PbMain.Maximum / PbMain.Value;
            var remainingTime = allTimeFordownloading - elapsedTime;
            var ts = TimeSpan.FromSeconds(remainingTime);

            if (ts.Hours > 0)
                LbRemainingTime.Content = $"{ts.Hours}ч {ts.Minutes}мин";
            else if (ts.Minutes > 0)
                LbRemainingTime.Content = $"{ts.Minutes}мин {ts.Seconds}сек";
            else
                LbRemainingTime.Content = $"{ts.Seconds} сек";


            LbProgressPercent.Content = Convert.ToInt32(PbMain.Value * 100 / PbMain.Maximum) + "%";
        }

        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            UpdateProgressBarMax();
            PbMain.Value = 0;
            LbState.Content = "Анализ";
            LbProgressPercent.Content = "0%";

            checkForUpdate = new FWCheckForUpdate(new string[] { Path.Combine(FwZipPath, FwZipName) });

            Driver.OnDevChange += new DeviceV7.OnDevChangeDelegate(Driver_OnDevChange);
            Driver.OnReceiveData += Driver_OnReceiveData;
            if (!Driver.Init())
                return;

            try
            {
                if (PageAddLiftBlock.LiftBlocks.Count != 0)
                    foreach (var item in PageAddLiftBlock.LiftBlocks)
                    {
                        DeviceV7 dev = item;

                        Driver.AddDevice(ref dev);
                    }

                else
                {
                    var Devices = DeviceV7.FromArgs(App.Args);

                    List<string> args = new List<string>();


                   

                    for (int i = 0; i < App.Args.Count(); i++)
                    {
                        if (!App.Args[i].Contains("+"))
                            args.Add(App.Args[i].Trim());
                        else
                        {
                            Devices = DeviceV7.FromArgs(args.ToArray());
                            Driver.AddDevice(ref Devices[0]);
                            args.Clear();
                        }
                        if (i == App.Args.Count() - 1)
                        {
                            if (args.Count == 0)
                                break;
                            Devices = DeviceV7.FromArgs(args.ToArray());
                            Driver.AddDevice(ref Devices[0]);
                            args.Clear();
                        }

                    }

                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
            }


            timer.Tick += new EventHandler(timerTick);
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
            AnalyzeFlag = true;
            _analyzeStartTime = DateTime.Now;
        }

        #endregion

        #region LKDSFramework Events

        private void Driver_OnReceiveData(PackV7 pack)
        {
            UpdateProgressBarMax();
            #region comment  whoans

            if (pack is PackV7WhoAns)
            {
                PackV7WhoAns who = pack as PackV7WhoAns;
                var dev = who.Device;

                #region new ver dll 
                FirmwareAnalysis firmwareAnalysis = null;
                lock (this)
                {
                    try
                    {
                        while(checkForUpdate == null) { }

                        var fwAndResults = checkForUpdate.Analyze(who.IAPDeviceInfo);

                        var devFwVer = GetFWVer(who.IAPDeviceInfo.Processors[0]);

                        var devType = dev is DeviceV7 ? dev.DevClass.GetNameOfEnum().ToString() : dev.SubDevClass.GetNameOfEnum().ToString();

                        

                        foreach (var result in fwAndResults)
                        {
                            DateTime createDate = result.fileDescriptor == null ? DateTime.MinValue : result.fileDescriptor.IFD.CreateDate;
                            Console.WriteLine($"Result: {result.checkResult} _ {++counter}");
                            var descriptor = result.fileDescriptor;
                            switch (result.checkResult)
                            {
                                case CheckResult.firmwareIsNotFound: case CheckResult.firmwareMatch: case CheckResult.currentFirmwareIsNewr: case CheckResult.justManualUpdate:
                                    {
                                        string[] path = FirmwareStatus.Actual.GetNameOfEnum().Split('|');

                                        firmwareAnalysis = new FirmwareAnalysis(pack.CanID)
                                        {
                                            CAN = who.CanID,
                                            Unit = who.UnitID,
                                            FileDescriptor = descriptor,
                                            DevType = devType,
                                            Title = dev.ToString(),
                                            Version = devFwVer,
                                            StatusCode = FirmwareStatus.Actual,
                                            Date = createDate,
                                            StatusTxt = result.checkResult.GetNameOfEnum(),
                                            PathFill = path[0].Trim(),
                                            PathData = path[1].Trim()
                                        };
                                        break;
                                    }
                                   
                                case CheckResult.newFirmwareIsFound:
                                    {
                                        string[] path = FirmwareStatus.Outdated.GetNameOfEnum().Split('|');

                                        firmwareAnalysis = new FirmwareAnalysis(pack.CanID)
                                        {
                                            CAN = who.CanID,
                                            Unit = who.UnitID,
                                            FileDescriptor = descriptor,
                                            DevType = devType,
                                            Title = dev.ToString(),
                                            Version = devFwVer,
                                            StatusCode = FirmwareStatus.Outdated,
                                            Date = createDate,
                                            StatusTxt = result.checkResult.GetNameOfEnum(),
                                            PathFill = path[0].Trim(),
                                            PathData = path[1].Trim(),

                                        };
                                        break;
                                    }
                                default:
                                    {
                                        string[] path = FirmwareStatus.Error.GetNameOfEnum().Split('|');

                                        firmwareAnalysis = new FirmwareAnalysis(pack.CanID)
                                        {
                                            CAN = who.CanID,
                                            Unit = who.UnitID,
                                            FileDescriptor = descriptor,
                                            DevType = devType,
                                            Title = dev.ToString(),
                                            Version = devFwVer,
                                            StatusCode = FirmwareStatus.Error,
                                            Date = createDate,
                                            StatusTxt = result.checkResult.GetNameOfEnum(),
                                            PathFill = path[0].Trim(),
                                            PathData = path[1].Trim(),
                                        };
                                        break;
                                    }
                            }

                           

                        }
                       


                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.StackTrace);
                        Console.WriteLine(ex.Source);

                    }

                    Dispatcher.Invoke(() =>
                    {
                        try
                        {

                            PbMain.Value++;
                            LbProgressPercent.Content = Convert.ToInt32(PbMain.Value * 100 / PbMain.Maximum) + "%";
                            var elapsedTime = (DateTime.Now - _analyzeStartTime).TotalSeconds;
                            var allTimeFordownloading = elapsedTime * PbMain.Maximum / PbMain.Value;

                            var remainingTime = allTimeFordownloading - elapsedTime;
                            var ts = TimeSpan.FromSeconds(remainingTime);

                            if (ts.Hours > 0)
                                LbRemainingTime.Content = $"{ts.Hours}ч {ts.Minutes}мин";
                            else if (ts.Minutes > 0)
                                LbRemainingTime.Content = $"{ts.Minutes}мин {ts.Seconds}сек";
                            else
                                LbRemainingTime.Content = $"{ts.Seconds} сек";

                            Update(firmwareAnalysis, who);

                           
                        }
                        catch { }


                    });

                    return;


                }

                #endregion
            }
            #endregion

            #region comment procans

            /*if (pack is PackV7IAPProcessorAns)
            {
               *//* if (subDeviceV7s.Count == 0)
                    return;*//*

                //SubDeviceV7 subdev = subDeviceV7s.FirstOrDefault(p => p.CanID == pack.CanID);



                PackV7IAPProcessorAns PackProcessorAns = pack as PackV7IAPProcessorAns;


                CheckForUpdate checkForUpdate = new CheckForUpdate(new UpdateOption());

                var res = checkForUpdate.check(PackProcessorAns.IAPDeviceInfo);


                foreach (var item in res)
                {
                    Console.WriteLine($"--------\n UnitID: {PackProcessorAns.UnitID} CanId: {PackProcessorAns.CanID} CheckRes: {item.checkResult}\n---------");
                    Console.WriteLine($"--------\nDescriptor: {item.fileDescriptor.ifd.descriptor}\n---------");
                }

                *//* foreach (FirmwareInfo FWInfo in FirmwareInfo.firmwares)
                 {
                     foreach (ProcessorStateStruct PSS in PackProcessorAns.Proccessors)
                         if (FWInfo.deviceClass.Equals(PSS.Aclass))
                             if (FWInfo.devicePlatform.Equals(PSS.Bplatform))
                                 if (FWInfo.deviceType.Equals(PSS.Cdevtype))
                                     if (FWInfo.deviceProcessor.Equals(PSS.DNum))
                                         if (FWInfo.localization.Equals(PSS.Elocal))
                                         {
                                             //Console.WriteLine("Localization: " + FWInfo.localization);
                                             string[] URLFrags = FWInfo.url.Split('/');
                                             var m = Regex.Matches(URLFrags[URLFrags.Length - 1], @"\d+,?\d+").Cast<Match>();
                                             string[] VerFragments = FWInfo.version.ToString().Split('.');
                                             string ActualVer = "";

                                             foreach (string VerFrag in VerFragments)
                                             {
                                                 ActualVer += VerFrag;
                                             }

                                             string CurrentDevVer = "";
                                             string CurrentDevVerView = "";

                                             for (int i = 0; i < PSS.Ver.ToArray.Length; i++)
                                             {
                                                 CurrentDevVer += PSS.Ver.ToArray[i];
                                                 CurrentDevVerView += $"{PSS.Ver.ToArray[i]}.";
                                             }

                                             CurrentDevVerView = CurrentDevVerView.Trim();


                                             if (Convert.ToInt32(ActualVer) > Convert.ToInt32(CurrentDevVer))
                                             {

                                                 //
                                                 // Версия устарела
                                                 //
                                                 #region
                                                 string[] path = FirmwareStatus.Outdated.GetNameOfEnum().Split('|');

                                                 FirmwareAnalysis firmwareAnalysis = new FirmwareAnalysis(pack.CanID)
                                                 {
                                                     CAN = pack.CanID,
                                                     Unit = subdev.Parrent.UnitID,
                                                     DevType = subdev.SubDevClass.GetNameOfEnum().ToString(),
                                                     Title = subdev.ToString(),
                                                     Version = CurrentDevVerView,
                                                     StatusCode = FirmwareStatus.Outdated,
                                                     Date = DateTime.Now.ToShortDateString(),
                                                     StatusTxt = "Устаревшая прошивка ",
                                                     PathFill = path[0].Trim(),
                                                     PathData = path[1].Trim(),

                                                 };

                                                 UpdateCollections(firmwareAnalysis, PackProcessorAns);

                                                 FirmwareFile = Path.Combine(FwZipPath, URLFrags[URLFrags.Length - 1]);
                                                 bool DownloadFlag = false;
                                                 while (!DownloadFlag)
                                                 {
                                                     try
                                                     {
                                                         using (var client = new WebClient())
                                                         {
                                                             client.DownloadFile(FWInfo.url, FirmwareFile);
                                                         }
                                                         DownloadFlag = true;
                                                     }
                                                     catch { }
                                                 }
                                                 FirmwaresToDownload.Add(new FWForDevice(Convert.ToInt32(ActualVer), FWInfo.url, FirmwareFile, URLFrags[URLFrags.Length - 1]));
                                                 Dispatcher.Invoke(() =>
                                                 {
                                                     PbMain.Value++;
                                                     LbProgressPercent.Content = Convert.ToInt32(PbMain.Value * 100 / PbMain.Maximum) + "%";
                                                 });

                                                 #endregion
                                                 continue;
                                             }
                                             else
                                             {

                                                 FileInfo[] files = new DirectoryInfo(FwZipPath).GetFiles(URLFrags[URLFrags.Length - 1]);

                                                 if (files.Length == 0)
                                                 {

                                                     //
                                                     // Считаем, что актуальна, если нет в скачанных
                                                     //
                                                     #region

                                                     string[] path = FirmwareStatus.Actual.GetNameOfEnum().Split('|');


                                                     FirmwareAnalysis firmwareAnalysis = new FirmwareAnalysis(pack.CanID)
                                                     {
                                                         CAN = pack.CanID,
                                                         Unit = subdev.Parrent.UnitID,
                                                         DevType = subdev.SubDevClass.GetNameOfEnum().ToString(),
                                                         Title = subdev.ToString(),
                                                         Version = FWInfo.version.ToString(),
                                                         StatusCode = FirmwareStatus.Actual,
                                                         Date = FWInfo.date,
                                                         StatusTxt = "Актуальная прошивка",
                                                         PathFill = path[0].Trim(),
                                                         PathData = path[1].Trim()
                                                     };

                                                     UpdateCollections(firmwareAnalysis, PackProcessorAns);

                                                     Dispatcher.Invoke(() =>
                                                     {
                                                         PbMain.Value++;
                                                         LbProgressPercent.Content = Convert.ToInt32(PbMain.Value * 100 / PbMain.Maximum) + "%";
                                                     });


                                                     #endregion
                                                     continue;
                                                 }
                                                 else if (files[0].CreationTime < Convert.ToDateTime(FWInfo.date))
                                                 {
                                                     //
                                                     // Устарела
                                                     //
                                                     #region
                                                     string[] path = FirmwareStatus.Outdated.GetNameOfEnum().Split('|');
                                                     FirmwareAnalysis firmwareAnalysis = new FirmwareAnalysis(pack.CanID)
                                                     {
                                                         CAN = pack.CanID,
                                                         Unit = subdev.Parrent.UnitID,
                                                         DevType = subdev.SubDevClass.GetNameOfEnum().ToString(),
                                                         Title = subdev.ToString(),
                                                         Version = CurrentDevVerView,
                                                         StatusCode = FirmwareStatus.Outdated,
                                                         Date = "дата с устройства",
                                                         StatusTxt = "Устаревшая прошивка",
                                                         PathFill = path[0].Trim(),
                                                         PathData = path[1].Trim()
                                                     };

                                                     UpdateCollections(firmwareAnalysis, PackProcessorAns);

                                                     Dispatcher.Invoke(() =>
                                                     {
                                                         PbMain.Value++;
                                                         LbProgressPercent.Content = Convert.ToInt32(PbMain.Value * 100 / PbMain.Maximum) + "%";
                                                     });
                                                     #endregion
                                                     continue;

                                                 }
                                                 else
                                                 {

                                                     //
                                                     // Актуальна
                                                     //
                                                     #region
                                                     string[] path = FirmwareStatus.Actual.GetNameOfEnum().Split('|');
                                                     FirmwareAnalysis firmwareAnalysis = new FirmwareAnalysis(pack.CanID)
                                                     {
                                                         CAN = pack.CanID,
                                                         Unit = subdev.Parrent.UnitID,
                                                         DevType = subdev.SubDevClass.GetNameOfEnum().ToString(),
                                                         Title = subdev.ToString(),
                                                         Version = FWInfo.version.ToString(),
                                                         StatusCode = FirmwareStatus.Actual,
                                                         Date = FWInfo.date,
                                                         StatusTxt = "Актуальная прошивка",
                                                         PathFill = path[0].Trim(),
                                                         PathData = path[1].Trim()
                                                     };

                                                     UpdateCollections(firmwareAnalysis, PackProcessorAns);

                                                     Dispatcher.Invoke(() =>
                                                     {
                                                         PbMain.Value++;
                                                         LbProgressPercent.Content = Convert.ToInt32(PbMain.Value * 100 / PbMain.Maximum) + "%";
                                                     });
                                                     #endregion
                                                     continue;
                                                 }
                                             }
                                         }
                 }*//*

                stopwatch.Stop();

                Dispatcher.Invoke(() =>
                {

                    DateTime end = DateTime.Now;

                    TimeSpan ts = (end - start);

                    int sec = Convert.ToInt32(ts.TotalMilliseconds / 1000 * subDeviceV7s.Count);
                    int min = Convert.ToInt32(sec / 60);
                    int hour = min / 60;

                    if (hour > 0)
                        LbRemainingTime.Content = $"{hour}ч {min - sec / 60}мин";
                    else if (min > 0)
                        LbRemainingTime.Content = $"{min}мин {sec - min * 60}сек";
                    else
                        LbRemainingTime.Content = $"{sec} сек";
                });
                UpdateView(subdev);


                return;
            }*/

            #endregion


        }

        private void Driver_OnDevChange(VirtualDeviceV7 _dev)
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    FWGet = true;

                    if (_dev is DeviceV7)
                    {
                        var dev = _dev as DeviceV7;

                        deviceV7s.Add(dev);
                        LocalDeviceV7s.Add(_dev);
                            
                        PbMain.Maximum += dev.LCountSubDev;
                        //Console.WriteLine(dev.UnitID);

                    } else if (_dev is SubDeviceV7)
                    {
                        var dev = _dev as SubDeviceV7;

                        LocalDeviceV7s.Add(_dev);
                        //Console.WriteLine(dev.Parrent.UnitID);

                    }

                   

                    CheckAnalyze = true;
                });

            }
            catch (Exception ex)
            {
                var a = ex.ToString();
            }
        }

        #endregion

        #region Other Events
        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            ListOfFWList.Clear();
            IsUpdateFW = true;
            FirmwaresToDownload.Clear();
            Driver.OnDevChange = null;

            if (OnlineActive)
                Console.WriteLine("OnlineMode");

            else if (OfflineActive)
            {
                WndOfflineMode wndOfflineMode = new WndOfflineMode();
                wndOfflineMode.ShowDialog();
            }
            else if (ManualActive)
            {
                WndManualMode wndManualMode = new WndManualMode();
                wndManualMode.ShowDialog();

                if (WndManualMode.IsApplied)
                    Console.WriteLine("PreparingTheFile()"); 
            }
        }

        private void LvAnalysisInfo_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                wndDetail = new WndDetail();
                var res = new ObservableCollection<FirmwareAnalysis>(OcAllDevFwData.Where(p => p.Unit == (LvAnalysisInfo.SelectedItem as FirmwareAnalysis).Unit));
                wndDetail.UpdateData(ref res);
                wndDetail.ShowDialog();
                wndDetail = null;
            }
            catch { }
        }

        private void BtnTitleFilt_Click(object sender, RoutedEventArgs e)
        {
            currentFilt = FilterAnalysis.Title;
            UpdateAnalysisData(currentFilt, PageCharts.currentChart);
            (sender as Button).Foreground = (Brush)new BrushConverter().ConvertFrom("#486FEF");
        }

        private void BtnFwFilt_Click(object sender, RoutedEventArgs e)
        {
            currentFilt = FilterAnalysis.FwVersion;
            UpdateAnalysisData(currentFilt, PageCharts.currentChart);
            (sender as Button).Foreground = (Brush)new BrushConverter().ConvertFrom("#486FEF");
        }

        private void BtnDateFilt_Click(object sender, RoutedEventArgs e)
        {
            currentFilt = FilterAnalysis.Date;
            UpdateAnalysisData(currentFilt, PageCharts.currentChart);
            (sender as Button).Foreground = (Brush)new BrushConverter().ConvertFrom("#486FEF");
        }

        private void BtnErrorTypeFilt_Click(object sender, RoutedEventArgs e)
        {
            currentFilt = FilterAnalysis.ErrorType;
            UpdateAnalysisData(currentFilt, PageCharts.currentChart);
            (sender as Button).Foreground = (Brush)new BrushConverter().ConvertFrom("#486FEF");
        }

        private void BtnSaveXlsx_Click(object sender, RoutedEventArgs e)
        {
            WndSaveXlsx wndSaveXlsx = new WndSaveXlsx();
            wndSaveXlsx.ShowDialog();
        }

        private void PbMain_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!AnalyzeFlag)
                return;
            if (PbMain.Value == PbMain.Maximum)
            {
                BtnUpdate.IsEnabled = true;
                BtnUpdate.Style = (Style)Styles[styleUpdateEnable];
            }
            else
            {
                BtnUpdate.IsEnabled = false;
                BtnUpdate.Style = (Style)Styles[styleUpdateDisable];
            }

        }

        private void timerTick(object sender, EventArgs e)
        {
            UpdateProgressBarMax();
        }

        #endregion

        #region Functions & procedures
        void PreparationForAnalysis()
        {
            checkForUpdate = new FWCheckForUpdate(new string[] { Path.Combine(FwZipPath, FwZipName) });
        }

        void Update(FirmwareAnalysis firmwareAnalysis, PackV7WhoAns who)
        {
            UpdateCollections(firmwareAnalysis, who);
            UpdateView(who.Device);
            pageCharts.UpdateActualData();
        }

        void ChangeMode(ModesCodes mode)
        {
            if (mode == ModesCodes.Online)
            {
                BtnOnlineMode.Style = (Style)Styles[styleActiveMode];
                BtnOfflineMode.Style = (Style)Styles[styleInactiveMode];
                BtnManualMode.Style = (Style)Styles[styleInactiveMode];

                OnlineActive = true;
                OfflineActive = false;
                ManualActive = false;
            }
            else if (mode == ModesCodes.Offline)
            {
                BtnOnlineMode.Style = (Style)Styles[styleInactiveMode];
                BtnOfflineMode.Style = (Style)Styles[styleActiveMode];
                BtnManualMode.Style = (Style)Styles[styleInactiveMode];

                OnlineActive = false;
                OfflineActive = true;
                ManualActive = false;
            }
            else
            {
                BtnOnlineMode.Style = (Style)Styles[styleInactiveMode];
                BtnOfflineMode.Style = (Style)Styles[styleInactiveMode];
                BtnManualMode.Style = (Style)Styles[styleActiveMode];

                OnlineActive = false;
                OfflineActive = false;
                ManualActive = true;
            }
        }

        public void UpdateAnalysisData(FilterAnalysis code, ChartsCodes chart)
        {
            var res = OcLiftFwData;

            if (chart == ChartsCodes.Error)
                res = new ObservableCollection<FirmwareAnalysis>(res.Where(p => p.StatusCode == FirmwareStatus.Error));
            else if (chart == ChartsCodes.Outdate)
                res = new ObservableCollection<FirmwareAnalysis>(res.Where(p => p.StatusCode == FirmwareStatus.Outdated));
            else if (chart == ChartsCodes.Actual)
                res = new ObservableCollection<FirmwareAnalysis>(res.Where(p => p.StatusCode == FirmwareStatus.Actual));


            if (code == FilterAnalysis.Title)
                res = new ObservableCollection<FirmwareAnalysis>(res.OrderBy(p => p.Title));
            else if (code == FilterAnalysis.FwVersion)
                res = new ObservableCollection<FirmwareAnalysis>(res.OrderBy(p => p.Version));
            else if (code == FilterAnalysis.Date)
                res = new ObservableCollection<FirmwareAnalysis>(res.OrderBy(p => p.Date));
            else
                res = new ObservableCollection<FirmwareAnalysis>(res.OrderBy(p => p.StatusCode));

            Dispatcher.Invoke(() => { LvAnalysisInfo.ItemsSource = res; });

        }

        void UpdateView(VirtualDeviceV7 dev)
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    LbDevCount.Content = OcAllDevFwData.Where(p => p.StatusCode == FirmwareStatus.Outdated).Count();
                    pageCharts.UpdateActualData();
                    UpdateAnalysisData(currentFilt, PageCharts.currentChart);
                    if (wndDetail != null)
                    {
                        var res = new ObservableCollection<FirmwareAnalysis>(OcAllDevFwData.Where(p => p.Unit == (LvAnalysisInfo.SelectedItem as FirmwareAnalysis).Unit));
                        wndDetail.UpdateData(ref res);
                    }
                });
            }
            catch { }
           
        }

        void UpdateCollections(FirmwareAnalysis firmwareAnalysis, PackV7WhoAns pack)
        {
            if (pack.Device is DeviceV7)
            {
                bool CheckDuplicate = false;
                foreach (var item in OcLiftFwData)
                    if (item.Unit == pack.UnitID)
                        CheckDuplicate = true;

                if (!CheckDuplicate)
                    OcLiftFwData.Add(firmwareAnalysis);
                /*if (OcAllDevFwData.ToList().FindAll(p => p.FileDescriptor == firmwareAnalysis.FileDescriptor && p.Unit == firmwareAnalysis.Unit).Count != 0)
                    return;*/
            }

            OcAllDevFwData.Add(firmwareAnalysis);
        }

        void UpdateProgressBarMax()
        {
            Dispatcher.Invoke(() =>
            {
                int PbMax = 1;

                foreach (var item in Driver.Devices)
                {
                    PbMax += item.LCountSubDev;
                    if (PbMax == 1)
                    {
                        PbMain.Value = 1;
                        PbMain.Maximum = PbMax;
                        Console.WriteLine("Информация о кол-ве устройств у ЛБ не обновлена");
                        return;
                    }
                }

                if (PbMax - 1 != 0)
                    PbMain.Maximum = PbMax - 1;
                else
                    PbMain.Maximum = PbMax;
            });
        }

        public string GetFWVer(ProcessorStateStruct proc)
        {
            return $"{proc.Ver.Byte3}.{proc.Ver.Byte2}.{proc.Ver.Byte1}.{proc.Ver.Byte0}";
        }


        #endregion

        #region Send methods
        /*
                void SendStateAsk()
                {
                    Driver.SendPack(new LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPStateAsk()
                    {
                        UnitID = opt.UnitID,
                        CanID = opt.CanID,
                    });
                }

                void SendReadAsk()
                {
                    Driver.SendPack(new LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPReadAsk()
                    {
                        UnitID = opt.UnitID,
                        Num = (byte)SelectedPageNum,
                        CanID = CANID
                    });
                }

                void SendCleardAsk()
                {
                    Driver.SendPack(new LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPClearAsk()
                    {
                        UnitID = opt.UnitID,
                        Num = (byte)SelectedPageNum,
                        CanID = CANID
                    });

                }

                void SendWriteAsk()
                {
                    if (ListOfFWList.Count > 0)
                    {
                        Driver.SendPack(new LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPWriteAsk()
                        {
                            UnitID = opt.UnitID,
                            Num = (byte)SelectedPageNum,
                            Fragment = new LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPWriteAsk.FragmentPageSturct()
                            {
                                Buff = ListOfFWList[ActualFWNum][FirstFragmentNum],
                                Offset = FirstFragmentNum,
                                isLastFrag = FlagForFirstFragment
                            },
                            CanID = CANID
                        });
                    }
                }

                void SendUpdateAsk()
                {

                    Driver.SendPack(new LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPUpdateAsk()
                    {
                        UnitID = opt.UnitID,
                        Num = (byte)SelectedPageNum,
                        CanID = CANID
                    });
                }

                void SendActivateAsk()
                {
                    Driver.SendPack(new LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPActiveAsk()
                    {
                        UnitID = opt.UnitID,
                        Num = (byte)SelectedPageNum,
                        CanID = CANID
                    });
                }

                void SendProcessorAsk()
                {
                    Driver.SendPack(new LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPProcessorAsk()
                    {
                        UnitID = opt.UnitID,
                        CanID = CANID
                    });
                }

                void SendProcessorAsk(uint UnitID, byte CanId)
                {
                    Driver.SendPack(new LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPProcessorAsk()
                    {
                        UnitID = UnitID,
                        CanID = CanId
                    });
                }

                void SendWhoAsk(uint UnitID, byte CanId)
                {
                    Driver.SendPack(new LKDSFramework.Packs.DataDirect.PackV7WhoAsk()
                    {
                        UnitID = UnitID,
                        CanID = CanId
                    });
                }
        */

        void SendProcessorAsk(uint UnitID, byte CanId)
        {
            Driver.SendPack(new LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPProcessorAsk()
            {
                UnitID = UnitID,
                CanID = CanId
            });
        }


        #endregion

       
    }

}
