using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Speech.Synthesis;
using System.Net.Sockets;

namespace SynthIRC
{

    public partial class MainForm : Form
    {
        delegate void AddTextCallback(string text);
        delegate void SetBoldCallback();
        delegate void UnsetBoldCallback();
        private VoiceSetup dialog = null;
        private IRCSetup ircSetup = null;
        private Socket socket;
        private Boolean connected;
        private UserOptions options;
        private System.Text.ASCIIEncoding encoding;
        private NetworkStream stream;
        private SpeechSynthesizer synth;
        private static int BUFSIZE = 8192;
        private byte[] buffer = new byte[BUFSIZE];
        private int prefixLength;
        private bool motd;
        private Logger logger;

        public MainForm()
        {
            InitializeComponent();
            synth = new SpeechSynthesizer();
            //socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            connected = false;
            options = new UserOptions();
            encoding = new System.Text.ASCIIEncoding();
            checkBox1.Checked = options.AutoRead;
            button1.Enabled = !(checkBox1.Checked);
            logger = new Logger("synthIRC.logdata");
        }

        private void button1_Click(object sender, EventArgs e)
        {

            synth.SetOutputToDefaultAudioDevice();
            synth.SelectVoice(options.Voice);
            try
            {
                synth.SpeakAsync(textBox1.Text);
            }
            catch (Exception exp)
            {
                MessageBox.Show("Při syntéze řeči vznikla výjimka: " + exp, "Chyba");
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void disconnect()
        {
            string s = "quit\n";
            byte[] buf = encoding.GetBytes(s);
            socket.Send(buf);
            
            connected = false;
            //backgroundWorker1.
            socket.Disconnect(false);
            socket = null;
            //stream = null;
            //backgroundWorker1.CancelAsync();
            button2.Text = "Připojit";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (connected)
            {
                disconnect();
            }
            else
            {
                try
                {
                    socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socket.Connect(options.Server, options.Port);
                    stream = new NetworkStream(socket);
                    prefixLength = options.Server.Length + options.Nick.Length + 7;
                    connected = true;
                    
                    joinChannels();
                }
                catch (Exception exp)
                {
                    MessageBox.Show("Připojení se nezdařilo:\n" + exp, "Chyba");
                    connected = false;
                }
                //ThreadStart ts = new ThreadStart(MainForm.run);
                //Thread t = new Thread(this.run);
                //t.Start();
                if (connected)
                {
                    button2.Text = "Odpojit";
                    sendHandshake();
                }
            }
        }

        private void AddText(string text)
        {
            if (this.textBox1.InvokeRequired)
            {
                AddTextCallback d = new AddTextCallback(AddText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBox1.AppendText(text);
            }
        }

        private void SetBold()
        {
            if (this.textBox1.InvokeRequired)
            {
                SetBoldCallback d = new SetBoldCallback(SetBold);
                this.Invoke(d);
            }
            else
            {
                Font oldFont = textBox1.SelectionFont;
                Font newFont = new Font(oldFont, FontStyle.Bold);
                textBox1.SelectionFont = newFont;
            }
        }

        private void UnsetBold()
        {
            if (this.textBox1.InvokeRequired)
            {
                UnsetBoldCallback d = new UnsetBoldCallback(UnsetBold);
                this.Invoke(d);
            }
            else
            {
                Font oldFont = textBox1.SelectionFont;
                Font newFont = new Font(oldFont, FontStyle.Regular);
                textBox1.SelectionFont = newFont;
            }
        }

        private void sendPong(string server)
        {
            if (connected)
            {
                string s = "PONG :" + server + "\n";
                byte[] b = encoding.GetBytes(s);
                socket.Send(b);
            }
        }

        private void processText(string text)
        {
            Boolean added = false;
            text = text.Trim();
            string[] lines = text.Split('\n');
            //if (text.Contains("\0")) text = text.Replace ('\0', '');
            //this.AddText("Prijat text:\n");
            foreach (string lineIt in lines)
            {

                added = false;
                string line = lineIt;
                logger.appendLine(line);
                //Console.WriteLine(line);
                line = line.Trim();
                if (line.StartsWith("PING :"))
                {
                    sendPong(text.Substring(6, text.Length - 6));
                    //added = true;
                }
                else if ((line.StartsWith(":")) && (line.Contains(options.Nick)))
                {
                    //line = line.Substring(prefixLength + 1);
                    
                    /*line = line.Replace(":" + options.Server + " ", "");
                    line = line.Replace(" " + options.Nick + " ", "");
                    line = line.Substring(3);
                    line = line.Replace("\x16", "\t");
                     */ 
                    if (line.StartsWith(":")) {
                        line = line.Substring(1);
                        if (line.Contains(":")) {
                                line = line.Substring(line.IndexOf(":"));
                        }
                    }
                    if (line.Contains(options.Server + " " + "Message of the Day")) {
                        motd = options.SkipMotd;
                    }
                }
                if (!motd)
                {
                    if (line.Contains("\x02"))
                    {
                        string[] splitLines = line.Split(new Char[] { '\x02' });
                        for (int i = 0; i < splitLines.Length; i++)
                        {
                            if ((i & 1) == 0) //suda je tucne
                            {
                                this.SetBold();
                            }
                            else this.UnsetBold();
                            this.AddText(splitLines[i].Replace("\x02", ""));

                        }
                        this.AddText("\n");
                        added = true;
                    }
                    if (!added)
                    {
                        if (options.AutoRead) this.synth.SpeakAsync(line);
                        this.AddText(line + "\n");
                    }
                }
                if (line.StartsWith(":End of MOTD command"))
                {
                    motd = false;
                }
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

            while (connected)
            {
                //textBox1.AppendText("run() - pripojeno");
                string text;
                try
                {
                    int i = stream.Read(buffer, 0, BUFSIZE);
                    text = System.Text.ASCIIEncoding.ASCII.GetString(buffer, 0, i);
                    this.processText(text);
                }
                catch (Exception exp)
                {
                    MessageBox.Show("Výjimka při přijímání dat:\n" + exp, "Chyba");
                    socket.Close();
                    connected = false;
                    return;
                }
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void sendHandshake()
        {

            String nick = "NICK " + options.Nick + "\n";
            String user = "USER " + options.Username + " 0 * " +
                ":" + options.Nick + "\n";
            StringBuilder sb = new StringBuilder();
            sb.Append(nick).Append(user);
            
            byte[] buf = encoding.GetBytes(sb.ToString());
            if (connected)
            {
                socket.Send(buf);
            }
            backgroundWorker1.RunWorkerAsync();
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (connected)
                if (e.KeyCode == Keys.Enter)
                {
                    string s = textBox2.Text;
                    if (s.ToLower().Trim().Equals("quit"))
                    {
                        disconnect();
                        return;
                    }
                    s = s + "\n";
                    this.AddText("\n" + s);
                    byte[] b = encoding.GetBytes(s);
                    try
                    {
                        socket.Send(b);
                    }
                    catch (Exception exp)
                    {
                        MessageBox.Show("Výjimka při odesílání dat:\n" + exp, "Chyba");
                        socket.Close();
                        connected = false;
                    }
                    textBox2.Text = "";
                }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dialog == null) dialog = new VoiceSetup(synth, options);
            dialog.Show(this);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            options.Save();
            if (socket != null) socket.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            synth.SpeakAsyncCancelAll();
        }

        private void joinChannels()
        {
            foreach (string chname in options.Channels)
            {
                string s = "join " + chname + "\n";
                byte[] buf = encoding.GetBytes(s);
                socket.Send(buf);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            options.AutoRead = checkBox1.Checked;
            if (checkBox1.Checked) button1.Enabled = false;
            else button1.Enabled = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (ircSetup == null) ircSetup = new IRCSetup(options);
            ircSetup.Show(this);
        }
    }
}