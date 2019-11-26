using SuperSocket.ProtoBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientEngineDemo.Test
{
    public class MyFilter : BeginEndMarkReceiveFilter<MyPackageInfo>
    {
        //开始和结束标记也可以是两个或两个以上的字节
        private readonly static byte[] BeginMark = new byte[] { (byte)'1' };
        private readonly static byte[] EndMark = new byte[] { (byte)'2' };

        public MyFilter() : base(BeginMark, EndMark)
        { }

        public override MyPackageInfo ResolvePackage(IBufferStream bufferStream)
        {
            byte[] header = bufferStream.Buffers[0].ToArray();
            //byte[] bodyBuffer = bufferStream.Buffers[1].ToArray();
            byte[] allBuffer = bufferStream.Buffers[0].Array.CloneRange(0, (int)bufferStream.Length);

            return new MyPackageInfo(allBuffer, allBuffer);
        }


    }
}
