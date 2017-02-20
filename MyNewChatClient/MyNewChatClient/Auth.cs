using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyNewChatClient
{
    class Auth
    {
        public void LogInHendler(TcpClient client, string name, string pass, Request request)
        {
            request.modul = name;
            request.command = "auth";
            request.data = pass;
            StreamWriter writer = new StreamWriter(client.GetStream());
            writer.WriteLine(JsonConvert.SerializeObject(request));
            writer.Flush();
        }
        public void RegHendler(TcpClient client, string name, string pass, Request request)
        {
            request.modul = name;
            request.command = "reg";
            request.data = pass;
            StreamWriter writer = new StreamWriter(client.GetStream());
            writer.WriteLine(JsonConvert.SerializeObject(request));
            writer.Flush();
        }
        public void LogoutHendler(TcpClient client, Request request)
        {
            request.modul = "auth";
            request.command = "exit";
            request.data = "";
            StreamWriter writer = new StreamWriter(client.GetStream());
            writer.WriteLine(JsonConvert.SerializeObject(request));
            writer.Flush();
        }
    }
}
