using System;
using System.Windows.Forms;
using System.Speech.Synthesis;
using System.Collections.ObjectModel;

namespace SynthIRC
{
    public partial class VoiceSetup : Form
    {
        private SpeechSynthesizer synth;
        private UserOptions options;

        public VoiceSetup(SpeechSynthesizer s, UserOptions o)
        {
            synth = s;
            this.options = o;
            InitializeComponent();
            ReadOnlyCollection<InstalledVoice> InstalledVoices = synth.GetInstalledVoices();
            int i = 0;
            foreach (InstalledVoice voice in InstalledVoices)
            {
                string str = voice.VoiceInfo.Name;
                comboBox1.Items.Add(str);
                if (str.Equals(options.Voice))
                {
                    comboBox1.SelectedIndex = i;
                    break;
                }
                i++;

            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            synth.SelectVoice(comboBox1.SelectedItem.ToString());
            numericUpDown2.Value = 100;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            synth.Rate = (int)numericUpDown1.Value;
        }

        private void Voice_Load(object sender, EventArgs e)
        {
            synth.Volume = (int)numericUpDown2.Value;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            options.Voice = comboBox1.SelectedText;
            this.Hide();
        }
    }
}