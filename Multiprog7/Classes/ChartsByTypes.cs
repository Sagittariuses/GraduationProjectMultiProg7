using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LKDSFramework.VirtualDeviceV7;

namespace Multiprog7.Classes
{
    public class ChartsByTypes
    {
        public SeriesCollection SeriesCol { get; set; }
        public string DevType { get; set; }
        public int DevCount { get; set; }

        public string DevColour { get; set; }
    }
}
