using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerMultiRoom
{
    class Lobby
    {
        AdminFunc admin;
        Server server;
        public Lobby (Server server)
        {
            admin = new AdminFunc();
            this.server = server;

        }
        public void SetCommand(Request req, int index)
        {
            switch (req.command)
            {
                case "refresh":
                    server.SetRoom(server.clientsList,server.rooms,index);
                    for (int z = 0; z < server.rooms.roomList.Count; z++)
                    {
                        if (server.rooms.roomList[z].IsPassive(server.clientsList.ElementAt(index)))
                        {
                            server.rooms.roomList[z].SendForPassiv();
                        }
                    }
                    break;
                case "refreshclients":
                    server.SetClient(server.clientsList, index);
                    break;
                case "ban":
                    admin.BanUser(req.data, req.time);
                    break;
                case "unban":
                    admin.Unban(req.data);
                    break;
            }
        }
            

    }
}
