using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace DBServer
{
    public class Client
    {
        public TcpClient tcpClient;

        public Client(TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
        }

        public string GetIP()
        {
            return (IPAddress.Parse(((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString()).ToString());
        }
    }
}
