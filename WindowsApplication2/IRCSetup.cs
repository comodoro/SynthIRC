using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SynthIRC
{
    public partial class IRCSetup : Form
    {
        private UserOptions options;
        public IRCSetup(UserOptions options)
        {
            InitializeComponent();
            this.options = options;
            emailTextBox.Text = options.Mail;
            nickTextBox.Text = options.Nick;
            portUpDown.Value = options.Port;
            serverTextBox.Text = options.Server;
            usernameTextBox.Text = options.Username;
            //channelListView.
            foreach (string s in options.Channels) {
                channelListView.Items.Add(s);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            options.Mail = emailTextBox.Text;
            options.Nick = nickTextBox.Text;
            options.Port = (int)portUpDown.Value;
            options.Server = serverTextBox.Text;
            options.Username = usernameTextBox.Text;
            foreach (ListViewItem it in channelListView.Items) {
                options.Channels.Add(it.Name);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        
        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

    }
}