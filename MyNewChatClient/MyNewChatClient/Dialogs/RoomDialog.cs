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
        }
        private void btn_back_Click(object sender, EventArgs e)
        {
            room.LeaveHendler(sender, request);
        }
        private void txt_msg_TextChanged(object sender, EventArgs e)
        {
            room.ChengeTextHendler(txt_msg.Text);
        }
    }
}
