using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multiprog7.Classes
{
    public enum FirmwareStatus
    {
        [Description("#EB5757 | M11.3334 2.60669L9.39342 0.666687L6.00008 4.06002L2.60675 0.666687L0.666748 2.60669L4.06008 6.00002L0.666748 9.39335L2.60675 11.3334L6.00008 7.94002L9.39342 11.3334L11.3334 9.39335L7.94008 6.00002L11.3334 2.60669Z")]
        Error = 0, 
        [Description("#6FCF97 | M4.14 10.36L0 6.22L1.88667 4.33333L4.14 6.59333L10.7267 0L12.6133 1.88667L4.14 10.36Z")]
        Actual = 1,
        [Description("#F5AB18 | M0.666626 0H3.33329V7.33333H0.666626V0ZM0.666626 12V9.33333H3.33329V12H0.666626Z")]
        Outdated = 2,
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
