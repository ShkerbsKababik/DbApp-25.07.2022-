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

namespace DBClient.Models
{
    // This class owns information of ViewModel's files 
    // Also here can be set ViewModel defalt information
    public class ViewData : INotifyPropertyChanged
    {
        // Usrs autorization name to identify 
        private string name;
        public string Name 
        {
            get { return name; }
            set { name = value; OnPropertyChanged(nameof(Name));} 
        }
        // User password that gives him rights
        private string password;
        public string Password
        {
            get { return password; }
            set { password = value; OnPropertyChanged(nameof(Password)); }
        }
        // IP of server that will be asked
        private string ip;
        public string IP
        {
            get { return ip; }
            set { ip = value; OnPropertyChanged(nameof(IP)); }
        }
        // Console that i used to show debug information <- it needs to be deleted after all !!!
        private string console = "check all !";
        public string Console
        {
            get { return console; }
            set { console = value; OnPropertyChanged(nameof(Console)); }
        }
        // Port of server that will be asked 
        private int port;
        public int Port
        {
            get { return port; }
            set { port = value; OnPropertyChanged(nameof(Port)); }
        }
        // Name of table that will be open
        private string tableName;
        public string TableName
        {
            get { return tableName; }
            set { tableName = value; OnPropertyChanged(nameof(TableName)); }
        }
        // Slide1 height
        private string slide1Height = "*";
        public string Slide1Height
        {
            get { return slide1Height; }
            set { slide1Height = value; OnPropertyChanged(nameof(Slide1Height)); }
        }
        // Slide2 height
        private string slide2Height = "0";
        public string Slide2Height
        {
            get { return slide2Height; }
            set { slide2Height = value; OnPropertyChanged(nameof(Slide2Height)); }
        }

        // Constructor that used only for debug <- after all it will take info from app settings !!!
        public ViewData()
        {
            Name = "admin";
            Password = "123";
            IP = "127.0.0.1";
            Port = 8888;
        }
        // Method that clear fields of this class <- it needs to be deleted after all !!!
        public void ClearFilds()
        { 
            Name = string.Empty;
            Password = string.Empty;
            IP = string.Empty;
            Port = 0;
        }

        // MVVM Propertys
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
