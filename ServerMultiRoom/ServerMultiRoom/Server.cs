
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Newtonsoft.Json;
using System;

namespace ServerMultiRoom
{
    public class Server
    {
        private int PORT = 8888;
        TcpListener server;
        public List<Client> clientsList;
        public Rooms rooms;
        Authorization auth;
        Lobby lobbys;
       

        public Server()
        {
            server = new TcpListener(IPAddress.Parse("127.0.0.1"), PORT);
            rooms = new Rooms(this);
            auth = new Authorization(this);
            lobbys = new Lobby(this);
            clientsList = new List<Client>();
            server.Start();

        }
        public void Start()
        {
            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                auth.AddUser(client, clientsList, rooms);
                Thread tr = new Thread(new ThreadStart(Receive));
                tr.Start();
                Thread.Sleep(100);             
            }
        }
        public void Receive()
        {
            while (true)
            {
                for (int i = 0; i < clientsList.Count; i++)
                {
                    try
                    {
                        if (clientsList[i].netStream.DataAvailable)
                        {
                            string message = clientsList[i].Read();
                            Request req = JsonConvert.DeserializeObject<Request>(message);
                            switch (req.modul)
                            {
                                case "rooms":
                                    rooms.SetCommand(req, i);
                                    break;
                                case "lobby":
                                    lobbys.SetCommand(req, i);
                                    break;
                                case "auth":
                                    auth.SetCommand(req, i);
                                    break;
                            }
                        }
                        Thread.Sleep(100);
                    }
                    catch(Exception ex)
                    {
                        if(clientsList.Count < i)
                          clientsList.Remove(clientsList[i]);
                    }
                }
            }
        }
        public void SetRoom(List<Client> clientsList, Rooms rms, int index)
        {
            string roomss = "";
            foreach (var room in rms.roomList)
            {
                if (room.privateroom && room.IsHere(clientsList.ElementAt(index)))
                    roomss += room.name + ".";
                if (!room.privateroom)
                    roomss += room.name + ".";
            }
            roomss = roomss.TrimEnd('.');

            Request req = new Request("refresh", null, roomss);
            string responce = JsonConvert.SerializeObject(req);

            StreamWriter writer = new StreamWriter(clientsList.ElementAt(index).netStream);
            writer.WriteLine(responce);
            writer.Flush();

            for (int z = 0; z < rooms.roomList.Count; z++)
            {
                if (rooms.roomList[z].IsPassive(clientsList.ElementAt(index)))
                {
                    Thread.Sleep(100);
                    rooms.roomList[z].SendForPassiv();
                }
            }
        }
        public void SetClient(List<Client> clientsList, int index)
        {
            string clients = "";
            foreach (var client in clientsList)
            {
                if (client.name == "admin")
                    continue;
                if(index == clientsList.Count)
                {
                    --index;
                }
                if (client != clientsList.ElementAt(index))
                    clients += client.name + ".";
            }
            clients = clients.TrimEnd('.');

            Request req = new Request("refreshclients", null, clients);
            string responce = JsonConvert.SerializeObject(req);

            StreamWriter writer = new StreamWriter(clientsList.ElementAt(index).netStream);
            writer.WriteLine(responce);
            writer.Flush();
        }
        public void DeleteLogs()
        {
            DirectoryInfo dirInfo = new DirectoryInfo("logs/");

            foreach (FileInfo file in dirInfo.GetFiles())
            {
                file.Delete();
            }
        }
    }
}
