using System;
using System.Net.Sockets;
using System.Windows.Forms;

namespace MyNewChatClient
{
    public partial class RoomDialog : Form
    {
        TcpClient connection;
        public Room room;
        Request request;

        public RoomDialog(TcpClient connection, Request request)
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            this.connection = connection;
            room = new Room(connection);
            this.request = request;
        }
        private void btn_send_Click(object sender, EventArgs e)
        {
           if( room.SendHendler(txt_msg.Text, request) )
                txt_msg.Text = "";
            txt_msg.Focus();
        }
        private void btn_back_Click(object sender, EventArgs e)
        {
            room.LeaveHendler(sender, request);
        }
        private void txt_msg_TextChanged(object sender, EventArgs e)
        {
            room.ChengeTextHendler(txt_msg.Text);
        }

        private void RoomDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (room.SendHendler(txt_msg.Text, request))
                    txt_msg.Clear();
            }
            txt_msg.SelectionLength = 0;
            txt_msg.Focus();
        }
    }
}
