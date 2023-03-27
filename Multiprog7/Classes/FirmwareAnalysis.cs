using Multiprog7.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Multiprog7.Classes
{
    public class FirmwareAnalysis : OnPropertyChangedClass
    {
        private int _can;
        private string _title;
        private int _fwVersion;
        private DateTime _date;
        private string _statustxt;
        private FirmwareStatus _status;

        public int ID { get; }
        public int CAN { get => _can; set => SetProperty(ref _can, value); }
        public string Title { get => _title; set => SetProperty(ref _title, value); }
        public int Version { get => _fwVersion; set => SetProperty(ref _fwVersion, value); }
        public DateTime Date { get => _date; set => SetProperty(ref _date, value); }
        public string StatusTxt { get => _statustxt; set => SetProperty(ref _statustxt, value); }
        public FirmwareStatus StatusCode { get => _status; set => SetProperty(ref _status, value); }

        public FirmwareAnalysis(int id) => ID = id;

        protected override void SetProperty<T>(ref T fieldProperty, T newValue, [CallerMemberName] string propertyName = "")
        {
            if (propertyName == nameof(Version) && newValue is int numb)
            {
                StatusCode = (FirmwareStatus)numb.CompareTo(Version);
            }
            base.SetProperty(ref fieldProperty, newValue, propertyName);

        }
    }
}
