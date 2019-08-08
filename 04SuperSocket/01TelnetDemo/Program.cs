using SuperSocket.SocketBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSocketDemo.TelnetDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press any key to start the server!");

            Console.ReadKey();
            Console.WriteLine();

            var appServer = new AppServer();
            appServer.NewSessionConnected += AppServer_NewSessionConnected;
            appServer.NewRequestReceived += AppServer_NewRequestReceived;
            appServer.SessionClosed += AppServer_SessionClosed;

            //Setup the appServer
            if(!appServer.Setup(2012))//setup with listening port
            {
                Console.WriteLine("Failed to setup");
                Console.ReadKey();
                return;
            }

            Console.WriteLine();

            //Try to start the appServer
            if(!appServer.Start())
            {
                Console.WriteLine("Failed to start");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("The server started successfully,press key 'q' to stop it!");

            while (Console.ReadKey().KeyChar != 'q')
            {
                Console.WriteLine();
                continue;
            }

            //Stop the appServer
            appServer.Stop();

            Console.WriteLine("The server was stopped!");
            Console.ReadKey();
        }

        private static void AppServer_SessionClosed(AppSession session, CloseReason value)
        {
            Console.WriteLine(string.Format("{0}断开连接", session.SessionID));
            session.Send(value.ToString());
        }

        private static void AppServer_NewRequestReceived(AppSession session, SuperSocket.SocketBase.Protocol.StringRequestInfo requestInfo)
        {
            switch (requestInfo.Key.ToUpper())
            {
                case ("ECHO"):
                    session.Send(requestInfo.Body);
                    break;

                case ("ADD"):
                    session.Send(requestInfo.Parameters.Select(p => Convert.ToInt32(p)).Sum().ToString());
                    break;

                case ("MULT"):

                    var result = 1;

                    foreach (var factor in requestInfo.Parameters.Select(p => Convert.ToInt32(p)))
                    {
                        result *= factor;
                    }

                    session.Send(result.ToString());
                    break;
            }
        }

        private static void AppServer_NewSessionConnected(AppSession session)
        {
            session.Send("Welcome to SuperSocket Telnet Server");
        }
    }
}
