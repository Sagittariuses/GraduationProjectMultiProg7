using LKDSFramework.FWUpdate;
using Multiprog7.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static LKDSFramework.VirtualDeviceV7;

namespace Multiprog7.Classes
{
    public class FirmwareUpdate : OnPropertyChangedClass
    {
       

        private int _can;
        private string _title;
        private string _fwVersion;
        private DateTime _date;
        private string _stateTxt;
        private string _statusTxt;
        private FirmwareErrorType _errorType;
        private FirmwareUpdateStatus _status;

        public int ID { get; }
        public uint Unit { get; set; }
        public FWFileDescriptor FileDescriptor { get; set; }
        public string DevType { get; set; }
        public int CAN { get => _can; set => SetProperty(ref _can, value); }
        public string Title { get => _title; set => SetProperty(ref _title, value); }
        public string Version { get => _fwVersion; set => SetProperty(ref _fwVersion, value); }
        public DateTime Date { get => _date; set => SetProperty(ref _date, value); }
        public string StateTxt { get => _stateTxt; set => SetProperty(ref _stateTxt, value); }
        public string StatusTxt { get => _statusTxt; set => SetProperty(ref _statusTxt, value); }
        public FirmwareErrorType ErrorType { get => _errorType; set => SetProperty(ref _errorType, value); }
        public FirmwareUpdateStatus Status { get => _status; set => SetProperty(ref _status, value); }
        public string PathDataState { get; set; }
        public string PathData { get; set; }
        public string PathFill { get; set; }

        public FirmwareUpdate(int id, FirmwareAnalysis firmwareAnalysis, string statusTxt, FirmwareUpdateStatus status, string pathDataSate)
        {
            ID = id;
            _can = firmwareAnalysis.CAN;
            _title = firmwareAnalysis.Title;
            _fwVersion = firmwareAnalysis.Version;
            _date = firmwareAnalysis.Date;
            _stateTxt = firmwareAnalysis.StatusTxt;
            _statusTxt = statusTxt;
            _errorType = firmwareAnalysis.StatusCode;
            _status = status;

            FileDescriptor = firmwareAnalysis.FileDescriptor;
            Unit = firmwareAnalysis.Unit;
            DevType = firmwareAnalysis.DevType;
            PathData = firmwareAnalysis.PathData;
            PathFill = firmwareAnalysis.PathFill;
            PathDataState = pathDataSate;
        }

        protected override void SetProperty<T>(ref T fieldProperty, T newValue, [CallerMemberName] string propertyName = "")
        {
            if (propertyName == nameof(Version) && newValue is int numb)
            {
                ErrorType = (FirmwareErrorType)numb.CompareTo(Version);
            }
            base.SetProperty(ref fieldProperty, newValue, propertyName);

        }
    }
}
