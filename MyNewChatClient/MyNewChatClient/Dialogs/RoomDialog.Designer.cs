namespace MyNewChatClient
{
    partial class RoomDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btn_back = new System.Windows.Forms.Button();
            this.btn_send = new System.Windows.Forms.Button();
            this.txt_msg = new System.Windows.Forms.TextBox();
            this.lst_messages = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // btn_back
            // 
            this.btn_back.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_back.Location = new System.Drawing.Point(493, 12);
            this.btn_back.Name = "btn_back";
            this.btn_back.Size = new System.Drawing.Size(75, 23);
            this.btn_back.TabIndex = 0;
            this.btn_back.Text = "Go back";
            this.btn_back.UseVisualStyleBackColor = true;
            this.btn_back.Click += new System.EventHandler(this.btn_back_Click);
            // 
            // btn_send
            // 
            this.btn_send.Location = new System.Drawing.Point(493, 315);
            this.btn_send.Name = "btn_send";
            this.btn_send.Size = new System.Drawing.Size(75, 23);
            this.btn_send.TabIndex = 1;
            this.btn_send.Text = "Send";
            this.btn_send.UseVisualStyleBackColor = true;
            this.btn_send.Click += new System.EventHandler(this.btn_send_Click);
            // 
            // txt_msg
            // 
            this.txt_msg.Location = new System.Drawing.Point(12, 315);
            this.txt_msg.Name = "txt_msg";
            this.txt_msg.Size = new System.Drawing.Size(475, 20);
            this.txt_msg.TabIndex = 2;
            // 
            // lst_messages
            // 
            this.lst_messages.FormattingEnabled = true;
            this.lst_messages.HorizontalScrollbar = true;
            this.lst_messages.Location = new System.Drawing.Point(12, 12);
            this.lst_messages.Name = "lst_messages";
            this.lst_messages.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lst_messages.Size = new System.Drawing.Size(475, 290);
            this.lst_messages.TabIndex = 3;
            // 
            // RoomDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btn_back;
            this.ClientSize = new System.Drawing.Size(580, 350);
            this.ControlBox = false;
            this.Controls.Add(this.lst_messages);
            this.Controls.Add(this.txt_msg);
            this.Controls.Add(this.btn_send);
            this.Controls.Add(this.btn_back);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RoomDialog";
            this.Text = "chat";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_back;
        private System.Windows.Forms.Button btn_send;
        private System.Windows.Forms.TextBox txt_msg;
        public System.Windows.Forms.ListBox lst_messages;
    }
}