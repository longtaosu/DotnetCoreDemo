using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSocket;
using SuperSocket.Facility.Protocol;
using SuperSocket.SocketBase.Protocol;

namespace CommandLineProtocal
{
    public class MyReceiveFilter : BeginEndMarkReceiveFilter<StringRequestInfo>
    {
        //开始和结束标记也可以是两个或两个以上的字节
        private readonly static byte[] BeginMark = new byte[] { (byte)'!' };
        private readonly static byte[] EndMark = new byte[] { (byte)'$' };

        public MyReceiveFilter()
            : base(BeginMark, EndMark) //传入开始标记和结束标记
        {

        }
        protected override StringRequestInfo ProcessMatchedRequest(byte[] readBuffer, int offset, int length)
        {
            throw new NotImplementedException();
        }
    }
}
