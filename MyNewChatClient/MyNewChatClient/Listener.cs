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
        Thread listenthread;
        Thread tr;
        public Listener(TcpClient client,  MainForm form1)
        {
            this.client = client;
            listenthread = new Thread(new ThreadStart(Listen));
            listenthread.Start();
            this.form1 = form1;   
        }
        public void CloseThreads()
        {
            if (listenthread != null)
            {
                if (listenthread.IsAlive)
                    listenthread.Abort();
            }
            if (tr != null)
            {
                if (tr.IsAlive)
                    tr.Abort();
            }
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
                            if (req.data == "missed")
                            {
                                string[] tmp1 = req.time.Split('.');
                                for (int i = 0; i < tmp1.Length; i++)
                                {
                                    rd.rtb_message.Text += tmp1[i] + "\n";
                                }
                            }
                            tr = new Thread(new ThreadStart(OpenForm));
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
                        case "badlogin":
                            MessageBox.Show("Плохой Log/pass");
                            break;
                        case "nicelogin":
                            VisibleComponents(req.data);
                            break;

                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Сервер упал");
                form1.client = null;
                try
                {
                    rd.Close();
                }
                catch (Exception e) { }

                foreach (Control item in form1.Controls)
                {
                    item.Enabled=false;
                    client = null;
                }
            }
        }

        private void VisibleComponents(object name)
        {
            form1.lst_rooms.Invoke(new Action(() => { form1.lst_rooms.Visible = true; }));
            form1.btn_create_room.Invoke(new Action(() => { form1.btn_create_room.Visible = true; }));
            form1.btn_refresh_rooms.Invoke(new Action(() => { form1.btn_refresh_rooms.Visible = true; }));
            form1.btn_room_enter.Invoke(new Action(() => { form1.btn_room_enter.Visible = true; }));
            form1.btn_refresh_clients.Invoke(new Action(() => { form1.btn_refresh_clients.Visible = true; }));
            form1.btn_private.Invoke(new Action(() => { form1.btn_private.Visible = true; }));

            form1.lst_clients.Invoke(new Action(() => { form1.lst_clients.Visible = true; }));
            form1.lst_rooms.Invoke(new Action(() => { form1.lst_rooms.Visible = true; }));

            form1.lst_clients.Invoke(new Action(() => { form1.lst_clients.Visible = true; }));
            form1.btn_logout.Invoke(new Action(() => { form1.btn_logout.Visible = true; }));
            form1.btn_login.Invoke(new Action(() => { form1.btn_login.Visible = false; }));
            form1.txt_name.Invoke(new Action(() => { form1.txt_name.Visible = false; }));

            form1.lb_hint.Invoke(new Action(() => { form1.lb_hint.Visible = false; }));
            form1.label1.Invoke(new Action(() => { form1.label1.Text = (string)name; }));
            form1.label1.Invoke(new Action(() => { form1.label1.Visible = true; }));
            form1.txt_pass.Invoke(new Action(() => { form1.txt_pass.Visible = false; }));
            form1.btn_reg.Invoke(new Action(() => { form1.btn_reg.Visible = false; }));
            form1.lbl_pass.Invoke(new Action(() => { form1.lbl_pass.Visible = false; }));
            if ((string)name == "admin")
            {
                form1.btn_ban.BeginInvoke(new InvokeDelegate(form1.VisibleBan));
                form1.btn_unban.BeginInvoke(new InvokeDelegate(form1.VisibeUnban));
            }

        }
    }
}
