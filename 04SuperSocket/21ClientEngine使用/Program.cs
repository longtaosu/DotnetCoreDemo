using ClientEngineDemo.SupersocketExt;
using ClientEngineDemo.Test;
using SuperSocket.ClientEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ClientEngineDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            //var client = SuperEasyClient.ConnectServer();

            //if(client.Result)
            //{
            //    var message = Console.ReadLine();
            //    while(string.IsNullOrEmpty(message))
            //    {
            //        SuperEasyClient.SendMessage(message);
            //    }
            //}

            var client = new EasyClient<MyPackageInfo>();
            client.Initialize(new MyFilter());
            client.NewPackageReceived += Client_NewPackageReceived;   
            var connected = client.ConnectAsync(new IPEndPoint(IPAddress.Parse("192.168.11.167"), 2020));

            if (connected.Result)
            {
                // Send data to the server
                client.Send(Encoding.ASCII.GetBytes("LOGIN kerry"));
            }

            while(true)
            {
                var message = Console.ReadLine();
                client.Send(Encoding.ASCII.GetBytes(message));
            }

            Console.ReadLine();
        }

        private static void Client_NewPackageReceived(object sender, PackageEventArgs<MyPackageInfo> e)
        {
            try
            {
                Console.WriteLine(e.Package.Body);
            }
            catch(Exception ee)
            {
                Console.WriteLine(ee);
            }

        }
    }
}
