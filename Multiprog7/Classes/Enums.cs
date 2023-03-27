using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multiprog7.Classes
{
    public enum FirmwareStatus
    {
        Error = 0, Actual = 1, Outdated = 2
    }
    public enum FilterAnalysis
    {
        Title = 0, FwVersion = 1, Date = 2, ErrorType = 3, 
    }
    public enum ModesCodes
    {
        Online = 0, Offline = 1, Manual = 2
    }
    public enum ChartsCodes
    {
        Full = 0, Error = 1, Outdate = 2, Actual = 3, 
    }
}
