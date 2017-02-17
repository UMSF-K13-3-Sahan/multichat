using System.IO;
using System.Net.Sockets;
using Newtonsoft.Json;

namespace MyNewChatClient
{
    public class CreateRoom
    {
        public CreateRoom()
        {
        }
        public void CreateHendler(string name,NetworkStream stream, Request request )
        {
            request.modul = "rooms";
            request.command = "createroom";
            request.data = name;
            StreamWriter writer = new StreamWriter(stream);
            writer.WriteLine(JsonConvert.SerializeObject(request));
            writer.Flush();
        }
    }
}
