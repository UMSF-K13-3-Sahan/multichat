using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Text.RegularExpressions;

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
        public void AddUser(TcpClient connectedClient, List<Client> clientsList, Rooms rooms)
        {
            UniversalStream stream = new UniversalStream(connectedClient);

            while (!connectedClient.GetStream().DataAvailable) { }

            string data = stream.Read();
            if (new Regex("^GET").IsMatch(data))
            {
                stream.Type = ClientType.Web;
                stream.WriteHandshake(data);

                while (!connectedClient.GetStream().DataAvailable) { }

                data = stream.Read();
            }

            if (stream.Type == ClientType.Web)
                data = stream.Decode();

            Request req = JsonConvert.DeserializeObject<Request>(data);

            if (dbmanager.CreateNewLogin(req.modul, req.data, req.command))
            {
                Client cl = clientsList.Find(c => c.name == req.modul);
                if (cl == null)
                {
                    Client client = new Client(connectedClient, req.modul, stream);
                    clientsList.Add(client);
                    if (client.name == "admin")
                    {
                        Thread tr = new Thread(delegate () { ForAdmin(client, rooms); });
                        tr.Start();
                    }
                    Thread.Sleep(100);
                    req.data = client.name;
                    req.modul = "ok";
                    stream.Write(JsonConvert.SerializeObject(req));
                }
                else
                {
                    req.modul = "badlogin";
                    stream.Write(JsonConvert.SerializeObject(req));
                }
            }
            else
            {
                req.modul = "badlogin";
                stream.Write(JsonConvert.SerializeObject(req));
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
