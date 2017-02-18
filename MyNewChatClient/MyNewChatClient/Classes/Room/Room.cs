using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyNewChatClient
{
    public class Room
    {
        TcpClient connection;
        public String name;


        public Room(TcpClient connection)
        {
            this.connection = connection;
        }
        public bool SendHendler(object sender, Request request)
        {
            if ((sender as string).Length > 500)
            {
                MessageBox.Show("Большой текст");
                return false;
            }
            request.modul = "rooms";
            request.command = "message";
            request.data = sender as string;
            request.time = name;
            StreamWriter writer = new StreamWriter(connection.GetStream());
            writer.WriteLine(JsonConvert.SerializeObject(request));
            writer.Flush();
            return true;
        }
        public void LeaveHendler(object sender, Request request)
        {
            request.modul = "rooms";
            request.command = "leave";
            request.data = name;
            StreamWriter writer = new StreamWriter(connection.GetStream());
            writer.WriteLine(JsonConvert.SerializeObject(request));
            writer.Flush();
        }
        public void ChengeTextHendler(string text)
        {
            if (text.Length > 500)
                MessageBox.Show("Большой текст");
        }
    }
}
