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
using Newtonsoft.Json;

namespace DBServer
{
    public class Server
    {
        private TcpListener _tcpListener;
        public Server(string ip, int port)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip),port);
            _tcpListener = new TcpListener(endPoint);

            Main();
        }
        private void Main()
        {
            try { Console.WriteLine("-> Program started"); StartProcess(); }
            catch(Exception e) { Console.WriteLine(e); EndProcess(); }
        }
        private void StartProcess()
        { 
            _tcpListener.Start();
            while (true)
            { 
                TcpClient tcpClient = _tcpListener.AcceptTcpClient();             
                ClientProcess(new Client(tcpClient));
            }
        }
        private void EndProcess()
        {
            try { Console.WriteLine("-> There was an error"); }
            catch{ Console.WriteLine("-> There was an error"); }
        }
        private async void ClientProcess(Client client)
        {
            await Task.Run(() =>
            {
                Packet packet;
                try
                {
                    packet = GetPacket(client.tcpClient);
                    ShowInfo(client, packet);

                    packet = SQLHandler.Operate(client, packet);
                    SendPacket(client, packet); 
                }
                catch
                {
                    Console.WriteLine("<- ClientProcess error");
                }
                finally
                { 
                    client.tcpClient.Dispose();
                }
            });
        }
        private Packet GetPacket(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            List<byte> byteList = new List<byte>();

            do
            {
                byteList.Add(Convert.ToByte(stream.ReadByte()));
            }
            while (stream.DataAvailable);

            return JsonConvert.DeserializeObject<Packet>(Encoding.Unicode.GetString(byteList.ToArray()));
        }
        private void ShowInfo(Client client, Packet packet)
        {
            Console.WriteLine("======== Process =========");
            Console.WriteLine("<- name     : " + packet.name);
            Console.WriteLine("<- password : " + packet.password);
            Console.WriteLine("<- ip       : " + client.GetIP());
            Console.WriteLine("<- type     : " + packet.type);
        }
        private void SendPacket(Client client, Packet packet)
        {
            NetworkStream stream = client.tcpClient.GetStream();
            string str = JsonConvert.SerializeObject(packet);
            byte[] data = Encoding.Unicode.GetBytes(str);
            stream.Write(data, 0, data.Length);
            stream.Close();
            client.tcpClient.Close();
        }
    }
}
