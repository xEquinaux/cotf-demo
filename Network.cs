using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGamePort
{
    public class NetHost
    {
        public static NetHost Instance;
        public static IList<UdpClient> Client = new List<UdpClient>();
        TcpListener tcp;
        int port;
        public NetHost(int port)
        {
            Instance = this;
            this.port = port;
            Initialize();
        }
        protected void Initialize()
        {
            tcp = new TcpListener(IPAddress.Any, port);
            tcp.Start(8);
        }
        protected UdpClient AcceptConnections()
        {
            var client = new UdpClient(port);
            client.Client = tcp.AcceptSocket();
            return client;
        }                        
    }
    public class NetClient
    {
        public NetHost Host
        {
            get { return NetHost.Instance; }
        }
        private NetworkStream stream;
        private UdpClient client;
        public void ConnectToHost(IPAddress ip, int port = 8100)
        {
            client = EstablishConnection(ip, port).Result;
            stream = new NetworkStream(client.Client);
        }
        private async Task<UdpClient> EstablishConnection(IPAddress ip, int port = 8100)
        {
            return await Task.Factory.StartNew(() =>
            {
                UdpClient client = new UdpClient();
                int tries = 0;
                while (tries++ < 10)
                {
                    Thread.Sleep(1000);
                    try
                    {
                        client.Connect(ip, port);
                    }
                    catch (Exception e)
                    {
                        continue;
                    }
                }
                return client;
            });
        }
    }
}
