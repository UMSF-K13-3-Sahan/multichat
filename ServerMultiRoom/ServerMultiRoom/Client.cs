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
        public UniversalStream us;
        public Client(TcpClient client, string name, UniversalStream us)
        {
            this.client = client;
            netStream = client.GetStream();
            this.us = us;
            this.name = name;
        }
        public string Read()
        {
            return us.Read();
        }
        public void Write(string message)
        {
            us.Write(message);
        }
    }

}
