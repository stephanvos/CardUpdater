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
            this.groupBoxCard = new System.Windows.Forms.GroupBox();
            this.btnReadAIDs = new System.Windows.Forms.Button();
            this.listBoxAIDs = new System.Windows.Forms.ListBox();
            this.lblCardInfo = new System.Windows.Forms.Label();
            this.groupBoxFiles = new System.Windows.Forms.GroupBox();
            this.txtFileData = new System.Windows.Forms.TextBox();
            this.listBoxFiles = new System.Windows.Forms.ListBox();
            this.lblFileInfo = new System.Windows.Forms.Label();
            this.groupBoxAuth = new System.Windows.Forms.GroupBox();
            this.lblKeyNumber = new System.Windows.Forms.Label();
            this.numKeyNumber = new System.Windows.Forms.NumericUpDown();
            this.lblKey = new System.Windows.Forms.Label();
            this.txtKey = new System.Windows.Forms.TextBox();
            this.btnAuthenticate = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBoxReader.SuspendLayout();
            this.groupBoxCard.SuspendLayout();
            this.groupBoxFiles.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.groupBoxAuth.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numKeyNumber)).BeginInit();
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
            // groupBoxCard
            // 
            this.groupBoxCard.Controls.Add(this.lblCardInfo);
            this.groupBoxCard.Controls.Add(this.listBoxAIDs);
            this.groupBoxCard.Controls.Add(this.btnReadAIDs);
            this.groupBoxCard.Controls.Add(this.groupBoxFiles);
            this.groupBoxCard.Enabled = false;
            this.groupBoxCard.Location = new System.Drawing.Point(12, 145);
            this.groupBoxCard.Name = "groupBoxCard";
            this.groupBoxCard.Size = new System.Drawing.Size(776, 270);
            this.groupBoxCard.TabIndex = 1;
            this.groupBoxCard.TabStop = false;
            this.groupBoxCard.Text = "Card Operations";
            // 
            // btnReadAIDs
            // 
            this.btnReadAIDs.Location = new System.Drawing.Point(15, 25);
            this.btnReadAIDs.Name = "btnReadAIDs";
            this.btnReadAIDs.Size = new System.Drawing.Size(150, 30);
            this.btnReadAIDs.TabIndex = 0;
            this.btnReadAIDs.Text = "Read Application IDs";
            this.btnReadAIDs.UseVisualStyleBackColor = true;
            this.btnReadAIDs.Click += new System.EventHandler(this.btnReadAIDs_Click);
            // 
            // listBoxAIDs
            // 
            this.listBoxAIDs.FormattingEnabled = true;
            this.listBoxAIDs.ItemHeight = 15;
            this.listBoxAIDs.Location = new System.Drawing.Point(15, 90);
            this.listBoxAIDs.Name = "listBoxAIDs";
            this.listBoxAIDs.Size = new System.Drawing.Size(200, 154);
            this.listBoxAIDs.TabIndex = 1;
            this.listBoxAIDs.SelectedIndexChanged += new System.EventHandler(this.listBoxAIDs_SelectedIndexChanged);
            // 
            // lblCardInfo
            // 
            this.lblCardInfo.AutoSize = true;
            this.lblCardInfo.Location = new System.Drawing.Point(15, 65);
            this.lblCardInfo.Name = "lblCardInfo";
            this.lblCardInfo.Size = new System.Drawing.Size(200, 15);
            this.lblCardInfo.TabIndex = 2;
            this.lblCardInfo.Text = "Place card on reader and click Read";
            // 
            // groupBoxFiles
            // 
            this.groupBoxFiles.Controls.Add(this.txtFileData);
            this.groupBoxFiles.Controls.Add(this.listBoxFiles);
            this.groupBoxFiles.Controls.Add(this.lblFileInfo);
            this.groupBoxFiles.Controls.Add(this.groupBoxAuth);
            this.groupBoxFiles.Location = new System.Drawing.Point(230, 25);
            this.groupBoxFiles.Name = "groupBoxFiles";
            this.groupBoxFiles.Size = new System.Drawing.Size(530, 230);
            this.groupBoxFiles.TabIndex = 3;
            this.groupBoxFiles.TabStop = false;
            this.groupBoxFiles.Text = "Files";
            // 
            // groupBoxAuth
            // 
            this.groupBoxAuth.Controls.Add(this.lblKeyNumber);
            this.groupBoxAuth.Controls.Add(this.numKeyNumber);
            this.groupBoxAuth.Controls.Add(this.lblKey);
            this.groupBoxAuth.Controls.Add(this.txtKey);
            this.groupBoxAuth.Controls.Add(this.btnAuthenticate);
            this.groupBoxAuth.Location = new System.Drawing.Point(175, 15);
            this.groupBoxAuth.Name = "groupBoxAuth";
            this.groupBoxAuth.Size = new System.Drawing.Size(340, 85);
            this.groupBoxAuth.TabIndex = 3;
            this.groupBoxAuth.TabStop = false;
            this.groupBoxAuth.Text = "Authentication (for encrypted files)";
            // 
            // lblKeyNumber
            // 
            this.lblKeyNumber.AutoSize = true;
            this.lblKeyNumber.Location = new System.Drawing.Point(10, 25);
            this.lblKeyNumber.Name = "lblKeyNumber";
            this.lblKeyNumber.Size = new System.Drawing.Size(70, 15);
            this.lblKeyNumber.TabIndex = 0;
            this.lblKeyNumber.Text = "Key Number:";
            // 
            // numKeyNumber
            // 
            this.numKeyNumber.Location = new System.Drawing.Point(90, 22);
            this.numKeyNumber.Maximum = new decimal(new int[] { 13, 0, 0, 0 });
            this.numKeyNumber.Name = "numKeyNumber";
            this.numKeyNumber.Size = new System.Drawing.Size(60, 23);
            this.numKeyNumber.TabIndex = 1;
            this.numKeyNumber.Value = new decimal(new int[] { 0, 0, 0, 0 });
            // 
            // lblKey
            // 
            this.lblKey.AutoSize = true;
            this.lblKey.Location = new System.Drawing.Point(10, 52);
            this.lblKey.Name = "lblKey";
            this.lblKey.Size = new System.Drawing.Size(74, 15);
            this.lblKey.TabIndex = 2;
            this.lblKey.Text = "Key (16 hex):";
            // 
            // txtKey
            // 
            this.txtKey.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtKey.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtKey.Location = new System.Drawing.Point(90, 49);
            this.txtKey.MaxLength = 47;
            this.txtKey.Name = "txtKey";
            this.txtKey.PlaceholderText = "00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00";
            this.txtKey.Size = new System.Drawing.Size(160, 22);
            this.txtKey.TabIndex = 3;
            this.txtKey.Text = "00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00";
            // 
            // btnAuthenticate
            // 
            this.btnAuthenticate.Location = new System.Drawing.Point(260, 20);
            this.btnAuthenticate.Name = "btnAuthenticate";
            this.btnAuthenticate.Size = new System.Drawing.Size(70, 50);
            this.btnAuthenticate.TabIndex = 4;
            this.btnAuthenticate.Text = "Auth";
            this.btnAuthenticate.UseVisualStyleBackColor = true;
            this.btnAuthenticate.Click += new System.EventHandler(this.btnAuthenticate_Click);
            // 
            // lblFileInfo
            // 
            this.lblFileInfo.AutoSize = true;
            this.lblFileInfo.Location = new System.Drawing.Point(10, 105);
            this.lblFileInfo.Name = "lblFileInfo";
            this.lblFileInfo.Size = new System.Drawing.Size(180, 15);
            this.lblFileInfo.TabIndex = 0;
            this.lblFileInfo.Text = "Select an application to view files";
            // 
            // listBoxFiles
            // 
            this.listBoxFiles.FormattingEnabled = true;
            this.listBoxFiles.ItemHeight = 15;
            this.listBoxFiles.Location = new System.Drawing.Point(10, 125);
            this.listBoxFiles.Name = "listBoxFiles";
            this.listBoxFiles.Size = new System.Drawing.Size(150, 94);
            this.listBoxFiles.TabIndex = 1;
            this.listBoxFiles.SelectedIndexChanged += new System.EventHandler(this.listBoxFiles_SelectedIndexChanged);
            // 
            // txtFileData
            // 
            this.txtFileData.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtFileData.Location = new System.Drawing.Point(175, 105);
            this.txtFileData.Multiline = true;
            this.txtFileData.Name = "txtFileData";
            this.txtFileData.ReadOnly = true;
            this.txtFileData.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtFileData.Size = new System.Drawing.Size(340, 114);
            this.txtFileData.TabIndex = 2;
            this.txtFileData.WordWrap = false;
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
            this.Controls.Add(this.groupBoxCard);
            this.Controls.Add(this.groupBoxReader);
            this.Name = "FormMain";
            this.Text = "Card Updater - Mifare DESFire";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.groupBoxReader.ResumeLayout(false);
            this.groupBoxReader.PerformLayout();
            this.groupBoxCard.ResumeLayout(false);
            this.groupBoxCard.PerformLayout();
            this.groupBoxFiles.ResumeLayout(false);
            this.groupBoxFiles.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.groupBoxAuth.ResumeLayout(false);
            this.groupBoxAuth.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numKeyNumber)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
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
