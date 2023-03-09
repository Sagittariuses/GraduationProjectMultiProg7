using Multiprog7.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multiprog7.Model
{
    public class VMDevice
    {
        public int CanID { get; set; }
        public string Title { get; set; }
        public string AppVer { get; set; }
        public string FWTitle { get; set; }
        public string FWVersion { get; set; }
        public string FWDate { get; set; }

        public static void SendReadAsk(byte CanID)
        {
            PageMain.Driver.SendPack(new LKDSFramework.Packs.DataDirect.IAPService.PackV7IAPReadAsk()
            {
                UnitID = PageMain.Driver.Devices[0].UnitID,
                CanID = CanID,
                Num = 2
            });
        }
    }
}
