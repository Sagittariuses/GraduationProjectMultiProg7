using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multiprog7.Classes
{
    public class FWForDevice
    {
        public int ver = 0;
        public string url;
        public string path;
        public string name;

        public FWForDevice(int FWVer, string FWUrl, string FWPath, string FWName)
        {
            ver = FWVer;
            url = FWUrl;
            path = FWPath;
            name = FWName;
        }
        public FWForDevice(string FWPath)
        {
            path = FWPath;
        }
    }
}
