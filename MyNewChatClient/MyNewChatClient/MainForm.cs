using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace MyNewChatClient
{
    public partial class MainForm : Form
    {
        Dialog dialog;
        public TcpClient client;
        Auth auth;
        RefreshListBoxes refresh;
        public Request request;
        Thread threadDialog;
        Listener listener;
        public MainForm()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            auth = new Auth();
            refresh = new RefreshListBoxes();
            request = new Request();
            FormClosed += MainForm_FormClosed;
        }

        private void btn_create_room_Click(object sender, EventArgs e)
        {
            dialog = new Dialog("createroom",client,"name", request);
            threadDialog = new Thread(new ThreadStart(OpenDialogForm));
            threadDialog.Start();
            Thread.Sleep(100);
            refresh.RefreshHendler(client.GetStream(), "Rooms", request);
        }
        private void btn_private_Click(object sender, EventArgs e)
        {
            if (lst_clients.SelectedItem != null)
            {
                PrivateRoom create = new PrivateRoom();
                create.CreatePrivateHandler(client.GetStream(), lst_clients.SelectedItem.ToString(), request);
            }
            else
            {
                MessageBox.Show("User is not choosen. Please, chose the user first.");
            }
            refresh.RefreshHendler(client.GetStream(), "Rooms", request);
        }
        private void btn_login_Click(object sender, EventArgs e)
        {
            if(txt_name.Text.Length ==0 || txt_pass.Text.Length == 0)
                MessageBox.Show("Введите имя и пароль!");
            else if (CheckName())
            {
                try
                {
                client = new TcpClient();
                client.Connect("127.0.0.1", 8888);
                auth.LogInHendler(client, txt_name.Text, txt_pass.Text, request);

                listener = new Listener(client, this);

               

                refresh.RefreshHendler(client.GetStream(), "Rooms", request);
                refresh.RefreshHendler(client.GetStream(), "clients", request);
                }
                catch (Exception ex)
                {
                    this.client = null;
                    MessageBox.Show(ex.Message);
                }
            }
            else
                MessageBox.Show("Имя может содержать только буквы и цифры не более 15 символов");
            Thread.Sleep(100);
        }
        private void btn_reg_Click(object sender, EventArgs e)
        {
            if (txt_name.Text.Length == 0 || txt_pass.Text.Length == 0)
                MessageBox.Show("Введите имя и пароль!");
            else if (CheckName())
            {
                try
                {
                    client = new TcpClient();
                    client.Connect("127.0.0.1", 8888);
                    auth.RegHendler(client, txt_name.Text, txt_pass.Text, request);
                    listener = new Listener(client, this);
                }
                catch (Exception ex)
                {
                    this.client = null;
                    MessageBox.Show(ex.Message);
                }
            }
            else
                MessageBox.Show("Имя может содержать только буквы и цифры не более 15 символов");
            Thread.Sleep(100);
        }
        private bool CheckName()
        {
            Regex rgx = new Regex("^[а-яА-ЯёЁa-zA-Z0-9]+$");
            if (rgx.IsMatch(txt_name.Text.ToString()) && txt_name.Text.ToString().Length <= 15)
                return true;
            return false;
        }

        private void btn_logout_Click(object sender, EventArgs e)
        {
            auth.LogoutHendler(client, request);
            Application.Restart();
        }

        private void btn_refresh_rooms_Click(object sender, EventArgs e)
        {
            refresh.RefreshHendler(client.GetStream(),"Rooms", request);
        }

        private void btn_refresh_clients_Click(object sender, EventArgs e)
        {
            refresh.RefreshHendler(client.GetStream(), "clients", request);
        }

        private void btn_room_enter_Click(object sender, EventArgs e)
        {
            EnterRoom();
        }

        private void btn_ban_Click(object sender, EventArgs e)
        {
            if (lst_clients.SelectedItem != null)
            {
                dialog = new Dialog("ban",client, lst_clients.SelectedItem.ToString(), request);
                threadDialog = new Thread(new ThreadStart(OpenDialogForm));
                threadDialog.Start();
            }
        }

        private void EnterRoom()
        {
            if (lst_rooms.SelectedItem != null)
            {
                string[] str = lst_rooms.SelectedItem.ToString().Split(' ');
                request.modul = "rooms";
                request.command = "enter";
                request.data = str[0];
                StreamWriter writer = new StreamWriter(client.GetStream());
                writer.WriteLine(JsonConvert.SerializeObject(request));
                writer.Flush();             
            }
            else
            {
                MessageBox.Show("Room is not choosen. Please, chose the room first.");
            }
        }

        private void OpenDialogForm()
        {
            dialog.ShowDialog();
        }
        public void VisibleBan()
        {
           btn_ban.Visible = true;
        }
        public void VisibeUnban()
        {
            btn_unban.Visible = true;
        }

        private void btn_unban_Click(object sender, EventArgs e)
        {
            if (lst_clients.SelectedItem != null)
            {
                dialog = new Dialog("unban",client, lst_clients.SelectedItem.ToString(),request);
                threadDialog = new Thread(new ThreadStart(OpenDialogForm));
                threadDialog.Start();
            }
        }
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                auth.LogoutHendler(client, request);
                Application.Exit();
            }
            catch (Exception ex)
            {
                Environment.Exit(0);
            }
        }


    }
}
