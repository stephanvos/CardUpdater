namespace CardUpdater
{
    partial class FormMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBoxReader = new System.Windows.Forms.GroupBox();
            this.lblReaderStatus = new System.Windows.Forms.Label();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnRefreshReaders = new System.Windows.Forms.Button();
            this.comboBoxReaders = new System.Windows.Forms.ComboBox();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBoxReader.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxReader
            // 
            this.groupBoxReader.Controls.Add(this.lblReaderStatus);
            this.groupBoxReader.Controls.Add(this.btnConnect);
            this.groupBoxReader.Controls.Add(this.btnRefreshReaders);
            this.groupBoxReader.Controls.Add(this.comboBoxReaders);
            this.groupBoxReader.Location = new System.Drawing.Point(12, 12);
            this.groupBoxReader.Name = "groupBoxReader";
            this.groupBoxReader.Size = new System.Drawing.Size(450, 120);
            this.groupBoxReader.TabIndex = 0;
            this.groupBoxReader.TabStop = false;
            this.groupBoxReader.Text = "Card Reader";
            // 
            // lblReaderStatus
            // 
            this.lblReaderStatus.AutoSize = true;
            this.lblReaderStatus.Location = new System.Drawing.Point(250, 68);
            this.lblReaderStatus.Name = "lblReaderStatus";
            this.lblReaderStatus.Size = new System.Drawing.Size(90, 15);
            this.lblReaderStatus.TabIndex = 3;
            this.lblReaderStatus.Text = "Not connected";
            // 
            // btnConnect
            // 
            this.btnConnect.Enabled = false;
            this.btnConnect.Location = new System.Drawing.Point(130, 60);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(100, 30);
            this.btnConnect.TabIndex = 2;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnRefreshReaders
            // 
            this.btnRefreshReaders.Location = new System.Drawing.Point(15, 60);
            this.btnRefreshReaders.Name = "btnRefreshReaders";
            this.btnRefreshReaders.Size = new System.Drawing.Size(100, 30);
            this.btnRefreshReaders.TabIndex = 1;
            this.btnRefreshReaders.Text = "Refresh";
            this.btnRefreshReaders.UseVisualStyleBackColor = true;
            this.btnRefreshReaders.Click += new System.EventHandler(this.btnRefreshReaders_Click);
            // 
            // comboBoxReaders
            // 
            this.comboBoxReaders.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxReaders.FormattingEnabled = true;
            this.comboBoxReaders.Location = new System.Drawing.Point(15, 25);
            this.comboBoxReaders.Name = "comboBoxReaders";
            this.comboBoxReaders.Size = new System.Drawing.Size(420, 23);
            this.comboBoxReaders.TabIndex = 0;
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 428);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(800, 22);
            this.statusStrip.TabIndex = 1;
            this.statusStrip.Text = "statusStrip1";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(39, 17);
            this.toolStripStatusLabel.Text = "Ready";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.groupBoxReader);
            this.Name = "FormMain";
            this.Text = "Card Updater - Mifare DESFire";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.groupBoxReader.ResumeLayout(false);
            this.groupBoxReader.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxReader;
        private System.Windows.Forms.ComboBox comboBoxReaders;
        private System.Windows.Forms.Button btnRefreshReaders;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Label lblReaderStatus;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
    }
}
