using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketEngine;
using SuperSocket.WebSocket;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SslSocketDemo
{
    public partial class frmMain : Form
    {
        AppServer appServer;
        //WebSocketServer appServer;

        public frmMain()
        {
            InitializeComponent();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            var config = new ServerConfig()
            {
                Name = "AliPayServer",
                ClearIdleSession = true,
                ClearIdleSessionInterval = 60,
                IdleSessionTimeOut = 90,
                Ip = "Any",
                Port = 10100,
                Mode = SuperSocket.SocketBase.SocketMode.Tcp,
                MaxConnectionNumber = 600,
                MaxRequestLength = 1024 * 1024 * 20,
                Security = "tls",
                LogCommand = true,
                Certificate = new CertificateConfig
                {
                    Password = "123456",
                    FilePath = Application.StartupPath + @"\Certificates.pfx",
                }
            };

            appServer = new AppServer();
            appServer.NewSessionConnected += AppServer_NewSessionConnected;
            appServer.NewRequestReceived += AppServer_NewRequestReceived;
            //appServer = new WebSocketServer();
            //appServer.NewSessionConnected += AppServer_NewSessionConnected1;
            //appServer.NewDataReceived += AppServer_NewDataReceived;
            if (appServer.Setup(1100))
            {
                if (appServer.Start())
                {
                    txtContent.Text = "服务器已启动";
                }
            }

            //var bootstrap = new DefaultBootstrap(new RootConfig(), new IWorkItem[] { appServer });
            //bootstrap.Start();
        }

        private void AppServer_NewDataReceived(WebSocketSession session, byte[] value)
        {
            throw new NotImplementedException();
        }

        private void AppServer_NewSessionConnected1(WebSocketSession session)
        {
            //throw new NotImplementedException();
        }

        private void AppServer_NewRequestReceived(AppSession session, SuperSocket.SocketBase.Protocol.StringRequestInfo requestInfo)
        {
            throw new NotImplementedException();
        }

        private void AppServer_NewSessionConnected(AppSession session)
        {
            //throw new NotImplementedException();
        }
    }
}
