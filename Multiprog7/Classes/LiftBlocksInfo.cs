using LKDSFramework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Multiprog7.Classes
{
    public class LiftBlocksInfo : DeviceV7
    {
        private static string appPath = AppDomain.CurrentDomain.BaseDirectory;
        private static string appName = AppDomain.CurrentDomain.FriendlyName;

        public ArgsToConnect Connect;


        public LiftBlocksInfo(DeviceV7 dev, ArgsToConnect connect) : base(dev)
        {
            this.Connect = connect;
            Title = dev.ToString();
        }
        public LiftBlocksInfo(DeviceV7 dev) : base(dev)
        {
            Title = dev.ToString();
        }
        public string Title { get; set; }


        public void SaveCurrentLbBat(string path, ArgsToConnect args)
        {
            FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);

            StreamWriter streamWriter = new StreamWriter(fileStream);
            streamWriter.AutoFlush = true;
            streamWriter.WriteLine("@chcp 65001");
            streamWriter.Write($"\"{appPath.Substring(0,appPath.Length-1)}\"\\{appName} ");
            var res = "";

            foreach (var item in args.Args)
            {
                res += item + " ";
            }
            res = res.Trim();
            streamWriter.Write(res);
            streamWriter.Close();
        }

        public static void SaveFullBat(string path, ObservableCollection<LiftBlocksInfo> LiftBlocks)
        {
            FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);

            StreamWriter streamWriter = new StreamWriter(fileStream);
            streamWriter.AutoFlush = true;
            streamWriter.WriteLine("@chcp 65001");
            streamWriter.Write($"\"{appPath.Substring(0, appPath.Length - 1)}\"\\{appName} ");
            var res = "";

            foreach (var lb in LiftBlocks)
            {
                foreach (var args in lb.Connect.Args)
                {
                    foreach (var item in args)
                    {
                        res += item;
                    }
                    res += " ";
                }
                res += "+ ";
            }




            res = res.Trim();
            streamWriter.Write(res);
            streamWriter.Close();
        }


    }
}
