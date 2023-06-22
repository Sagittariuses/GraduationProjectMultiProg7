using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multiprog7.Classes
{
    public enum FirmwareErrorType
    {
        [Description("#EB5757 | M11.3334 2.60669L9.39342 0.666687L6.00008 4.06002L2.60675 0.666687L0.666748 2.60669L4.06008 6.00002L0.666748 9.39335L2.60675 11.3334L6.00008 7.94002L9.39342 11.3334L11.3334 9.39335L7.94008 6.00002L11.3334 2.60669Z")]
        Error = 0, 
        [Description("#6FCF97 | M4.14 10.36L0 6.22L1.88667 4.33333L4.14 6.59333L10.7267 0L12.6133 1.88667L4.14 10.36Z")]
        Actual = 1,
        [Description("#F5AB18 | M0.666626 0H3.33329V7.33333H0.666626V0ZM0.666626 12V9.33333H3.33329V12H0.666626Z")]
        Outdated = 2,
    }

    public enum FirmwareUpdateStatus
    {
        [Description("M8.33333 4.33333V6.33333L11 3.66667L8.33333 1V3C6.91885 3 5.56229 3.5619 4.5621 4.5621C3.5619 5.56229 3 6.91885 3 8.33333C3 9.38 3.30667 10.3533 3.82667 11.1733L4.8 10.2C4.5 9.64667 4.33333 9 4.33333 8.33333C4.33333 7.27247 4.75476 6.25505 5.50491 5.50491C6.25505 4.75476 7.27247 4.33333 8.33333 4.33333ZM12.84 5.49333L11.8667 6.46667C12.16 7.02667 12.3333 7.66667 12.3333 8.33333C12.3333 9.3942 11.9119 10.4116 11.1618 11.1618C10.4116 11.9119 9.3942 12.3333 8.33333 12.3333V10.3333L5.66667 13L8.33333 15.6667V13.6667C9.74782 13.6667 11.1044 13.1048 12.1046 12.1046C13.1048 11.1044 13.6667 9.74782 13.6667 8.33333C13.6667 7.28667 13.36 6.31333 12.84 5.49333Z")]
        InProcess = 0,
        [Description("M4.14 10.36L0 6.22L1.88667 4.33333L4.14 6.59333L10.7267 0L12.6133 1.88667L4.14 10.36Z")]
        Updated = 1,

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
