using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using Newtonsoft.Json;

namespace ServerMultiRoom
{
    public class Client
    {
        public string name { get; set; }
        public TcpClient client;
        public NetworkStream netStream;
        private StreamReader sr;
        public Client(TcpClient client, string name)
        {
            this.client = client;
            netStream = client.GetStream();
            sr = new StreamReader(netStream);
            this.name = name;
        }
        public string Read()
        {
            return sr.ReadLine();
        }
    }

}
