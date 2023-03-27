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

        public static DriverV7 Driver = new DriverV7();
        LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPWriteAns FirmwareLoadPackAns;
        SubDeviceV7 FocusedDev;
        SubDeviceV7 opt = null;
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
        private string FirmwareXMLPath = AppDomain.CurrentDomain.BaseDirectory + "\\Firmwares\\";

        const string FwXMLToParseName = "firmware.xml";
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

        Stopwatch stopwatch = new Stopwatch();

        ResourceDictionary Styles = (ResourceDictionary)Application.LoadComponent(
            new Uri("/MultiProg7;component/ResourceDictionaries/Styles.xaml", UriKind.Relative));

        public ObservableCollection<VMDevice> OcVMDev;


        public static FWForDevice FwFromManualMode = null;
        #endregion


        public static FilterAnalysis currentFilt;

       

        public static ObservableCollection<FirmwareAnalysis> ocTest = new ObservableCollection<FirmwareAnalysis>();

        #endregion

        #region Page Events
        public PageMain()
        {
            InitializeComponent();
            Random random = new Random();
            FirmwareAnalysis firmwareAnalysis;
            for (int i = 0; i < 100; i++) 
            {
                int a = random.Next(1, 4);
                firmwareAnalysis = new FirmwareAnalysis(i)
                {
                    CAN = new Random().Next(100),
                    Title = "Строка " + i,
                    Version = random.Next(100),
                    Date = DateTime.Now,
                };
                if (a == 1)
                    firmwareAnalysis.StatusCode = FirmwareStatus.Error;
                else if (a == 2)
                    firmwareAnalysis.StatusCode = FirmwareStatus.Actual;
                else
                    firmwareAnalysis.StatusCode = FirmwareStatus.Outdated;
                firmwareAnalysis.StatusTxt = "Статус " + firmwareAnalysis.StatusCode;
                
                ocTest.Add(firmwareAnalysis);



            }

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Driver.OnSubDevChange += Driver_OnSubDevChange;
            Driver.OnReceiveData += Driver_OnReceiveData;
            if (!Driver.Init())
            {
                return;
            }

            if (App.Args.Length != 0)
            {
                try
                {
                    var Devices = DeviceV7.FromArgs(App.Args);
                    Driver.AddDevice(ref Devices[0]);
                    opt = Devices[0];
                    CANID = opt.CanID;
                    Devices[0].WorkMode = DeviceV7.WorkModeType.LogExtra;
                }
                catch { }
            }
            PageCharts pageCharts = new PageCharts(ocTest);
            pageCharts.pageMain = this;
            OcVMDev = new ObservableCollection<VMDevice>();
            FrameChartsMain.Navigate(pageCharts);
            UpdateAnalysisData(currentFilt, PageCharts.currentChart);

        }

        #endregion

        #region Modes
        private void BtnOnlineMode_Click(object sender, RoutedEventArgs e) => ChangeMode(ModesCodes.Online);

        private void BtnOfflineMode_Click(object sender, RoutedEventArgs e) => ChangeMode(ModesCodes.Offline);

        private void BtnManualMode_Click(object sender, RoutedEventArgs e) => ChangeMode(ModesCodes.Manual);
        #endregion

        #region LKDSFramework Events
        private void Driver_OnReceiveData(PackV7 pack)
        {
            if (FocusedDev != null)
            {
                if (pack.CanID != FocusedDev.CanID)
                {
                    return;
                }
            }

            if (RebootingTheDeviceFlag)
            {
                if (ActualFWNum == ListOfFWList.Count - 1)
                {
                    Dispatcher.Invoke(() => { LbState.Content = labelContentAnalyze; });
                    FinishFlag = true;
                    RebootingTheDeviceFlag = !RebootingTheDeviceFlag;
                    return;
                }
                else
                {
                    Dispatcher.Invoke(() => { LbState.Content = labelContentAnalyze; });
                    FinishFlag = false;
                    RebootingTheDeviceFlag = !RebootingTheDeviceFlag;
                    ActualFWNum++;
                    return;
                }

            }

            if (pack is LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPProcessorAns)
            {
                if (pack.CanID != CANID)
                    return;

                if (pack.CanID != SelectedDevOrSubDevCANID)
                    return;

                LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPProcessorAns PackProcessorAns = pack as LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPProcessorAns;
                foreach (FirmwareInfo FWInfo in FirmwareInfo.firmwares)
                    foreach (ProcessorStateStruct PSS in PackProcessorAns.Proccessors)
                        if (FWInfo.deviceClass.Equals(PSS.Aclass))
                        {
                            Console.WriteLine("Class: " + FWInfo.deviceClass);
                            if (FWInfo.devicePlatform.Equals(PSS.Bplatform))
                            {
                                Console.WriteLine("Platform: " + FWInfo.devicePlatform);

                                if (FWInfo.deviceType.Equals(PSS.Cdevtype))
                                {
                                    Console.WriteLine("Processor: " + FWInfo.deviceType);
                                    if (FWInfo.deviceProcessor.Equals(PSS.DNum))
                                    {
                                        Console.WriteLine("Num: " + FWInfo.deviceProcessor);
                                        if (FWInfo.localization.Equals(PSS.Elocal))
                                        {
                                            Console.WriteLine("Localization: " + FWInfo.localization);

                                            string[] URLFrags = FWInfo.url.Split('/');
                                            var m = Regex.Matches(URLFrags[URLFrags.Length - 1], @"\d+,?\d+").Cast<Match>();
                                            string[] VerFragments = FWInfo.version.ToString().Split('.');
                                            string CurrentVer = "";

                                            foreach (string VerFrag in VerFragments)
                                            {
                                                CurrentVer += VerFrag;
                                            }

                                            string CurrentDevVer = "";

                                            for (int i = PSS.Ver.ToArray.Length - 1; i >= 0; i--)
                                            {
                                                CurrentDevVer += PSS.Ver.ToArray[i];
                                            }

                                            if (Convert.ToInt32(CurrentVer) > Convert.ToInt32(CurrentDevVer))
                                            {
                                                FirmwareFile = Path.Combine(FirmwareXMLPath, URLFrags[URLFrags.Length - 1]);
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
                                                FirmwaresToDownload.Add(new FWForDevice(Convert.ToInt32(CurrentVer), FWInfo.url, FirmwareFile, URLFrags[URLFrags.Length - 1]));
                                                continue;
                                            }
                                            else
                                            {

                                                FileInfo[] files = new DirectoryInfo(FirmwareXMLPath).GetFiles(URLFrags[URLFrags.Length - 1]);

                                                if (files.Length == 0)
                                                {
                                                    FirmwareFile = Path.Combine(FirmwareXMLPath, URLFrags[URLFrags.Length - 1]);
                                                    FirmwaresToDownload.Add(new FWForDevice(Convert.ToInt32(CurrentVer), FWInfo.url, FirmwareFile, URLFrags[URLFrags.Length - 1]));
                                                    continue;
                                                }
                                                else if (files[0].CreationTime < Convert.ToDateTime(FWInfo.date))
                                                {

                                                    FirmwareFile = Path.Combine(FirmwareXMLPath, URLFrags[URLFrags.Length - 1]);

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

                                                    continue;

                                                }
                                                else
                                                {
                                                    MessageBox.Show("Версия актуальна");
                                                    ResetProgressBar(1);
                                                    FinishFlag = false;
                                                    ActualFlag = true;
                                                    continue;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                if (FirmwaresToDownload.Count == 0 && ActualFlag)
                {
                    //Environment.ExitCode = 0;
                    /*Dispatcher.Invoke(new Action(() =>
                    {
                        *//*LBInfo.Hide();
                        LoadFirmwareBTN.Hide();
                        progressBar2.Hide();
                        ResultLB.Text = "  Версия \nактуальна";
                        LBTimeRes.Visible = false;
                        ResultLB.Show();
                        TimerToClose.Start();*//*
                    }));*/
                    //MessageBox.Show("Версия актуальна");
                    ResetProgressBar(1);
                    FinishFlag = true;

                    return;
                }

                PreparingTheFile();

                return;
            }

            if (pack is LKDSFramework.Packs.DataDirect.PackV7IAPService)
            {
                LKDSFramework.Packs.DataDirect.PackV7IAPService IAPAns = (LKDSFramework.Packs.DataDirect.PackV7IAPService)pack;
                if (IAPAns.Error != LKDSFramework.Packs.DataDirect.PackV7IAPService.IAPErrorType.NoError)
                    Console.WriteLine("Error");
                
                else
                {
                    if (IsUpdateFW)
                    {
                        //
                        // Searching for an empty page
                        //

                        if (Step == 1)
                        {
                            try
                            {
                                Dispatcher.Invoke(() => { LbState.Content = labelContentUpdate; });
                            }
                            catch { }

                            if (pack is LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPReadAns && !EmptyPageFound)
                            {
                                LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPReadAns PackReadAns = pack as LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPReadAns;
                                if (PackReadAns.Data[3] == 85)
                                {
                                    EmptyPageFound = true;
                                    SendCleardAsk();
                                    Step++;
                                    return;
                                }
                                else if (SelectedPageNum + 1 < PagesCount)
                                {
                                    SelectedPageNum++;
                                    SendReadAsk();
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

                                    SendCleardAsk();

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
                                        SendReadAsk();
                                        return;
                                    }
                                    else
                                    {
                                        SendReadAsk();
                                        return;
                                    }
                                }
                                else
                                    Console.WriteLine("Ans is null");
                            }
                        }

                        //
                        // Load firmware
                        //
                        if (Step == 2)
                        {

                            if (FirstWriteFlag)
                            {
                                Dispatcher.Invoke(() => { PbMain.Value++; });

                                SendWriteAsk();
                                FirstWriteFlag = !FirstWriteFlag;

                                stopwatch.Start();
                                return;

                            }
                            else
                            {
                                stopwatch.Stop();
                                FirmwareLoadPackAns = pack as LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPWriteAns;
                                Console.WriteLine("Offset: " + FirmwareLoadPackAns.Offset);
                                int Pos = FirmwareLoadPackAns.Offset * 32 / PartSize;
                                int Offset = FirmwareLoadPackAns.Offset;
                                SendLastFragFlag = Pos == ListOfFWList[ActualFWNum].Count - 1;

                                /*TimeSpan Ts = stopwatch.Elapsed;
                                Time = ((ListOfFWList[ActualFWNum].Count - Pos) * (long)(Ts.Milliseconds)) / 1000;

                                long min = Time / 60;
                                long sec = Time - min * 60;*/


                                Dispatcher.Invoke(() =>
                                {
                                    /*if (min == 0)
                                    {
                                        sec = Time;
                                        LBTimeRes.Text = $"{sec} {InfoStrs[2]}";

                                    }
                                    else
                                        LBTimeRes.Text = $"{min} {InfoStrs[3]} {sec} {InfoStrs[2]}";
                                    */
                                    PbMain.Value++;

                                });

                                if (Pos > ListOfFWList[ActualFWNum].Count)
                                {
                                    Step++;
                                    throw new Exception("Not found");
                                }

                                Driver.SendPack(new LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPWriteAsk()
                                {
                                    Num = (byte)SelectedPageNum,
                                    UnitID = opt.UnitID,

                                    Fragment = new LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPWriteAsk.FragmentPageSturct()
                                    {
                                        Buff = ListOfFWList[ActualFWNum][Pos],
                                        Offset = (short)Offset,
                                        isLastFrag = SendLastFragFlag
                                    },
                                    CanID = CANID
                                });


                                if (SendLastFragFlag)
                                {
                                    Dispatcher.Invoke(() => { PbMain.Value = PbMain.Maximum; });
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
                            SendUpdateAsk();
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
                                SendReadAsk();
                                return;
                            }

                            if (pack is LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPReadAns)
                            {
                                LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPReadAns PackReadAns = pack as LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPReadAns;

                                if (PackReadAns.Data[3] == 170)
                                {
                                    SendActivateAsk();
                                    Step++;
                                    return;
                                }
                            }
                            else
                            {
                                SendReadAsk();
                                return;
                            }
                        }
                        if (Step == 5)
                        {
                            if (pack is LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPActiveAns)
                            {
                                LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPActiveAns PackActiveAns = pack as LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPActiveAns;
                                if (PackActiveAns.Error == LKDSFramework.Packs.DataDirect.PackV7IAPService.IAPErrorType.NoError)
                                {
                                    Console.WriteLine("/-/-/-/-/-/ Done /-/-/-/-/-/");
                                    RebootingTheDeviceFlag = true;
                                    IsUpdateFW = false;
                                }
                                else
                                {
                                    FinishFlag = true;
                                    Console.WriteLine("/-/-/-/-/-/ Error /-/-/-/-/-/");
                                    IsUpdateFW = false;
                                }
                            }
                        }
                    } 
                    else
                    {
                        if (IAPAns is LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPStateAns)
                        {
                            LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPStateAns PackStateAns = IAPAns as LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPStateAns;
                            FileExt = "*.b" + PackStateAns.IAPState.AppVer.ToString("X2");
                            FwOnPages.Clear();
                        }
                        else if (IAPAns is LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPReadAns)
                        {
                            LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPReadAns packV7IAPReadAns = IAPAns as LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPReadAns;
                            FwOnPages.Add(packV7IAPReadAns.PageState.Name);


                            VMDevice VMDev = null;
                            Dispatcher.Invoke(() =>
                            {
                                stopwatch.Start();
                                progressValue++;
                                if (progressValue > PbMain.Maximum)
                                    return;

                                string FWVer = "";
                                char[] FWName = packV7IAPReadAns.PageState.Name.ToCharArray();
                                for (int i = FWName.Length - 1; i > 0; i--)
                                {
                                    if (Char.IsDigit(FWName[i]) || FWName[i].Equals('0'))
                                    {
                                        FWVer += FWName[i];
                                    }
                                    else
                                    {
                                        try
                                        {
                                            int a = (int)FWName[i];
                                            if (a.Equals(32))
                                            {
                                                continue;
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                        catch
                                        {
                                            break;
                                        }
                                    }
                                }
                                FWName = FWVer.Reverse().ToArray();
                                FWVer = "";

                                foreach (char ch in FWName)
                                {
                                    FWVer += ch + ".";
                                }
                                FWVer = FWVer.Trim();
                                SubDeviceV7 dev;
                                if (pack.CanID.Equals(0) && LBCheck)
                                {
                                    return;
                                }
                                else
                                {
                                    dev = Driver.Devices[0];

                                    LBCheck = true;
                                }
                                if (!pack.CanID.Equals(0))
                                {
                                    dev = Driver.Devices[0].SubDevices[pack.CanID];
                                }

                                VMDev = new VMDevice()
                                {
                                    CanID = dev.CanID,
                                    Title = dev.ToString(),
                                    AppVer = dev.AppVer,
                                    FWTitle = packV7IAPReadAns.PageState.Name,
                                    FWVersion = FWVer,
                                    FWDate = "Не определено"
                                };

                                stopwatch.Stop();
                                TimeSpan Ts = stopwatch.Elapsed;
                                time = CounterDevSubdev - progressValue * (long)(Ts.Milliseconds) / 1000;

                                long min = time / 60;
                                long sec = time - min * 60;
                                OcVMDev.Add(VMDev);
                                //LbRemainingTime.Content = $"{min}мин{sec}сек";
                                if (UpdateListView)
                                {
                                    UpdateListView = false;
                                    //  LVCanDevList.ItemsSource = OcVMDev;
                                }
                                LbDevCount.Content = CounterDevSubdev.ToString();
                                //LVCanDevList.Items.Refresh();
                                PbMain.Value++;
                            });



                        }
                    }
                }
            }
        }

        private void Driver_OnSubDevChange(SubDeviceV7 dev)
        {
            try
            {

                Dispatcher.Invoke((Action)(() =>
                {
                    FWGet = true;

                    VMDevice.SendReadAsk(dev.CanID);

                    CounterDevSubdev++;

                    PbMain.Maximum = CounterDevSubdev;


                    CheckAnalyze = true;
                }));

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
            /*BtnUpdate.IsEnabled = false;
            BtnUpdate.Style = (Style)Styles[styleUpdateDisable];*/
            FirmwaresToDownload.Clear();
            if (OnlineActive)
            {
                if (!Directory.Exists(FirmwareXMLPath))
                {
                    Directory.CreateDirectory(FirmwareXMLPath);
                }
                bool DownloadFlag = false;
                while (!DownloadFlag)
                {
                    try
                    {
                        using (var client = new WebClient())
                        {
                            client.DownloadFile(@"http://lkds.ru/upload/firmware/firmware.xml", Path.Combine(FirmwareXMLPath, FwXMLToParseName));
                        }
                        DownloadFlag = true;
                    }
                    catch
                    {
                        Console.WriteLine("Connection error");
                        break;
                    }
                }
                ParcingFirmware();
                SendProcessorAsk();


            } 
            else if (OfflineActive)
            {
                // 
                // Подготовка к диплому
                //
                WndOfflineMode wndOfflineMode = new WndOfflineMode();
                wndOfflineMode.ShowDialog();
            }
            else if (ManualActive)
            {
                WndManualMode wndManualMode = new WndManualMode();
                wndManualMode.ShowDialog();

                if (WndManualMode.IsApplied)
                {
                    PreparingTheFile();
                }
            }
        }

        #endregion

        #region Functions & procedures
        void ParcingFirmware()
        {
            StreamReader file = new StreamReader(FirmwareXMLPath + FwXMLToParseName, Encoding.GetEncoding(1251));

            FirmwareInfo.parceFirmware(file.ReadToEnd());

            while (FirmwareInfo.firmwares == null) { }

        }

        void PreparingTheFile()
        {
            FirstWriteFlag = true;
            EmptyPageFound = false;
            Step = 0;
            Dispatcher.Invoke(() => { LbState.Content = labelContentPrepareFile; });

            for (int i = 0; i < FirmwaresToDownload.Count && FirmwaresToDownload[0].name != null; i++)
            {
                if (FirmwaresToDownload.Count > 1)
                {
                    for (int j = 0; j < FirmwaresToDownload.Count; j++)
                    {

                        if (i != j)
                        {
                            string str1 = FirmwaresToDownload[i].path.Substring(0, FirmwaresToDownload[i].path.Length - FirmwaresToDownload[i].ver.ToString().Length - ExpLen);
                            string str2 = FirmwaresToDownload[j].path.Substring(0, FirmwaresToDownload[j].path.Length - FirmwaresToDownload[j].ver.ToString().Length - ExpLen);

                            if (str1.Contains(str2))
                            {
                                if (FirmwaresToDownload[i].ver > FirmwaresToDownload[j].ver)
                                {
                                    bool DownloadFlag = false;
                                    while (!DownloadFlag)
                                    {
                                        try
                                        {
                                            using (var client = new WebClient())
                                            {
                                                client.DownloadFile(FirmwaresToDownload[i].url, FirmwaresToDownload[i].path);
                                            }
                                            DownloadFlag = true;
                                        }
                                        catch { }
                                    }
                                    FirmwaresToDownload.Remove(FirmwaresToDownload[j]);

                                }
                                else
                                {
                                    bool DownloadFlag = false;
                                    while (!DownloadFlag)
                                    {
                                        try
                                        {
                                            using (var client = new WebClient())
                                            {
                                                client.DownloadFile(FirmwaresToDownload[j].url, FirmwaresToDownload[j].path);
                                            }
                                            DownloadFlag = true;
                                        }
                                        catch { }
                                    }
                                    FirmwaresToDownload.Remove(FirmwaresToDownload[i]);
                                }
                            }
                        }
                    }
                }
                else
                {
                    bool DownloadFlag = false;
                    while (!DownloadFlag)
                    {
                        try
                        {
                            using (var client = new WebClient())
                            {
                                client.DownloadFile(FirmwaresToDownload[i].url, FirmwaresToDownload[i].path);
                            }
                            DownloadFlag = true;
                        }
                        catch { }
                    }
                }


            }

            //
            // File Division
            //
            if (FirmwaresToDownload.Count > 0)
            {
                foreach (FWForDevice FWFile in FirmwaresToDownload)
                {
                    FileDivision(FWFile);
                }
            }
            else
            {
                FileDivision(FwFromManualMode);
            }

            ResetProgressBar(ListOfFWList[ActualFWNum].Count);
            
            SendStateAsk();
            Step++;
        }

        void FileDivision(FWForDevice FWFile)
        {
            List<byte[]> FirmwareFragments = new List<byte[]>();
            byte[] SelectedFile;
            try
            {
                SelectedFile = File.ReadAllBytes(FWFile.path);

            }
            catch
            {
                MessageBox.Show("Отсутвует файл");
                FinishFlag = true;
                return;
            }
            int ActualPosition = 0;
            for (int i = 0; i < SelectedFile.Length; i += PartSize)
            {
                byte[] PartOfFile = new byte[PartSize];

                if (PartSize * FirmwareFragments.Count + PartSize > SelectedFile.Length)
                {
                    try
                    {
                        PartOfFile = new byte[SelectedFile.Length - PartSize * FirmwareFragments.Count];

                        for (int j = 0; j < PartOfFile.Length; j++)
                        {
                            PartOfFile[j] = SelectedFile[ActualPosition++];
                        }
                    }
                    catch { }
                }
                else
                {
                    for (int j = 0; j < PartSize && ActualPosition != SelectedFile.Length; j++)
                    {
                        PartOfFile[j] = SelectedFile[ActualPosition++];
                    }
                }
                FirmwareFragments.Add(PartOfFile);
            }
            ListOfFWList.Add(FirmwareFragments);
        }

        void ResetProgressBar(int Max)
        {
            Dispatcher.Invoke(() =>
            {
                PbMain.Value = 0;
                PbMain.Maximum = Max;
            });
        }
        #endregion

        #region Send methods

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

        #endregion


        private void LvAnalysisInfo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WndDetail wndDetail = new WndDetail(ocTest);
            wndDetail.ShowDialog();
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

        public void UpdateAnalysisData(FilterAnalysis code, ChartsCodes chart)
        {
            var res = ocTest;

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
            LvAnalysisInfo.ItemsSource = res;
        }

        private void ChangeMode(ModesCodes mode)
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

        private void PbMain_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (PbMain.Value == PbMain.Maximum)
            {
                BtnUpdate.IsEnabled = true;
                BtnUpdate.Style = (Style)Styles[styleUpdateEnable];
            } else
            {
                BtnUpdate.IsEnabled = false;
                BtnUpdate.Style = (Style)Styles[styleUpdateDisable];
            }
            
        }
    }
   
}
