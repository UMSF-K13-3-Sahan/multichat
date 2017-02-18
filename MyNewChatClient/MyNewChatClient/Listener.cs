using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.Drawing;

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
            try
            {
                while (true)
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
                            if (str[0] == "missed")
                            {
                                string[] tmp1 = str[1].Split('.');
                                for (int i = 0; i < tmp1.Length; i++)
                                {
                                    rd.rtb_message.Text += tmp1[i] + "\n";
                                }
                            }
                            Thread tr = new Thread(new ThreadStart(OpenForm));
                            tr.Start();
                            break;
                        case "leave":
                            rd.Close();
                            break;
                        case "bancreate":
                            MessageBox.Show(req.data);
                            break;
                        case "wrongroom":
                            MessageBox.Show("Ошибка, комната " + req.data + " уже создана");
                            break;
                        case "refresh":
                            string[] splitrooms = req.data.Split('.');
                            if (splitrooms[0] == "")
                            {
                                break;
                            }
                            form1.lst_rooms.Items.Clear();
                            form1.lst_rooms.Items.AddRange(splitrooms);
                            break;
                        case "refreshclients":
                            string[] splitclients = req.data.Split('.');
                            form1.lst_clients.Items.Clear();
                            if (splitclients[0] == "")
                            {
                                break;
                            }
                            form1.lst_clients.Items.AddRange(splitclients);
                            break;
                        case "message":
                            rd.rtb_message.Text += req.data + "\n";
                            string[] tmp = req.data.Split(':');
                            string str11 = tmp[0];
                            int j = 0;
                            while (j <= rd.rtb_message.Text.Length - str11.Length)
                            {
                                j = rd.rtb_message.Text.IndexOf(str11, j);
                                if (j < 0) break;
                                rd.rtb_message.SelectionStart = j;
                                rd.rtb_message.SelectionLength = str11.Length;
                                rd.rtb_message.SelectionColor = Color.Red;
                                j += str11.Length;
                            }
                            break;
                        case "youbanned":
                            rd.rtb_message.Text += req.data + "\n";
                            break;
                        case "pofig":
                            int a = -1;
                            for (int i = 0; i < form1.lst_rooms.Items.Count; i++)
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
                        case "wrongprivate":
                            MessageBox.Show("Уже есть приватная комната с этим клиентом");
                            break;
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Сервер упал");
                foreach (Control item in form1.Controls)
                {
                    item.Enabled=false;
                    client = null;
                }
            }
        }
    }
}
