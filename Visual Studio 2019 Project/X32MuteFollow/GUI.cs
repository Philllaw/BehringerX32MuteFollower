using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static X32MuteFollow.SaveLoad;

namespace X32MuteFollow
{
    public partial class GUI : Form
    {
        public CheckBox[] checkBoxes;
        public CheckBox[] radioButtonsMaster;
        public CheckBox[] radioButtonsFollower;
        Label[] labels;
        ProgramSettings Settings;

        public GUI(ProgramSettings settings)
        {
            Settings = settings;
            InitializeComponent();
            addCheckBoxes();
            this.notifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info; //Shows the info icon so the user doesn't think there is an error.
            this.notifyIcon.BalloonTipText = "Double-click tray icon to restore";
            this.notifyIcon.BalloonTipTitle = "X32 Mute Follower Minimised to tray";
            this.notifyIcon.Text = "X32 Mute Follower";
            //load settings
            for (int i = 0; i < 48; i++)
            {
                checkBoxes[i].Checked = Settings.Checkboxes[i];
            }
            this.textBoxMasterIP.Text = Settings.MasterIP;
            this.textBoxFollowerIP.Text = Settings.FollowerIP;
            checkIPs(null, null);
            addCheckBoxListeners();
            addTextBoxListeners();
        }
        
        private void addCheckBoxes()
        {
            checkBoxes = new CheckBox[48];
            radioButtonsMaster = new CheckBox[48];
            radioButtonsFollower = new CheckBox[48];
            labels = new Label[6];
            String[] labelText = { "1-8", "9-16", "17-24", "25-32", "Aux 1-8", "Fx 1-8" };

            for (int i = 0; i < 48; i++)
            {
                checkBoxes[i] = new CheckBox();
                checkBoxes[i].AutoSize = true;
                checkBoxes[i].Location = new Point(50 + 54 * (i % 8), 90 + 18 * (i / 8));
                checkBoxes[i].Size = new Size(22, 22);
                checkBoxes[i].UseVisualStyleBackColor = true;
                this.Controls.Add(checkBoxes[i]);

                radioButtonsMaster[i] = new CheckBox();
                radioButtonsMaster[i].AutoSize = true;
                radioButtonsMaster[i].Location = new Point(65 + 54 * (i % 8), 90 + 18 * (i / 8));
                radioButtonsMaster[i].Size = new Size(22, 22);
                radioButtonsMaster[i].Text = "";
                radioButtonsMaster[i].UseVisualStyleBackColor = true;
                radioButtonsMaster[i].Enabled = false;
                this.Controls.Add(radioButtonsMaster[i]);

                radioButtonsFollower[i] = new CheckBox();
                radioButtonsFollower[i].AutoSize = true;
                radioButtonsFollower[i].Location = new Point(80 + 54 * (i % 8), 90 + 18 * (i / 8));
                radioButtonsFollower[i].Size = new Size(22, 22);
                radioButtonsFollower[i].Text = "";
                radioButtonsFollower[i].UseVisualStyleBackColor = true;
                radioButtonsFollower[i].Enabled = false;
                this.Controls.Add(radioButtonsFollower[i]);
            }

            for (int i = 0; i < 6; i++)
            {
                labels[i] = new Label();
                labels[i].Location = new System.Drawing.Point(5, 90 + 18 * i);
                labels[i].Size = new System.Drawing.Size(45, 20);
                labels[i].TabIndex = 12;
                labels[i].Text = labelText[i];
                labels[i].TextAlign = ContentAlignment.TopRight;
                this.Controls.Add(labels[i]);
            }

        }

        private void addCheckBoxListeners()
        {
            for (int i = 0; i < 48; i++)
            {
                checkBoxes[i].CheckedChanged += new System.EventHandler(this.checkBoxChanged);
            }
        }

        private void addTextBoxListeners()
        {
            this.textBoxMasterIP.TextChanged += new System.EventHandler(this.checkIPs);
            this.textBoxFollowerIP.TextChanged += new System.EventHandler(this.checkIPs);
        }


