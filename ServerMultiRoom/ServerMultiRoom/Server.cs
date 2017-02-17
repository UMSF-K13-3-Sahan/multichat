
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Newtonsoft.Json;

namespace ServerMultiRoom
{
    public class Server
    {
        //some commit++
        private int PORT = 8888;
        TcpListener server;
        public List<Client> clientsList;
        public Rooms rooms;
        Authorization auth;
        Lobby lobbys;     
        //vasiliu
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
            }
        }
        public void Receive()
        {
            while (true)
            {
                for (int i = 0; i < clientsList.Count; i++)
                {
                    StreamReader reader = new StreamReader(clientsList[i].netStream);
                    if (clientsList[i].netStream.DataAvailable)
                    {
                        string message = reader.ReadLine();
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
                }
            }
        }
        public void SetRoom(List<Client> clientsList, Rooms rms, int index)
        {
            string rooms = "";
            foreach (var room in rms.roomList)
            {
                if (room.privateroom && room.IsHere(clientsList.ElementAt(index)))
                    rooms += room.name + ".";
                if (!room.privateroom)
                    rooms += room.name + ".";

            }
            rooms = rooms.TrimEnd('.');

            Request req = new Request("refresh", null, rooms);
            string responce = JsonConvert.SerializeObject(req);

            StreamWriter writer = new StreamWriter(clientsList.ElementAt(index).netStream);
            writer.WriteLine(responce);
            writer.Flush();
        }
        public void SetClient(List<Client> clientsList, int index)
        {
            string clients = "";
            foreach (var client in clientsList)
            {
                if (client.name == "admin")
                    continue;
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
    }
}
