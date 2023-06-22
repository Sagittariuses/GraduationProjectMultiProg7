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
using System.IO.Compression;

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

        int DevUpdate = 0;


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
        public static string FwArchivePath = "null";


        private string statustxt = "В процессе";
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

        bool UpdateFlag = false;
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

        ObservableCollection<FirmwareUpdate> OcOutdatesDevs = new ObservableCollection<FirmwareUpdate>();
        ObservableCollection<FirmwareUpdate> OcLiftFwDataUpdate = new ObservableCollection<FirmwareUpdate>();

        List<SubDeviceV7> subDeviceV7s = new List<SubDeviceV7>();
        List<DeviceV7> deviceV7s = new List<DeviceV7>();
        public static List<VirtualDeviceV7> LocalDeviceV7s = new List<VirtualDeviceV7>();

        PageCharts pageCharts = new PageCharts();
        WndDetail wndDetail;
        WndUpdateDetail wndUpdateDetail;
        bool DownloadFlag = false;
        bool CanAdd = true;
        bool OfflineArchiveFlag = false;
        bool AnalyzeFlag = false;


        DateTime _downloadStartTime;
        DateTime _analyzeStartTime;
        DateTime _updateStartTime;

        FWCheckForUpdate checkForUpdate;
        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();

        int counter = 0;

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
                var path = Path.Combine(FwZipPath, FwZipName);

                var CreationTime = File.GetLastWriteTime(path);

                if (!File.Exists(path) || (DateTime.Now - CreationTime).Days > 0)
                {

                   /* if (!ConnectionAvailable("http://www.lkds.ru/upload/firmware/"))
                    {
                        MessageBox.Show("Отсутсвует подключение к сети интернет", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                    }*/

                    

                    var client = new WebClient();

                    Uri link = new Uri("http://www.lkds.ru/upload/firmware/firmware.zip?20230414");
                    //Uri link = new Uri("http://lkds.ru/upload/firmware/firmware.xml");


                    LbState.Content = "Скачивание файла с прошивками";
                    client.DownloadFileCompleted += Client_DownloadFileCompleted; ;
                    client.DownloadProgressChanged += Client_DownloadProgressChanged;

                    _downloadStartTime = DateTime.Now;

                    client.DownloadFileAsync(link, path);
                } else
                {

                    PbMain.Maximum = 1;
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
           /* if (!ConnectionAvailable("http://www.lkds.ru/upload/firmware/"))
            {
                MessageBox.Show("Отсутсвует подключение к сети интернет", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }*/


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
            #region comment  whoans

            


            if (pack is PackV7WhoAns)
            {
                UpdateProgressBarMax();
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
                            Console.WriteLine($"Result: {result.checkResult} _ {++counter} _ canID {pack.CanID} _ unit {pack.UnitID}");
                            var descriptor = result.fileDescriptor;
                            switch (result.checkResult)
                            {
                                case CheckResult.firmwareIsNotFound: case CheckResult.firmwareMatch: case CheckResult.currentFirmwareIsNewr: case CheckResult.justManualUpdate:
                                    {
                                        string[] path = FirmwareErrorType.Actual.GetNameOfEnum().Split('|');

                                        firmwareAnalysis = new FirmwareAnalysis(pack.CanID)
                                        {
                                            CAN = who.CanID,
                                            Unit = who.UnitID,
                                            FileDescriptor = descriptor,
                                            DevType = devType,
                                            Title = dev.ToString(),
                                            Version = devFwVer,
                                            StatusCode = FirmwareErrorType.Actual,
                                            Date = createDate,
                                            StatusTxt = result.checkResult.GetNameOfEnum(),
                                            PathFill = path[0].Trim(),
                                            PathData = path[1].Trim()
                                        };
                                        break;
                                    }
                                   
                                case CheckResult.newFirmwareIsFound:
                                    {
                                        string[] path = FirmwareErrorType.Outdated.GetNameOfEnum().Split('|');

                                        firmwareAnalysis = new FirmwareAnalysis(pack.CanID)
                                        {
                                            CAN = who.CanID,
                                            Unit = who.UnitID,
                                            FileDescriptor = descriptor,
                                            DevType = devType,
                                            Title = dev.ToString(),
                                            Version = devFwVer,
                                            StatusCode = FirmwareErrorType.Outdated,
                                            Date = createDate,
                                            StatusTxt = result.checkResult.GetNameOfEnum(),
                                            PathFill = path[0].Trim(),
                                            PathData = path[1].Trim(),

                                        };
                                        break;
                                    }
                                default:
                                    {
                                        string[] path = FirmwareErrorType.Error.GetNameOfEnum().Split('|');

                                        firmwareAnalysis = new FirmwareAnalysis(pack.CanID)
                                        {
                                            CAN = who.CanID,
                                            Unit = who.UnitID,
                                            FileDescriptor = descriptor,
                                            DevType = devType,
                                            Title = dev.ToString(),
                                            Version = devFwVer,
                                             StatusCode = FirmwareErrorType.Error,
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

            if (RebootingTheDeviceFlag)
            {
                if (ActualFWNum == ListOfFWList.Count - 1)
                {
                    RebootingTheDeviceFlag = !RebootingTheDeviceFlag;
                    return;
                }
                else
                {
                    RebootingTheDeviceFlag = !RebootingTheDeviceFlag;
                    return;
                }

            }

            if (Step == 5)
            {
                lock (this)
                {
                    OcOutdatesDevs[DevUpdate - 1].StatusTxt = "Обновлён";
                    OcOutdatesDevs[DevUpdate - 1].Status = FirmwareUpdateStatus.Updated;
                    OcOutdatesDevs[DevUpdate - 1].PathDataState = FirmwareUpdateStatus.Updated.GetNameOfEnum();
                    Step = 1;
                    EmptyPageFound = false;
                    SelectedPageNum = 2;
                    PagesCount = 0;
                    ActualPageNum = 0;
                    FirstWriteFlag = true;
                    ActualFWNum = DevUpdate;
                    counter = 0;
                    if (DevUpdate >= OcOutdatesDevs.Count)
                    {
                        UpdateFlag = false;
                        Dispatcher.Invoke(() =>
                        {

                            LbProgressPercent.Content = "0%";
                            LbState.Content = "Анализ";
                            LbRemainingTime.Content = "0 сек";

                            Driver.OnDevChange += new DeviceV7.OnDevChangeDelegate(Driver_OnDevChange);
                            OcLiftFwData.Clear();
                            OcAllDevFwData.Clear();
                            OcLiftFwDataUpdate.Clear();
                            timer.Tick += new EventHandler(timerTick);

                            LvAnalysisInfo.Visibility = Visibility.Visible;
                            LvUpdateInfo.Visibility = Visibility.Hidden;


                            foreach (var dev in Driver.Devices)
                            {
                                var keys = dev.SubDevices.Keys;
                                var Unit = dev.UnitID;

                                dev.SendPack(new PackV7WhoAsk());

                                foreach (var key in keys)
                                    SendWhoAsk(Unit, key);
                            }
                            PbMain.Value = 0;
                            AnalyzeFlag = true;
                        });
                        return;
                    }
                    SendStateAsk(OcOutdatesDevs[DevUpdate].Unit, (byte)OcOutdatesDevs[DevUpdate].CAN);
                    return;
                }

            }

            if (pack is LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPActiveAns)
            {
                LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPActiveAns PackActiveAns = pack as LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPActiveAns;
                if (PackActiveAns.Error == LKDSFramework.Packs.DataDirect.PackV7IAPService.IAPErrorType.NoError)
                {
                    Console.WriteLine("/-/-/-/-/-/ Done /-/-/-/-/-/");
                    RebootingTheDeviceFlag = true;
                }
                else
                    Console.WriteLine("/-/-/-/-/-/ Error /-/-/-/-/-/");
            }

            if (pack is LKDSFramework.Packs.DataDirect.PackV7IAPService)
            {
                lock (this)
                {
                    LKDSFramework.Packs.DataDirect.PackV7IAPService IAPAns = (LKDSFramework.Packs.DataDirect.PackV7IAPService)pack;
                    if (IAPAns.Error != LKDSFramework.Packs.DataDirect.PackV7IAPService.IAPErrorType.NoError)
                    {
                        //WriteError(IAPAns);
                    }
                    else
                    {
                        if (UpdateFlag)
                        {
                            //
                            // Searching for an empty page
                            //

                            if (Step == 1)
                            {
                                if (pack is LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPReadAns && !EmptyPageFound)
                                {
                                    LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPReadAns PackReadAns = pack as LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPReadAns;
                                    if (PackReadAns.Data[3] == 85)
                                    {
                                        EmptyPageFound = true;
                                        SendCleardAsk(pack.UnitID, pack.CanID, (byte)SelectedPageNum);
                                        Step++;
                                        return;
                                    }
                                    else if (SelectedPageNum + 1 < PagesCount)
                                    {
                                        SelectedPageNum++;
                                        SendReadAsk(pack.UnitID, pack.CanID, (byte)SelectedPageNum);
                                        return;
                                    }
                                    else
                                    {
                                        //
                                        // If all pages are occupied, then the first inactive page is taken and cleared
                                        //


                                        SelectedPageNum = 2;
                                        if (SelectedPageNum == ActualPageNum)
                                            SelectedPageNum++;

                                        SendCleardAsk(pack.UnitID, pack.CanID, (byte)SelectedPageNum);

                                        EmptyPageFound = true;
                                        Step++;
                                        return;
                                    }


                                }
                                else
                                {
                                    LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPStateAns PackStateAns = pack as LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPStateAns;
                                    if (PackStateAns != null)
                                    {
                                        ActualPageNum = PackStateAns.IAPState.PageCur;
                                        PagesCount = PackStateAns.IAPState.PageCount;

                                        if (SelectedPageNum == ActualPageNum)
                                        {
                                            SelectedPageNum++;
                                            SendReadAsk(pack.UnitID, pack.CanID, (byte)SelectedPageNum);
                                            return;
                                        }
                                        else
                                        {
                                            SendReadAsk(pack.UnitID, pack.CanID, (byte)SelectedPageNum);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Ans is null");
                                    }
                                }
                            }

                            //
                            // Load firmware
                            //
                            if (Step == 2)
                            {


                                if (FirstWriteFlag)
                                {
                                    SendWriteAsk(OcOutdatesDevs[DevUpdate].Unit, (byte)OcOutdatesDevs[DevUpdate].CAN, (byte)SelectedPageNum);
                                    FirstWriteFlag = !FirstWriteFlag;

                                    Dispatcher.Invoke(new Action(() =>
                                    {
                                        PbMain.Value++;
                                        ElapsedTime(_updateStartTime);
                                    }));

                                    return;
                                }
                                else
                                {
                                    Dispatcher.Invoke(new Action(() =>
                                    {
                                        PbMain.Value++;
                                        ElapsedTime(_updateStartTime);
                                    }));
                                    FirmwareLoadPackAns = pack as LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPWriteAns;
                                    Console.WriteLine("Offset: " + FirmwareLoadPackAns.Offset);
                                    int Pos = FirmwareLoadPackAns.Offset * 32 / PartSize;
                                    int Offset = FirmwareLoadPackAns.Offset;
                                    SendLastFragFlag = Pos == ListOfFWList[ActualFWNum].Count - 1;




                                    if (Pos > ListOfFWList[ActualFWNum].Count)
                                    {
                                        Step++;
                                        throw new Exception("Not found");
                                    }

                                    Driver.SendPack(new LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPWriteAsk()
                                    {
                                        Num = (byte)SelectedPageNum,
                                        UnitID = pack.UnitID,

                                        Fragment = new LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPWriteAsk.FragmentPageSturct()
                                        {
                                            Buff = ListOfFWList[ActualFWNum][Pos],
                                            Offset = (short)Offset,
                                            isLastFrag = SendLastFragFlag
                                        },
                                        CanID = pack.CanID
                                    });


                                    if (SendLastFragFlag)
                                    {
                                        Dispatcher.Invoke(new Action(() =>
                                        {

                                            PbMain.Value++;
                                            ElapsedTime(_updateStartTime);

                                        }));
                                        SendLastFragFlag = false;
                                        Step++;
                                        return;
                                    }
                                }
                            }

                            //
                            // Update page
                            //

                            if (Step == 3)
                            {
                                SendUpdateAsk(pack.UnitID, pack.CanID, (byte)SelectedPageNum);
                                Step++;
                                return;
                            }

                            //
                            // Checking the correctness of the data
                            //

                            if (Step == 4)
                            {
                                if (pack is LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPUpdateAns)
                                {
                                    SendReadAsk(pack.UnitID, pack.CanID, (byte)SelectedPageNum);
                                    return;
                                }

                                if (pack is LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPReadAns)
                                {
                                    LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPReadAns PackReadAns = pack as LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPReadAns;

                                    if (PackReadAns.Data[3] == 170)
                                    {
                                        SendActivateAsk(pack.UnitID, pack.CanID, (byte)SelectedPageNum);
                                        Step++;
                                        DevUpdate++;
                                        RebootingTheDeviceFlag = true;
                                        return;
                                    }
                                }
                                else
                                {
                                    SendReadAsk(pack.UnitID, pack.CanID, (byte)SelectedPageNum);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
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
            BtnUpdate.IsEnabled = false;
            BtnUpdate.Style = (Style)Styles[styleUpdateDisable]; ListOfFWList.Clear();
            OcOutdatesDevs.Clear();
            UpdateFlag = true;
            Driver.OnDevChange = null;
            AnalyzeFlag = false;
            DevUpdate = 0;
            ActualFWNum = 0;
            ListOfFWList = new List<List<byte[]>>();


            if (OnlineActive)
                OnlineUpdate(Path.Combine(FwZipPath, FwZipName));
            else if (OfflineActive)
            {
                if (OfflineArchiveFlag)
                {
                    OfflineArchiveFlag = !OfflineArchiveFlag;
                    OnlineUpdate(FwArchivePath);
                    return;
                }
                WndOfflineMode wndOfflineMode = new WndOfflineMode();
                wndOfflineMode.ShowDialog();

                if (FwArchivePath != "null")
                {


                    try
                    {
                        LbState.Content = "Разбор архива микропрограмм";
                        checkForUpdate = new FWCheckForUpdate(new string[] { FwArchivePath });
                        AnalyzeFlag = true;
                        UpdateFlag = false;
                        _analyzeStartTime = DateTime.Now;
                        PbMain.Maximum = 1;
                        PbMain.Value = 0;
                        LbState.Content = "Анализ";
                        LbProgressPercent.Content = "0%";
                        OcLiftFwData.Clear();
                        OcAllDevFwData.Clear();
                        foreach (var dev in Driver.Devices)
                        {
                            var keys = dev.SubDevices.Keys;
                            var Unit = dev.UnitID;

                            dev.SendPack(new PackV7WhoAsk());

                            foreach (var key in keys)
                                SendWhoAsk(Unit, key);
                        }
                        OfflineArchiveFlag = !OfflineArchiveFlag;
                        Driver.OnDevChange += new DeviceV7.OnDevChangeDelegate(Driver_OnDevChange);
                    }
                    catch 
                    {
                        MessageBox.Show("Не удалось разобрать архив.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Error) ;
                        Driver.OnDevChange += new DeviceV7.OnDevChangeDelegate(Driver_OnDevChange);
                        BtnUpdate.IsEnabled = true;
                        BtnUpdate.Style = (Style)Styles[styleUpdateEnable];
                        return;
                    }
                }


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

        private void LvUpdateInfo_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                wndUpdateDetail = new WndUpdateDetail();
                var res = new ObservableCollection<FirmwareUpdate>(OcLiftFwDataUpdate.Where(p => p.Unit == (LvUpdateInfo.SelectedItem as FirmwareUpdate).Unit));
                wndUpdateDetail.UpdateData(ref res);
                wndUpdateDetail.ShowDialog();
                wndUpdateDetail = null;
            }
            catch { }
        }

        #region BtnFilt

        private void BtnTitleFilt_Click(object sender, RoutedEventArgs e)
        {
            currentFilt = FilterAnalysis.Title;
            UpdateAnalysisData(currentFilt, PageCharts.currentChart);
            GetUpdatingData(currentFilt);
            (sender as Button).Foreground = (Brush)new BrushConverter().ConvertFrom("#486FEF");
        }

        private void BtnFwFilt_Click(object sender, RoutedEventArgs e)
        {
            currentFilt = FilterAnalysis.FwVersion;
            UpdateAnalysisData(currentFilt, PageCharts.currentChart);
            GetUpdatingData(currentFilt);
            (sender as Button).Foreground = (Brush)new BrushConverter().ConvertFrom("#486FEF");
        }

        private void BtnDateFilt_Click(object sender, RoutedEventArgs e)
        {
            currentFilt = FilterAnalysis.Date;
            UpdateAnalysisData(currentFilt, PageCharts.currentChart);
            GetUpdatingData(currentFilt);
            (sender as Button).Foreground = (Brush)new BrushConverter().ConvertFrom("#486FEF");
        }

        private void BtnErrorTypeFilt_Click(object sender, RoutedEventArgs e)
        {
            currentFilt = FilterAnalysis.ErrorType;
            UpdateAnalysisData(currentFilt, PageCharts.currentChart);
            GetUpdatingData(currentFilt);
            (sender as Button).Foreground = (Brush)new BrushConverter().ConvertFrom("#486FEF");
        }

        #endregion

        private void BtnSaveXlsx_Click(object sender, RoutedEventArgs e)
        {
            WndSaveXlsx wndSaveXlsx = new WndSaveXlsx();
            wndSaveXlsx.ShowDialog();
        }

        private void PbMain_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            /*if (IsUpdateFW)
            {
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
            }*/
            

        }

        private void timerTick(object sender, EventArgs e)
        {
            UpdateProgressBarMax();

        }

        #endregion

        #region Functions & procedures


        void OnlineUpdate(string path)
        {
            if (OcAllDevFwData.Count(p => p.StatusCode == FirmwareErrorType.Outdated) == 0)
            {
                if (OfflineArchiveFlag == true)
                    OfflineArchiveFlag = false;

                Driver.OnDevChange += new DeviceV7.OnDevChangeDelegate(Driver_OnDevChange);
                BtnUpdate.IsEnabled = true;
                BtnUpdate.Style = (Style)Styles[styleUpdateEnable];
                return;
            }


            LvUpdateInfo.Visibility = Visibility.Visible;
            LvAnalysisInfo.Visibility = Visibility.Hidden;



            PbMain.Value = 0;
            PbMain.Maximum = 1;
            LbState.Content = "Обновление";
            LbProgressPercent.Content = "0%";

            List<uint> UnitsID = new List<uint>();

            for (int i = 0; i < OcAllDevFwData.Count; i++)
            {
                if (OcAllDevFwData[i].StatusCode == FirmwareErrorType.Outdated)
                {
                    UnitsID.Add(OcAllDevFwData[i].Unit);
                    FirmwareUpdate firmwareUpdate = new FirmwareUpdate(OcOutdatesDevs.Count + 1, OcAllDevFwData[i], $"{statustxt} {i * 5} минут", FirmwareUpdateStatus.InProcess, FirmwareUpdateStatus.InProcess.GetNameOfEnum());
                    OcOutdatesDevs.Add(firmwareUpdate);
                }
               
            }
            OcOutdatesDevs = new ObservableCollection<FirmwareUpdate>(OcOutdatesDevs.OrderBy(p => p.Title));
            UnitsID = UnitsID.Distinct().ToList();
            GetUpdatingData(currentFilt);


            FirstWriteFlag = true;
            EmptyPageFound = false;

            List<byte[]> AllFwBytes = new List<byte[]>();

            for (int i = 0; i < OcOutdatesDevs.Count; i++)
            {
                string[] fwpath = OcOutdatesDevs[i].FileDescriptor.Name.Split('>');

                AllFwBytes.Add(FindAndReadFWFromZip(fwpath[1],path));
            }

            FileDivision(AllFwBytes);

            Step = 1;
            int PbMainMax = 0;

            foreach (var item in ListOfFWList)
            {
                PbMainMax += item.Count;
            }

            timer.Tick -= timerTick;
            PbMain.Maximum = PbMainMax;

            _updateStartTime = DateTime.Now;

            SendStateAsk(OcOutdatesDevs[DevUpdate].Unit, (byte)OcOutdatesDevs[DevUpdate].CAN);
        }

        void ElapsedTime(DateTime startTime)
        {
            Dispatcher.Invoke(() => 
            {
                var elapsedTime = (DateTime.Now - startTime).TotalSeconds;
                var allTimeFordownloading = elapsedTime * PbMain.Maximum / PbMain.Value;
                var remainingTime = allTimeFordownloading - elapsedTime;
                var ts = TimeSpan.FromSeconds(remainingTime);


                var step = 0;
                foreach (var item in OcOutdatesDevs)
                    if (item.Status == FirmwareUpdateStatus.Updated)
                        step++;

                var speedLoad = PbMain.Value / elapsedTime;
                for (int i = step; i < OcOutdatesDevs.Count; i++)
                {
                    var fragsCount = ListOfFWList[step].Count;
                    for (int j = 1; j <= i; j++)
                    {
                        fragsCount += ListOfFWList[j].Count;
                    }

                    var LocalTs = TimeSpan.FromSeconds((fragsCount - PbMain.Value) / speedLoad);

                    if (i == OcOutdatesDevs.Count - 1)
                        LocalTs = ts;

                   /* var allTimeFordownloadingLocal = elapsedTime * ListOfFWList[step].Count / PbMain.Value;
                    var remainingTimeLocal = allTimeFordownloadingLocal - elapsedTime;
                    var time = remainingTime - remainingTimeLocal;
                    var LocalTs = TimeSpan.FromSeconds(time);
                    

                    if (i == OcOutdatesDevs.Count-1)
                    {
                        LocalTs = TimeSpan.FromSeconds(remainingTime);
                    }*/

                    if (LocalTs.Hours > 0)
                    {
                        OcOutdatesDevs[i].StatusTxt = $"{statustxt} {LocalTs.Hours}ч {LocalTs.Minutes}мин";
                    }
                    else if (LocalTs.Minutes > 0)
                    {
                        OcOutdatesDevs[i].StatusTxt = $"{statustxt} {LocalTs.Minutes}мин {LocalTs.Seconds}сек";
                    }
                    else if (LocalTs.Seconds < 0)
                    {
                        OcOutdatesDevs[i].StatusTxt = $"Активация...";
                    }
                    else
                    {
                        OcOutdatesDevs[i].StatusTxt = $"{statustxt} {LocalTs.Seconds} сек";
                    }
                }

                if (ts.Hours > 0)
                    LbRemainingTime.Content = $"{ts.Hours}ч {ts.Minutes}мин";
                else if (ts.Minutes > 0)
                    LbRemainingTime.Content = $"{ts.Minutes}мин {ts.Seconds}сек";
                else
                    LbRemainingTime.Content = $"{ts.Seconds} сек";


                LbProgressPercent.Content = Convert.ToInt32(PbMain.Value * 100 / PbMain.Maximum) + "%";
            });
            
        }

        byte[] FindAndReadFWFromZip(string entry, string pathZip)
        {
            using (ZipArchive zipArchive = ZipFile.OpenRead(pathZip))
            {
                foreach (ZipArchiveEntry zipArchiveEntry in zipArchive.Entries)
                {
                    if (zipArchiveEntry.ToString().Contains(entry))
                    {
                        Stream stream = zipArchiveEntry.Open();
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            stream.CopyToAsync(memoryStream);

                            var bytes = memoryStream.ToArray();

                            while (bytes.Count() != zipArchiveEntry.Length) 
                            {
                                bytes = memoryStream.ToArray();
                            }

                            return bytes;
                        }
                    } 
                }
            }
            return null;
        }

        void FileDivision(List<byte[]> FwsBytes)
        {
            foreach (var fw in FwsBytes)
            {
                int ActualPosition = 0;
                FirmwareFragments = new List<byte[]>();
                for (int i = 0; i < fw.Length; i += PartSize)
                {
                    byte[] PartOfFile = new byte[PartSize];

                    if (PartSize * FirmwareFragments.Count + PartSize > fw.Length)
                    {
                        try
                        {
                            PartOfFile = new byte[fw.Length - PartSize * FirmwareFragments.Count];

                            for (int j = 0; j < PartOfFile.Length; j++)
                            {
                                PartOfFile[j] = fw[ActualPosition++];
                            }
                        }
                        catch { }
                    }
                    else
                    {
                        for (int j = 0; j < PartSize && ActualPosition != fw.Length; j++)
                        {
                            PartOfFile[j] = fw[ActualPosition++];
                        }
                    }
                    FirmwareFragments.Add(PartOfFile);
                }
                ListOfFWList.Add(FirmwareFragments);
            }
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
                res = new ObservableCollection<FirmwareAnalysis>(res.Where(p => p.StatusCode == FirmwareErrorType.Error));
            else if (chart == ChartsCodes.Outdate)
                res = new ObservableCollection<FirmwareAnalysis>(res.Where(p => p.StatusCode == FirmwareErrorType.Outdated));
            else if (chart == ChartsCodes.Actual)
                res = new ObservableCollection<FirmwareAnalysis>(res.Where(p => p.StatusCode == FirmwareErrorType.Actual));


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

        public void GetUpdatingData(FilterAnalysis code)
        {
            var res = OcOutdatesDevs;

            if (code == FilterAnalysis.Title)
                res = new ObservableCollection<FirmwareUpdate>(res.OrderBy(p => p.Title));
            else if (code == FilterAnalysis.FwVersion)
                res = new ObservableCollection<FirmwareUpdate>(res.OrderBy(p => p.Version));
            else if (code == FilterAnalysis.Date)
                res = new ObservableCollection<FirmwareUpdate>(res.OrderBy(p => p.Date));
            else
                res = new ObservableCollection<FirmwareUpdate>(res.OrderBy(p => p.ErrorType));

            Dispatcher.Invoke(() => { LvUpdateInfo.ItemsSource = res; });

        }

        void UpdateView(VirtualDeviceV7 dev)
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    LbDevCount.Content = OcAllDevFwData.Where(p => p.StatusCode == FirmwareErrorType.Outdated).Count();
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
            try
            {
                bool CheckDuplicate = false;
                bool CheckLiftDuplicate = false;


                // Если у ЛБ в слейвах есть устаревшие прошивки, то лб относится к типу "Устаревшее", но прошивка является актуальной

                /*if (firmwareAnalysis.ErrorType == FirmwareErrorType.Outdated && !(pack.Device is DeviceV7))
                {
                    var devsLift = OcLiftFwData.Where(p => p.CAN == 0).ToList();
                    foreach (var item in devsLift)
                        OcLiftFwData[item.ID].ErrorType = FirmwareErrorType.Outdated;

                }*/

                if (pack.Device is DeviceV7)
                {

                    foreach (var item in OcLiftFwData)
                        if (item.Unit == pack.UnitID)
                            CheckLiftDuplicate = true;


                    if (!CheckLiftDuplicate)
                        OcLiftFwData.Add(firmwareAnalysis);

                }
                foreach (var item in OcAllDevFwData)
                    if (item.CAN == firmwareAnalysis.CAN
                        && item.FileDescriptor.IFD.Descriptor.ToString() == firmwareAnalysis.FileDescriptor.IFD.Descriptor.ToString())
                        CheckDuplicate = true;

                if (!CheckDuplicate)
                    OcAllDevFwData.Add(firmwareAnalysis);
            }
            catch
            {

            }
           
        }

        void UpdateProgressBarMax()
        {
            Dispatcher.Invoke((Action)(() =>
            {
                if (!AnalyzeFlag)
                    return;
                if (this.UpdateFlag)
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

                if (PbMain.Maximum != 1)
                {
                    PbMain.Maximum = PbMax;
                    return;
                }

                if (PbMax - 1 != 0)
                    PbMain.Maximum = PbMax - 1;
                else
                    PbMain.Maximum = PbMax;

               
            }));
        }

        public string GetFWVer(ProcessorStateStruct proc)
        {
            return $"{proc.Ver.Byte3}.{proc.Ver.Byte2}.{proc.Ver.Byte1}.{proc.Ver.Byte0}";
        }


        public static bool ConnectionAvailable(string strServer)
        {
            try
            {
                HttpWebRequest httpReq = (HttpWebRequest)HttpWebRequest.Create(strServer);
                HttpWebResponse httpWeb = (HttpWebResponse)httpReq.GetResponse();

                if (HttpStatusCode.OK == httpWeb.StatusCode)
                {
                    // HTTP = 200 - Интернет безусловно есть!
                    httpWeb.Close();
                    Console.WriteLine("Соединения с интернетом присутствует");
                    return true;
                }
                else
                {
                    // сервер вернул отрицательный ответ, возможно что инета нет
                    httpWeb.Close();
                    Console.WriteLine("Соединения с интернетом отсутствует, либо трафик сети перегружен");
                    return false;
                }
            }
            catch (WebException)
            {
                Console.WriteLine("Соединения с интернетом отсутствует");
                return false;
            }
        }



        #endregion

        #region Send methods

        void SendStateAsk(uint UnitID, byte CanId)
        {
            Driver.SendPack(new LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPStateAsk()
            {
                UnitID = UnitID,
                CanID = CanId,
            });
        }

        void SendReadAsk(uint UnitID, byte CanId, byte SelectedPageNum)
        {
            Driver.SendPack(new LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPReadAsk()
            {
                UnitID = UnitID,
                Num = (byte)SelectedPageNum,
                CanID = CanId
            });
        }

        void SendCleardAsk(uint UnitID, byte CanId, byte SelectedPageNum)
        {
            Driver.SendPack(new LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPClearAsk()
            {
                UnitID = UnitID,
                Num = SelectedPageNum,
                CanID = CANID
            });

        }

        void SendWriteAsk(uint UnitID, byte CanId, byte SelectedPageNum)
        {
            if (ListOfFWList.Count > 0)
            {
                Driver.SendPack(new LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPWriteAsk()
                {
                    UnitID = UnitID,
                    Num = SelectedPageNum,
                    Fragment = new LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPWriteAsk.FragmentPageSturct()
                    {
                        Buff = ListOfFWList[ActualFWNum][FirstFragmentNum],
                        Offset = FirstFragmentNum,
                        isLastFrag = FlagForFirstFragment
                    },
                    CanID = CanId
                });
            }
        }

        void SendUpdateAsk(uint UnitID, byte CanId, byte SelectedPageNum)
        {

            Driver.SendPack(new LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPUpdateAsk()
            {
                UnitID = UnitID,
                Num = (byte)SelectedPageNum,
                CanID = CanId
            });
        }

        void SendActivateAsk(uint UnitID, byte CanId, byte SelectedPageNum)
        {
            Driver.SendPack(new LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPActiveAsk()
            {
                UnitID = UnitID,
                Num = SelectedPageNum,
                CanID = CanId
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


        #endregion

        
    }

}
