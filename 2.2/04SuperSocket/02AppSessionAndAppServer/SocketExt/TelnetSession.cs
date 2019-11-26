using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppServerAndSession
{
    public class TelnetSession : AppSession<TelnetSession>
    {
        /// <summary>
        /// 组织机构代码
        /// </summary>
        public string OrgCode { get; set; }


        protected override void OnSessionStarted()
        {
            //base.OnSessionStarted();
            this.Send("Welcome to SuperSocket Telnet Server");
        }

        protected override void HandleUnknownRequest(StringRequestInfo requestInfo)
        {
            //base.HandleUnknownRequest(requestInfo);
            this.Send("Unknow request");
        }

        protected override void HandleException(Exception e)
        {
            //base.HandleException(e);
            this.Send("Application error:{0}", e.Message);
        }

        protected override void OnSessionClosed(CloseReason reason)
        {
            base.OnSessionClosed(reason);
        }
    }
}
