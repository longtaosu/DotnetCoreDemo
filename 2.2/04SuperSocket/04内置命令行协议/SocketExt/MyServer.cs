using SuperSocket.Facility.Protocol;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineProtocal
{
    public class MyServer : AppServer<MySession, StringRequestInfo>
    {
        public MyServer()
    : base(new DefaultReceiveFilterFactory<MyReceiveFilter, StringRequestInfo>()) //使用默认的接受过滤器工厂 (DefaultReceiveFilterFactory)
        {

        }
    }



    public class MyReceiveFilter : BeginEndMarkReceiveFilter<StringRequestInfo>
    {
        //开始和结束标记也可以是两个或两个以上的字节
        private readonly static byte[] BeginMark = new byte[] { (byte)'1',(byte)'2' };
        private readonly static byte[] EndMark = new byte[] { (byte)'*',(byte)'#' };

        public MyReceiveFilter()
            : base(BeginMark, EndMark) //传入开始标记和结束标记
        {

        }

        protected override StringRequestInfo ProcessMatchedRequest(byte[] readBuffer, int offset, int length)
        {
            //TODO: 通过解析到的数据来构造请求实例，并返回
            string str = System.Text.Encoding.Default.GetString(readBuffer);
            //TxtPrint.txtLog.Info(str);
            //TxtPrint.txt_Rec.Info(str);

            var byteBody = readBuffer.Skip(2).Take(length - 4).ToArray();
            var strBody = System.Text.Encoding.Default.GetString(byteBody);

            string[] strArr = { str };
            return new StringRequestInfo("*", strBody, strArr); ;
        }
    }

    public class MySession : AppSession<MySession, StringRequestInfo>
    {
        public new MyServer AppServer
        {
            get
            {
                return (MyServer)base.AppServer;
            }
        }
    }
}
