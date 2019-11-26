using SuperSocket.ProtoBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientEngineDemo.Test
{
    public class MyPackageInfo : IPackageInfo
    {
        public MyPackageInfo(byte[] header,byte[] data)
        {
            Header = header;
            Data = data;
        }
        public byte[] Header { get; set; }
        public byte[] Data { get; set; }

        public string Body
        {
            get
            {
                return Data==null?"hello world": Encoding.UTF8.GetString(Data);
            }
        }
    }
}
