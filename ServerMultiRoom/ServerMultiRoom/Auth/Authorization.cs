using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace ServerMultiRoom
{
    class Authorization
    {
        Server server;
        DataBaseManager dbmanager;
        public Authorization(Server server)
        {
            this.server = server;
            dbmanager = new DataBaseManager();
        }
        public void AddUser(TcpClient tcpclient, List<Client> clientsList, Rooms rooms)
        {
            StreamReader sr = new StreamReader(tcpclient.GetStream());
            string resp = sr.ReadLine();
            Request req = JsonConvert.DeserializeObject<Request>(resp);

            if (dbmanager.CreateNewLogin(req.modul, req.data, req.command))
            {
                Client cl = clientsList.Find(c => c.name == req.modul);
                if (cl == null)
                {
                    Client client = new Client(tcpclient, req.modul);
                    clientsList.Add(client);
                    if (client.name == "admin")
                    {
                        Thread tr = new Thread(delegate () { ForAdmin(client, rooms); });
                        tr.Start();
                    }
                    Thread.Sleep(100);
                    req.data = client.name;
                    req.modul = "nicelogin";
                    StreamWriter writer = new StreamWriter(client.netStream);
                    writer.WriteLine(JsonConvert.SerializeObject(req));
                    writer.Flush();
                }
            }
            else
            {
                req.modul = "badlogin";
                StreamWriter writer = new StreamWriter(tcpclient.GetStream());
                writer.WriteLine(JsonConvert.SerializeObject(req));
                writer.Flush();
            }
        }
        private void ForAdmin(Client client, Rooms rooms)
        {
            foreach (var room in rooms.roomList)
            {
                if(!room.privateroom)
                    room.AddPassive(client);
            }
        }
        public void SetCommand(Request req, int index)
        {
            switch (req.command)
            {
                case "exit":
                    for (int j = 0; j < server.rooms.roomList.Count; j++)
                    {
                        server.rooms.roomList[j].Exit(server.clientsList.ElementAt(index));
                    }
                    server.clientsList.Remove(server.clientsList.ElementAt(index));
                    Thread.Sleep(100);
                    break;
            }
        }
    }
}