        private void checkBoxChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < 48; i++)
            {
                Settings.Checkboxes[i] = checkBoxes[i].Checked;
            }
            saveToFile(Settings);
        }

        public void listenToButton(EventHandler handler)
        {
            this.buttonGoStop.Click += new System.EventHandler(handler);
        }

        private void checkIPs(object sender, EventArgs e)
        {
            try
            {
                IPAddress.Parse(textBoxMasterIP.Text);
                textBoxMasterIP.ForeColor = Color.Black;
                try
                {
                    IPAddress.Parse(textBoxFollowerIP.Text);
                    textBoxFollowerIP.ForeColor = Color.Black;
                    buttonGoStop.Enabled = true;
                    Settings.MasterIP = textBoxMasterIP.Text;
                    Settings.FollowerIP = textBoxFollowerIP.Text;
                    saveToFile(Settings);
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                    textBoxFollowerIP.ForeColor = Color.Red;
                    buttonGoStop.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                textBoxMasterIP.ForeColor = Color.Red;
                buttonGoStop.Enabled = false;
            }
        }

        internal void setRunning(bool runningState)
        {
            if (runningState)
            {
                buttonGoStop.Text = "Stop";
                textBoxMasterIP.Enabled = false;
                textBoxFollowerIP.Enabled = false;
            }
            else
            {
                buttonGoStop.Text = "Go";
                textBoxMasterIP.Enabled = true;
                textBoxFollowerIP.Enabled = true;
            }
            Settings.Running = runningState;
            saveToFile(Settings);
        }

        private void checkBoxAll_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < 48; i++)
            {
                checkBoxes[i].Checked = checkBoxAll.Checked;
            }

        }

        public void setStatusMessageMaster(string message)
        {
            if (InvokeRequired)
            {
                try
                {
                    this.Invoke(new Action<string>(setStatusMessageMaster), new object[] { message });
                }
                catch (Exception) { }
                return;
            }
            this.textBoxMasterState.Text = message;
        }

        internal void setStatusMessageFollower(string message)
        {
            if (InvokeRequired)
            {
                try
                {
                    this.Invoke(new Action<string>(setStatusMessageFollower), new object[] { message });
                }
                catch (Exception) { }
                return;
            }
            textBoxFollowerState.Text = message;
        }

        internal void updateMutesMaster(bool[] mutes)
        {
            if (InvokeRequired)
            {
                try
                {
                    this.Invoke(new Action<bool[]>(updateMutesMaster), new object[] { mutes });
                }
                catch (Exception) { }
                return;
            }
            for (int i = 0; i < 32; i++)
            {
                radioButtonsMaster[i].Checked = mutes[i];
            }
        }

        internal void updateAMutesMaster(bool[] mutes)
        {
            if (InvokeRequired)
            {
                try
                {
                    this.Invoke(new Action<bool[]>(updateAMutesMaster), new object[] { mutes });
                }
                catch (Exception) { }
                return;
            }
            for (int i = 0; i < 8; i++)
            {
                radioButtonsMaster[i + 32].Checked = mutes[i];
            }
        }

        internal void updateFMutesMaster(bool[] mutes)
        {
            if (InvokeRequired)
            {
                try
                {
                    this.Invoke(new Action<bool[]>(updateFMutesMaster), new object[] { mutes });
                }
                catch (Exception) { }
                return;
            }
            for (int i = 0; i < 8; i++)
            {
                radioButtonsMaster[i + 40].Checked = mutes[i];
            }
        }

        internal void updateMutesFollower(bool[] mutes)
        {
            if (InvokeRequired)
            {
                try
                {
                    this.Invoke(new Action<bool[]>(updateMutesFollower), new object[] { mutes });
                }
                catch (Exception) { }
                return;
            }
            for (int i = 0; i < 32; i++)
            {
                radioButtonsFollower[i].Checked = mutes[i];
            }
        }

        internal void updateAMutesFollower(bool[] mutes)
        {
            if (InvokeRequired)
            {
                try
                {
                    this.Invoke(new Action<bool[]>(updateAMutesFollower), new object[] { mutes });
                }
                catch (Exception) { }
                return;
            }
            for (int i = 0; i < 8; i++)
            {
                radioButtonsFollower[i + 32].Checked = mutes[i];
            }
        }

        internal void updateFMutesFollower(bool[] mutes)
        {
            if (InvokeRequired)
            {
                try
                {
                    this.Invoke(new Action<bool[]>(updateFMutesFollower), new object[] { mutes });
                }
                catch (Exception) { }
                return;
            }
            for (int i = 0; i < 8; i++)
            {
                radioButtonsFollower[i + 40].Checked = mutes[i];
            }
        }

        private void GUI_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                notifyIcon.Visible = true;
                notifyIcon.ShowBalloonTip(3000);
                this.ShowInTaskbar = false;
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            notifyIcon.Visible = false;
        }

        public void listenToFormClosing(FormClosedEventHandler handler)
        {
            this.FormClosed += handler;
        }
    }
}
