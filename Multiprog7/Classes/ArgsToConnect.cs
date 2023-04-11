using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multiprog7.Classes
{
    public class ArgsToConnect
    {
        private string _ip = "-ip";
        private string _port = "-port";
        private string _cloud = "-cloud";
        private string _lu = "-lu";
        private string _pass = "-pass";
        private string _hash = "-hash";
        private string _can = "-can";
        private string _file = "-file";

        public List<string> Args = new List<string>();
        public string LuId;

        public ArgsToConnect(string ip, string port, bool cloud, string lu, string pass, string hash, string can, string file)
        {
            if (ip != "" && ip != null && ip != " ")
                Args.Add(_ip + ip);
            if (port != "" && port != null && port != " ")
                Args.Add(_port + port);
            if (cloud)
                Args.Add(_cloud);
            if (lu != "" && lu != null && lu != " ")
            {
                Args.Add(_lu + lu );
                LuId = lu;
            }
            if (pass != "" && pass != null && pass != " ")
                Args.Add(_pass + pass);
            if (hash != "" && hash != null && hash != " ")
                Args.Add(_hash + hash);
            if (can != "" && can != null && can != " ")
                Args.Add(_can + can);
            if (file != "" && file != null && file != " ")
                Args.Add(_file + file);
        }
    }
}
