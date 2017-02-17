﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MyNewChatClient
{
    class Auth
    {
        public void LogInHendler(TcpClient client, string name)
        {
            StreamWriter writer = new StreamWriter(client.GetStream());
            writer.WriteLine(name);
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
