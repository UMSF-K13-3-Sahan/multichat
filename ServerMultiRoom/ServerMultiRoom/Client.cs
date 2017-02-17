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
        public Client(TcpClient client)
        {
            this.client = client;
            netStream = client.GetStream();
            GetName();
        }

        public void GetName()
        {
            StreamReader reader = new StreamReader(netStream);
            name = reader.ReadLine();
            StreamWriter sw = new StreamWriter(client.GetStream());
            Request req;
            if (name == "admin")
            {
                req = new Request("ok", null, "admin");
            }
            else
            {
                req = new Request("ok", null, null);
            }
            sw.WriteLine(JsonConvert.SerializeObject(req));
            sw.Flush();

        }
    }

}
