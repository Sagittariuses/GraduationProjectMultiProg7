using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multiprog7.Classes
{
    public class LiftBlocksInfo
    {
        private static string appName = AppDomain.CurrentDomain.FriendlyName;

        public ArgsToConnect Connect { get; set; }
        public string LiftTitle { get; set; }

        public void SaveCurrentLbBat(string path, ArgsToConnect args)
        {
            FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);

            StreamWriter streamWriter = new StreamWriter(fileStream);
            streamWriter.AutoFlush = true;
            streamWriter.Write(appName + " ");
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
            streamWriter.Write(appName + " ");
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
                res += "| ";
            }




            res = res.Trim();
            streamWriter.Write(res);
            streamWriter.Close();
        }


    }
}
