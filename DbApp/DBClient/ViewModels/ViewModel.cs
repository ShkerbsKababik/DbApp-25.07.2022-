using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using DBClient.Models;
using DBClient.ViewModels;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Collections.ObjectModel;
namespace DBClient.ViewModels
{
    public partial class ViewModel : INotifyPropertyChanged
    {
        // ViewModel Data / Thing that we need to know about View
        public ObservableCollection<TableSlide> tableSlides { get; } = new ObservableCollection<TableSlide>();
        public ViewData viewData { get; } = new ViewData();

        // MVVM Commnads that used with View buttons 
        public ICommand AutorizationCommand
        {
            get 
            {
                return new Command(async (obj)=> 
                {
                    Packet packet = new Packet()
                    {
                        name = viewData.Name,
                        password = viewData.Password,
                        ip = viewData.IP,
                        port = viewData.Port,
                        type = "autorization"
                    };
                    packet = await ServerRequestAsync(packet);
                    if (packet.pass == "true")
                    {
                        SwitchSlides();
                        viewData.Console = "check all !";
                        viewData.TableName = null;
                    }
                    else
                    {
                        viewData.Console = "autorizatiuon error";
                    }
                });
            }
        }
        public ICommand OpenCommand
        {
            get
            {
                return new Command(async (obj) =>
                {
                    bool canOpen = true;
                    foreach (TableSlide slide in tableSlides)
                    {
                        if (slide.TableName == viewData.TableName)
                        {
                            viewData.Console = "this table has been opened yet";
                            canOpen = false;
                        }
                    }
                    if (canOpen == true)
                    {
                        Packet packet = new Packet()
                        {
                            name = viewData.Name,
                            password = viewData.Password,
                            ip = viewData.IP,
                            port = viewData.Port,
                            type = "open",
                            table_name = viewData.TableName,
                        };
                        packet = await ServerRequestAsync(packet);

                        if (packet.table != null)
                        {
                            tableSlides.Add(new TableSlide(packet, this));
                            viewData.Console = "table was loaded";
                            viewData.TableName = null;
                        }
                        else
                        {
                            viewData.Console = "this table was not found";
                        }
                    }
                    else
                    {
                        viewData.Console = "this table has been opened yet";
                    }
                });
            }
        }

        // Method that helps with main operaions
        public void SwitchSlides()
        {
            if (viewData.Slide1Height == "*" && viewData.Slide2Height == "0")
            {
                viewData.Slide1Height = "0";
                viewData.Slide2Height = "*";
            }
            else if (viewData.Slide1Height == "0" && viewData.Slide2Height == "*")
            {
                viewData.Slide1Height = "*";
                viewData.Slide2Height = "0";
            }
        }
        public async Task<Packet> ServerRequestAsync(Packet packet)
        {
            await Task.Run(() =>
            {
                TcpClient client = new TcpClient();
                NetworkStream stream = null;
                try
                {
                    // Serialization
                    string str = JsonConvert.SerializeObject(packet);
                    byte[] data = Encoding.Unicode.GetBytes(str);

                    // Sending
                    client.Connect(packet.ip,viewData.Port);
                    stream = client.GetStream();
                    stream.Write(data, 0, data.Length);

                    // Reading answer
                    List<byte> answer = new List<byte>();
                    do
                    {
                        answer.Add(Convert.ToByte(stream.ReadByte()));
                    }
                    while (client.GetStream().DataAvailable);                    
                    packet = JsonConvert.DeserializeObject<Packet>(Encoding.Unicode.GetString(answer.ToArray()));
                }
                catch(Exception e)
                {
                    viewData.TableName = e.StackTrace/*"error"*/;
                }
                finally
                {
                    if (client != null && stream != null)
                    {
                        client.Close();
                        stream.Close();
                    }
                }
            });
            return packet;
        }
        public Packet ServerRequest(Packet packet)
        {
            TcpClient client = new TcpClient();
            NetworkStream stream = null;
            try
            {
                // Serialization
                string str = JsonConvert.SerializeObject(packet);
                byte[] data = Encoding.Unicode.GetBytes(str);

                // Sending
                client.Connect(packet.ip, viewData.Port);
                stream = client.GetStream();
                stream.Write(data, 0, data.Length);

                // Reading answer
                List<byte> answer = new List<byte>();
                do
                {
                    answer.Add(Convert.ToByte(stream.ReadByte()));
                }
                while (client.GetStream().DataAvailable);
                packet = JsonConvert.DeserializeObject<Packet>(Encoding.Unicode.GetString(answer.ToArray()));

                // For Debug
                if (packet.table != null)
                {
                    viewData.TableName = (Encoding.Unicode.GetString(answer.ToArray()));
                    viewData.Console = packet.table[1][1];
                }
            }
            catch (Exception e)
            {
                viewData.TableName = e.StackTrace/*"error"*/;
            }
            finally
            {
                if (client != null && stream != null)
                {
                    client.Close();
                    stream.Close();
                }
            }
            return packet;
        }

        // INotifyPropertyChanged realization / MVVM realization
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
