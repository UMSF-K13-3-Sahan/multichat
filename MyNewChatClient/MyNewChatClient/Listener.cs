using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

namespace MyNewChatClient
{
    class Listener
    {
        public delegate void InvokeDelegate();
        TcpClient client;
        RoomDialog rd;
        MainForm form1;
        public Listener(TcpClient client,  MainForm form1)
        {
            this.client = client;
            Thread listenthread = new Thread(new ThreadStart(Listen));
            listenthread.Start();
            this.form1 = form1;   
        }
        private void OpenForm()
        {
            rd.ShowDialog();
        }
        public void Listen()
        {
            while(true)
            {
                StreamReader reader = new StreamReader(client.GetStream());
                string message = reader.ReadLine();
                Request req = JsonConvert.DeserializeObject<Request>(message);
                switch (req.modul)
                {
                    case "ok":
                        if (req.data == "admin")
                        {
                            form1.btn_ban.BeginInvoke(new InvokeDelegate(form1.VisibleBan));
                            form1.btn_unban.BeginInvoke(new InvokeDelegate(form1.VisibeUnban));
                        }
                        break;
                    case "enter":
                        rd = new RoomDialog(client, form1.request);
                        rd.room.name = req.command;
                        rd.Text = req.command;
                        string[] str = req.data.Split(',');
                        if(str[0] == "missed")
                        {
                            string[] tmp = str[1].Split('.');
                            rd.lst_messages.Items.AddRange(tmp);
                        }
                        Thread tr = new Thread(new ThreadStart(OpenForm));
                        tr.Start();
                        break;
                    case "leave":
                        rd.Close();
                        break;
                    case "refresh":
                        string[] splitrooms = req.data.Split('.');
                        if(splitrooms[0] == "")
                        {
                            break;
                        }
                        form1.lst_rooms.Items.Clear();
                        form1.lst_rooms.Items.AddRange(splitrooms);
                        break;
                    case "refreshclients":
                        string[] splitclients = req.data.Split('.');
                        if (splitclients[0] == "")
                        {
                            break;
                        }
                        form1.lst_clients.Items.Clear();
                        form1.lst_clients.Items.AddRange(splitclients);
                        break;
                    case "message":
                        rd.lst_messages.Items.Add(req.data);
                        break;
                    case "youbanned":
                        rd.lst_messages.Items.Add(req.data);
                        break;
                    case "pofig":
                        int a=-1;
                        for (int i=0; i< form1.lst_rooms.Items.Count; i++)
                        {
                            string[] str1 = form1.lst_rooms.Items[i].ToString().Split(' ');
                            if (str1[0] == req.command)
                                a = i;
                        }
                        if (a != -1)
                        {
                            form1.lst_rooms.Items.RemoveAt(a);
                            form1.lst_rooms.Items.Insert(a, req.command + "   +" + req.data);
                        }
                        else
                            form1.lst_rooms.Items.Insert(form1.lst_rooms.Items.Count, req.command + "   +" + req.data);
                        break;
                }
            }
        }
    }
}
