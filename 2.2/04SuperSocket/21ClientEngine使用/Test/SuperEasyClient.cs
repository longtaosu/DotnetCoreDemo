using SuperSocket.ClientEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClientEngineDemo.Test
{
    public class SuperEasyClient
    {
        private static EasyClient<MyPackageInfo> client;
        //public static SuperEasyClient Client
        //{
        //    get
        //    {
        //        if (!client.IsConnected)
        //            ConnectServer();
        //        return client.IsConnected ? this : null;
        //    }
        //}

        public static async Task<bool> ConnectServer()
        {
            client = new EasyClient<MyPackageInfo>();

            client.Initialize(new MyFilter());
            client.Connected += Client_Connected;
            client.NewPackageReceived += Client_NewPackageReceived;
            client.Error += Client_Error;
            client.Closed += Client_Closed;

            var connected = await client.ConnectAsync(new IPEndPoint(IPAddress.Parse("192.168.11.167"), 2020));
            if (connected)
                Console.WriteLine("connected");
            else
                Console.WriteLine("not connected");

            return connected;
        }

        private static void Client_Closed(object sender, EventArgs e)
        {
            int attmpts = 5;
            do
            {
                Thread.Sleep(5000);
                ConnectServer();
                attmpts--;
            } while (!client.IsConnected && attmpts > 0);
        }

        private static void Client_Error(object sender, ErrorEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void Client_NewPackageReceived(object sender, PackageEventArgs<MyPackageInfo> e)
        {
            Console.WriteLine(e.Package.Body);
        }

        private static void Client_Connected(object sender, EventArgs e)
        {
            Console.WriteLine("already connected");
        }

        public static void SendMessage(string message)
        {
            if (client == null || !client.IsConnected || message.Length <= 0)
                return;
            //var response = BitConverter.GetBytes((ushort)command).Reverse().ToList();
            //var arr = System.Text.Encoding.UTF8.GetBytes(message);
            //response.AddRange(BitConverter.GetBytes((ushort)arr.Length).Reverse().ToArray());
            //response.AddRange(arr);
            var response = System.Text.Encoding.UTF8.GetBytes(message);
            client.Send(response.ToArray());
        }
    }
}
