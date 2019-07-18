using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppServerAndSession
{
    public class TelnetServer : AppServer<TelnetSession>
    {
        protected override bool Setup(IRootConfig rootConfig, IServerConfig config)
        {
            return base.Setup(rootConfig, config);
        }

        protected override void OnStarted()
        {
            base.OnStarted();
        }

        protected override void OnStopped()
        {
            base.OnStopped();
        }
    }
}
