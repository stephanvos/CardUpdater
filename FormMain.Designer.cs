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
            groupBoxReader = new GroupBox();
            lblReaderStatus = new Label();
            btnConnect = new Button();
            btnRefreshReaders = new Button();
            comboBoxReaders = new ComboBox();
            groupBoxCard = new GroupBox();
            lblCardInfo = new Label();
            listBoxAIDs = new ListBox();
            btnReadAIDs = new Button();
            groupBoxFiles = new GroupBox();
            txtFileData = new TextBox();
            listBoxFiles = new ListBox();
            lblFileInfo = new Label();
            groupBoxAuth = new GroupBox();
            lblKeyNumber = new Label();
            numKeyNumber = new NumericUpDown();
            lblKey = new Label();
            txtKey = new TextBox();
            btnAuthenticate = new Button();
            statusStrip = new StatusStrip();
            toolStripStatusLabel = new ToolStripStatusLabel();
            groupBoxReader.SuspendLayout();
            groupBoxCard.SuspendLayout();
            groupBoxFiles.SuspendLayout();
            groupBoxAuth.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numKeyNumber).BeginInit();
            statusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // groupBoxReader
            // 
            groupBoxReader.Controls.Add(lblReaderStatus);
            groupBoxReader.Controls.Add(btnConnect);
            groupBoxReader.Controls.Add(btnRefreshReaders);
            groupBoxReader.Controls.Add(comboBoxReaders);
            groupBoxReader.Location = new Point(12, 12);
            groupBoxReader.Name = "groupBoxReader";
            groupBoxReader.Size = new Size(776, 68);
            groupBoxReader.TabIndex = 0;
            groupBoxReader.TabStop = false;
            groupBoxReader.Text = "Card Reader";
            // 
            // lblReaderStatus
            // 
            lblReaderStatus.AutoSize = true;
            lblReaderStatus.Location = new Point(675, 33);
            lblReaderStatus.Name = "lblReaderStatus";
            lblReaderStatus.Size = new Size(86, 15);
            lblReaderStatus.TabIndex = 3;
            lblReaderStatus.Text = "Not connected";
            // 
            // btnConnect
            // 
            btnConnect.Enabled = false;
            btnConnect.Location = new Point(555, 25);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(100, 30);
            btnConnect.TabIndex = 2;
            btnConnect.Text = "Connect";
            btnConnect.UseVisualStyleBackColor = true;
            btnConnect.Click += btnConnect_Click;
            // 
            // btnRefreshReaders
            // 
            btnRefreshReaders.Location = new Point(440, 25);
            btnRefreshReaders.Name = "btnRefreshReaders";
            btnRefreshReaders.Size = new Size(100, 30);
            btnRefreshReaders.TabIndex = 1;
            btnRefreshReaders.Text = "Refresh";
            btnRefreshReaders.UseVisualStyleBackColor = true;
            btnRefreshReaders.Click += btnRefreshReaders_Click;
            // 
            // comboBoxReaders
            // 
            comboBoxReaders.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxReaders.FormattingEnabled = true;
            comboBoxReaders.Location = new Point(15, 25);
            comboBoxReaders.Name = "comboBoxReaders";
            comboBoxReaders.Size = new Size(420, 23);
            comboBoxReaders.TabIndex = 0;
            // 
            // groupBoxCard
            // 
            groupBoxCard.Controls.Add(lblCardInfo);
            groupBoxCard.Controls.Add(listBoxAIDs);
            groupBoxCard.Controls.Add(btnReadAIDs);
            groupBoxCard.Controls.Add(groupBoxFiles);
            groupBoxCard.Enabled = false;
            groupBoxCard.Location = new Point(12, 86);
            groupBoxCard.Name = "groupBoxCard";
            groupBoxCard.Size = new Size(776, 270);
            groupBoxCard.TabIndex = 1;
            groupBoxCard.TabStop = false;
            groupBoxCard.Text = "Card Operations";
            // 
            // lblCardInfo
            // 
            lblCardInfo.AutoSize = true;
            lblCardInfo.Location = new Point(15, 65);
            lblCardInfo.Name = "lblCardInfo";
            lblCardInfo.Size = new Size(193, 15);
            lblCardInfo.TabIndex = 2;
            lblCardInfo.Text = "Place card on reader and click Read";
            // 
            // listBoxAIDs
            // 
            listBoxAIDs.FormattingEnabled = true;
            listBoxAIDs.Location = new Point(15, 90);
            listBoxAIDs.Name = "listBoxAIDs";
            listBoxAIDs.Size = new Size(200, 154);
            listBoxAIDs.TabIndex = 1;
            listBoxAIDs.SelectedIndexChanged += listBoxAIDs_SelectedIndexChanged;
            // 
            // btnReadAIDs
            // 
            btnReadAIDs.Location = new Point(15, 25);
            btnReadAIDs.Name = "btnReadAIDs";
            btnReadAIDs.Size = new Size(150, 30);
            btnReadAIDs.TabIndex = 0;
            btnReadAIDs.Text = "Read Application IDs";
            btnReadAIDs.UseVisualStyleBackColor = true;
            btnReadAIDs.Click += btnReadAIDs_Click;
            // 
            // groupBoxFiles
            // 
            groupBoxFiles.Controls.Add(txtFileData);
            groupBoxFiles.Controls.Add(listBoxFiles);
            groupBoxFiles.Controls.Add(lblFileInfo);
            groupBoxFiles.Controls.Add(groupBoxAuth);
            groupBoxFiles.Location = new Point(230, 25);
            groupBoxFiles.Name = "groupBoxFiles";
            groupBoxFiles.Size = new Size(530, 230);
            groupBoxFiles.TabIndex = 3;
            groupBoxFiles.TabStop = false;
            groupBoxFiles.Text = "Files";
            // 
            // txtFileData
            // 
            txtFileData.Font = new Font("Consolas", 9F);
            txtFileData.Location = new Point(175, 105);
            txtFileData.Multiline = true;
            txtFileData.Name = "txtFileData";
            txtFileData.ReadOnly = true;
            txtFileData.ScrollBars = ScrollBars.Both;
            txtFileData.Size = new Size(340, 114);
            txtFileData.TabIndex = 2;
            txtFileData.WordWrap = false;
            // 
            // listBoxFiles
            // 
            listBoxFiles.FormattingEnabled = true;
            listBoxFiles.Location = new Point(10, 125);
            listBoxFiles.Name = "listBoxFiles";
            listBoxFiles.Size = new Size(150, 94);
            listBoxFiles.TabIndex = 1;
            listBoxFiles.SelectedIndexChanged += listBoxFiles_SelectedIndexChanged;
            // 
            // lblFileInfo
            // 
            lblFileInfo.AutoSize = true;
            lblFileInfo.Location = new Point(6, 105);
            lblFileInfo.Name = "lblFileInfo";
            lblFileInfo.Size = new Size(181, 15);
            lblFileInfo.TabIndex = 0;
            lblFileInfo.Text = "Select an application to view files";
            // 
            // groupBoxAuth
            // 
            groupBoxAuth.Controls.Add(lblKeyNumber);
            groupBoxAuth.Controls.Add(numKeyNumber);
            groupBoxAuth.Controls.Add(lblKey);
            groupBoxAuth.Controls.Add(txtKey);
            groupBoxAuth.Controls.Add(btnAuthenticate);
            groupBoxAuth.Location = new Point(10, 15);
            groupBoxAuth.Name = "groupBoxAuth";
            groupBoxAuth.Size = new Size(505, 85);
            groupBoxAuth.TabIndex = 3;
            groupBoxAuth.TabStop = false;
            groupBoxAuth.Text = "Authentication (for encrypted files)";
            // 
            // lblKeyNumber
            // 
            lblKeyNumber.AutoSize = true;
            lblKeyNumber.Location = new Point(10, 25);
            lblKeyNumber.Name = "lblKeyNumber";
            lblKeyNumber.Size = new Size(76, 15);
            lblKeyNumber.TabIndex = 0;
            lblKeyNumber.Text = "Key Number:";
            // 
            // numKeyNumber
            // 
            numKeyNumber.Location = new Point(90, 22);
            numKeyNumber.Maximum = new decimal(new int[] { 13, 0, 0, 0 });
            numKeyNumber.Name = "numKeyNumber";
            numKeyNumber.Size = new Size(60, 23);
            numKeyNumber.TabIndex = 1;
            // 
            // lblKey
            // 
            lblKey.AutoSize = true;
            lblKey.Location = new Point(10, 52);
            lblKey.Name = "lblKey";
            lblKey.Size = new Size(73, 15);
            lblKey.TabIndex = 2;
            lblKey.Text = "Key (16 hex):";
            // 
            // txtKey
            // 
            txtKey.CharacterCasing = CharacterCasing.Upper;
            txtKey.Font = new Font("Consolas", 9F);
            txtKey.Location = new Point(90, 49);
            txtKey.MaxLength = 47;
            txtKey.Name = "txtKey";
            txtKey.PlaceholderText = "00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00";
            txtKey.Size = new Size(344, 22);
            txtKey.TabIndex = 3;
            txtKey.Text = "00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00";
            // 
            // btnAuthenticate
            // 
            btnAuthenticate.Location = new Point(402, 17);
            btnAuthenticate.Name = "btnAuthenticate";
            btnAuthenticate.Size = new Size(97, 26);
            btnAuthenticate.TabIndex = 4;
            btnAuthenticate.Text = "Auth";
            btnAuthenticate.UseVisualStyleBackColor = true;
            btnAuthenticate.Click += btnAuthenticate_Click;
            // 
            // statusStrip
            // 
            statusStrip.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel });
            statusStrip.Location = new Point(0, 428);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(800, 22);
            statusStrip.TabIndex = 1;
            statusStrip.Text = "statusStrip1";
            // 
            // toolStripStatusLabel
            // 
            toolStripStatusLabel.Name = "toolStripStatusLabel";
            toolStripStatusLabel.Size = new Size(39, 17);
            toolStripStatusLabel.Text = "Ready";
            // 
            // FormMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(statusStrip);
            Controls.Add(groupBoxCard);
            Controls.Add(groupBoxReader);
            Name = "FormMain";
            Text = "Card Updater - Mifare DESFire";
            FormClosing += FormMain_FormClosing;
            Load += FormMain_Load;
            groupBoxReader.ResumeLayout(false);
            groupBoxReader.PerformLayout();
            groupBoxCard.ResumeLayout(false);
            groupBoxCard.PerformLayout();
            groupBoxFiles.ResumeLayout(false);
            groupBoxFiles.PerformLayout();
            groupBoxAuth.ResumeLayout(false);
            groupBoxAuth.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numKeyNumber).EndInit();
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxReader;
        private System.Windows.Forms.ComboBox comboBoxReaders;
        private System.Windows.Forms.Button btnRefreshReaders;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Label lblReaderStatus;
        private System.Windows.Forms.GroupBox groupBoxCard;
        private System.Windows.Forms.Button btnReadAIDs;
        private System.Windows.Forms.ListBox listBoxAIDs;
        private System.Windows.Forms.Label lblCardInfo;
        private System.Windows.Forms.GroupBox groupBoxFiles;
        private System.Windows.Forms.Label lblFileInfo;
        private System.Windows.Forms.ListBox listBoxFiles;
        private System.Windows.Forms.TextBox txtFileData;
        private System.Windows.Forms.GroupBox groupBoxAuth;
        private System.Windows.Forms.Label lblKeyNumber;
        private System.Windows.Forms.NumericUpDown numKeyNumber;
        private System.Windows.Forms.Label lblKey;
        private System.Windows.Forms.TextBox txtKey;
        private System.Windows.Forms.Button btnAuthenticate;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
    }
}
