namespace X32MuteFollow
{
    partial class GUI
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GUI));
            this.buttonGoStop = new System.Windows.Forms.Button();
            this.textBoxMasterIP = new System.Windows.Forms.TextBox();
            this.textBoxFollowerIP = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxMasterState = new System.Windows.Forms.TextBox();
            this.textBoxFollowerState = new System.Windows.Forms.TextBox();
            this.checkBoxAll = new System.Windows.Forms.CheckBox();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.SuspendLayout();
            // 
            // buttonGoStop
            // 
            this.buttonGoStop.Location = new System.Drawing.Point(315, 43);
            this.buttonGoStop.Name = "buttonGoStop";
            this.buttonGoStop.Size = new System.Drawing.Size(84, 65);
            this.buttonGoStop.TabIndex = 0;
            this.buttonGoStop.Text = "Go";
            this.buttonGoStop.UseVisualStyleBackColor = true;
            // 
            // textBoxMasterIP
            // 
            this.textBoxMasterIP.Location = new System.Drawing.Point(172, 45);
            this.textBoxMasterIP.Name = "textBoxMasterIP";
            this.textBoxMasterIP.Size = new System.Drawing.Size(138, 26);
            this.textBoxMasterIP.TabIndex = 3;
            this.textBoxMasterIP.Text = "192.168.1.4";
            // 
            // textBoxFollowerIP
            // 
            this.textBoxFollowerIP.Location = new System.Drawing.Point(172, 82);
            this.textBoxFollowerIP.Name = "textBoxFollowerIP";
            this.textBoxFollowerIP.Size = new System.Drawing.Size(138, 26);
            this.textBoxFollowerIP.TabIndex = 4;
            this.textBoxFollowerIP.Text = "192.168.1.5";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(144, 20);
            this.label1.TabIndex = 5;
            this.label1.Text = "Master IP Address:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 85);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(154, 20);
            this.label2.TabIndex = 6;
            this.label2.Text = "Follower IP Address:";
            // 
            // textBoxMasterState
            // 
            this.textBoxMasterState.Location = new System.Drawing.Point(405, 45);
            this.textBoxMasterState.Name = "textBoxMasterState";
            this.textBoxMasterState.Size = new System.Drawing.Size(308, 26);
            this.textBoxMasterState.TabIndex = 7;
            this.textBoxMasterState.Text = "Disconnected";
            // 
            // textBoxFollowerState
            // 
            this.textBoxFollowerState.Location = new System.Drawing.Point(405, 82);
            this.textBoxFollowerState.Name = "textBoxFollowerState";
            this.textBoxFollowerState.Size = new System.Drawing.Size(308, 26);
            this.textBoxFollowerState.TabIndex = 8;
            this.textBoxFollowerState.Text = "Disconnected";
            // 
            // checkBoxAll
            // 
            this.checkBoxAll.AutoSize = true;
            this.checkBoxAll.Location = new System.Drawing.Point(642, 303);
            this.checkBoxAll.Name = "checkBoxAll";
            this.checkBoxAll.Size = new System.Drawing.Size(52, 24);
            this.checkBoxAll.TabIndex = 66;
            this.checkBoxAll.Text = "All";
            this.checkBoxAll.UseVisualStyleBackColor = true;
            this.checkBoxAll.CheckedChanged += new System.EventHandler(this.checkBoxAll_CheckedChanged);
            // 
            // notifyIcon
            // 
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "notifyIcon";
            this.notifyIcon.Visible = true;
            this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // GUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(735, 462);
            this.Controls.Add(this.checkBoxAll);
            this.Controls.Add(this.textBoxFollowerState);
            this.Controls.Add(this.textBoxMasterState);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxFollowerIP);
            this.Controls.Add(this.textBoxMasterIP);
            this.Controls.Add(this.buttonGoStop);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "GUI";
            this.Text = "X32 Mute Follow";
            this.Resize += new System.EventHandler(this.GUI_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonGoStop;
        private System.Windows.Forms.TextBox textBoxMasterIP;
        private System.Windows.Forms.TextBox textBoxFollowerIP;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxMasterState;
        private System.Windows.Forms.TextBox textBoxFollowerState;
        private System.Windows.Forms.CheckBox checkBoxAll;
        private System.Windows.Forms.NotifyIcon notifyIcon;
    }
}

