using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBClient.Models
{
    public class Packet
    {
        public string name;
        public string password;
        public string ip;
        public string type;
        public int port;
        public string pass;
        public string table_name;

        public string[] columns_name;
        public string[][] table;
    }
}
