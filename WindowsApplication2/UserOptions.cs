using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections.Specialized;   
        
namespace SynthIRC
{
    public class UserOptions
    {
        private String server;
        private int port;
        private String nick;
        private String username;
        private String mail;
        private StringCollection channels;
        private String voice;
        private bool autoRead;
        private bool skipMotd;

        public UserOptions()
        {
            server = Properties.Settings.Default.Server;
            port = Properties.Settings.Default.Port;
            nick = Properties.Settings.Default.Nick;
            username = Properties.Settings.Default.User;
            mail = Properties.Settings.Default.Mail;
            channels = Properties.Settings.Default.Channels;
            voice = Properties.Settings.Default.PreferredVoice;
            autoRead = Properties.Settings.Default.AutoRead;
            skipMotd = Properties.Settings.Default.SkipMotd;
        }

        public String Server
        {
            get { return server; }
            set { server = value; }
        }

        public int Port
        {
            get { return port; }
            set { port = value; }
        }

        public String Username
        {
            get { return username; }
            set { username = value; }
        }

        public String Mail
        {
            get { return mail; }
            set { mail = value; }

        }

        public StringCollection Channels
        {
            get{return channels;}
            set{channels = value;}
        }

        public String Nick
        {
            get { return nick; }
            set { nick = value; }
        }

        public String Voice
        {
            get { return voice; }
            set { voice = value; }
        }

        public bool AutoRead
        {
            get { return autoRead; }
            set { autoRead = value; }
        }

        public bool SkipMotd
        {
            get { return skipMotd; }
            set { skipMotd = value; }
        }

        public void Save()
        {
            Properties.Settings.Default.Server = server;
            Properties.Settings.Default.Port = port;
            Properties.Settings.Default.Nick = nick;
            Properties.Settings.Default.User = username;
            Properties.Settings.Default.Mail = mail;
            Properties.Settings.Default.Channels = channels;
            Properties.Settings.Default.PreferredVoice = voice;
            Properties.Settings.Default.AutoRead = autoRead;
            Properties.Settings.Default.SkipMotd = skipMotd;
            Properties.Settings.Default.Save();
        }

    }
}
