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
       groupBoxSelection = new GroupBox();
   listBoxFiles = new ListBox();
 listBoxAIDs = new ListBox();
    lblFiles = new Label();
       lblAIDs = new Label();
   btnReadAIDs = new Button();
          groupBoxOperations = new GroupBox();
        txtFileData = new TextBox();
            lblFileData = new Label();
            groupBoxAuth = new GroupBox();
            btnAuthenticate = new Button();
            txtKey = new TextBox();
       lblKey = new Label();
    numKeyNumber = new NumericUpDown();
         lblKeyNumber = new Label();
            lblSelectedFile = new Label();
            lblSelectedAID = new Label();
    statusStrip = new StatusStrip();
  toolStripStatusLabel = new ToolStripStatusLabel();
        groupBoxReader.SuspendLayout();
        groupBoxSelection.SuspendLayout();
            groupBoxOperations.SuspendLayout();
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
      groupBoxReader.Size = new Size(960, 68);
            groupBoxReader.TabIndex = 0;
         groupBoxReader.TabStop = false;
     groupBoxReader.Text = "Card Reader";
       // 
   // lblReaderStatus
            // 
            lblReaderStatus.AutoSize = true;
     lblReaderStatus.Location = new Point(860, 33);
  lblReaderStatus.Name = "lblReaderStatus";
            lblReaderStatus.Size = new Size(86, 15);
      lblReaderStatus.TabIndex = 3;
            lblReaderStatus.Text = "Not connected";
    // 
  // btnConnect
            // 
     btnConnect.Enabled = false;
            btnConnect.Location = new Point(740, 25);
btnConnect.Name = "btnConnect";
    btnConnect.Size = new Size(100, 30);
     btnConnect.TabIndex = 2;
         btnConnect.Text = "Connect";
       btnConnect.UseVisualStyleBackColor = true;
            btnConnect.Click += btnConnect_Click;
 // 
            // btnRefreshReaders
     // 
            btnRefreshReaders.Location = new Point(625, 25);
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
    comboBoxReaders.Location = new Point(15, 27);
            comboBoxReaders.Name = "comboBoxReaders";
       comboBoxReaders.Size = new Size(600, 23);
            comboBoxReaders.TabIndex = 0;
 // 
        // groupBoxSelection
       // 
        groupBoxSelection.Controls.Add(listBoxFiles);
 groupBoxSelection.Controls.Add(listBoxAIDs);
 groupBoxSelection.Controls.Add(lblFiles);
  groupBoxSelection.Controls.Add(lblAIDs);
      groupBoxSelection.Controls.Add(btnReadAIDs);
            groupBoxSelection.Enabled = false;
   groupBoxSelection.Location = new Point(12, 86);
            groupBoxSelection.Name = "groupBoxSelection";
      groupBoxSelection.Size = new Size(960, 200);
        groupBoxSelection.TabIndex = 1;
       groupBoxSelection.TabStop = false;
            groupBoxSelection.Text = "Selection - Applications & Files";
      // 
 // listBoxFiles
      // 
            listBoxFiles.FormattingEnabled = true;
    listBoxFiles.ItemHeight = 15;
   listBoxFiles.Location = new Point(505, 62);
            listBoxFiles.Name = "listBoxFiles";
     listBoxFiles.Size = new Size(440, 124);
            listBoxFiles.TabIndex = 4;
       listBoxFiles.SelectedIndexChanged += listBoxFiles_SelectedIndexChanged;
    // 
      // listBoxAIDs
       // 
            listBoxAIDs.FormattingEnabled = true;
            listBoxAIDs.ItemHeight = 15;
            listBoxAIDs.Location = new Point(15, 62);
            listBoxAIDs.Name = "listBoxAIDs";
            listBoxAIDs.Size = new Size(475, 124);
   listBoxAIDs.TabIndex = 2;
   listBoxAIDs.SelectedIndexChanged += listBoxAIDs_SelectedIndexChanged;
       // 
    // lblFiles
            // 
   lblFiles.AutoSize = true;
    lblFiles.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblFiles.Location = new Point(505, 40);
        lblFiles.Name = "lblFiles";
     lblFiles.Size = new Size(33, 15);
            lblFiles.TabIndex = 3;
   lblFiles.Text = "Files";
         // 
            // lblAIDs
       // 
      lblAIDs.AutoSize = true;
     lblAIDs.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
      lblAIDs.Location = new Point(15, 40);
lblAIDs.Name = "lblAIDs";
       lblAIDs.Size = new Size(90, 15);
  lblAIDs.TabIndex = 1;
            lblAIDs.Text = "Application IDs";
            // 
      // btnReadAIDs
       // 
    btnReadAIDs.Location = new Point(15, 22);
     btnReadAIDs.Name = "btnReadAIDs";
         btnReadAIDs.Size = new Size(150, 23);
            btnReadAIDs.TabIndex = 0;
            btnReadAIDs.Text = "Read Applications";
            btnReadAIDs.UseVisualStyleBackColor = true;
         btnReadAIDs.Click += btnReadAIDs_Click;
  // 
            // groupBoxOperations
 // 
            groupBoxOperations.Controls.Add(txtFileData);
            groupBoxOperations.Controls.Add(lblFileData);
      groupBoxOperations.Controls.Add(groupBoxAuth);
            groupBoxOperations.Controls.Add(lblSelectedFile);
            groupBoxOperations.Controls.Add(lblSelectedAID);
groupBoxOperations.Enabled = false;
      groupBoxOperations.Location = new Point(12, 292);
        groupBoxOperations.Name = "groupBoxOperations";
        groupBoxOperations.Size = new Size(960, 320);
   groupBoxOperations.TabIndex = 2;
        groupBoxOperations.TabStop = false;
            groupBoxOperations.Text = "Read File Data";
            // 
    // txtFileData
 // 
       txtFileData.Font = new Font("Consolas", 9F);
            txtFileData.Location = new Point(15, 155);
            txtFileData.Multiline = true;
 txtFileData.Name = "txtFileData";
          txtFileData.ReadOnly = true;
            txtFileData.ScrollBars = ScrollBars.Both;
     txtFileData.Size = new Size(930, 150);
 txtFileData.TabIndex = 4;
  txtFileData.WordWrap = false;
            // 
         // lblFileData
            // 
   lblFileData.AutoSize = true;
lblFileData.Location = new Point(15, 130);
            lblFileData.Name = "lblFileData";
          lblFileData.Size = new Size(56, 15);
            lblFileData.TabIndex = 3;
            lblFileData.Text = "File Data:";
 // 
      // groupBoxAuth
    // 
    groupBoxAuth.Controls.Add(btnAuthenticate);
          groupBoxAuth.Controls.Add(txtKey);
            groupBoxAuth.Controls.Add(lblKey);
         groupBoxAuth.Controls.Add(numKeyNumber);
  groupBoxAuth.Controls.Add(lblKeyNumber);
  groupBoxAuth.Location = new Point(15, 50);
            groupBoxAuth.Name = "groupBoxAuth";
   groupBoxAuth.Size = new Size(930, 70);
            groupBoxAuth.TabIndex = 2;
  groupBoxAuth.TabStop = false;
groupBoxAuth.Text = "Authentication (required for encrypted files)";
         // 
            // btnAuthenticate
            // 
     btnAuthenticate.Location = new Point(820, 25);
        btnAuthenticate.Name = "btnAuthenticate";
            btnAuthenticate.Size = new Size(100, 30);
  btnAuthenticate.TabIndex = 4;
   btnAuthenticate.Text = "Authenticate";
       btnAuthenticate.UseVisualStyleBackColor = true;
            btnAuthenticate.Click += btnAuthenticate_Click;
            // 
            // txtKey
    // 
  txtKey.CharacterCasing = CharacterCasing.Upper;
   txtKey.Font = new Font("Consolas", 9F);
            txtKey.Location = new Point(290, 28);
       txtKey.MaxLength = 47;
  txtKey.Name = "txtKey";
   txtKey.PlaceholderText = "00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00";
            txtKey.Size = new Size(520, 22);
            txtKey.TabIndex = 3;
            txtKey.Text = "00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00";
   // 
   // lblKey
         // 
        lblKey.AutoSize = true;
    lblKey.Location = new Point(200, 31);
            lblKey.Name = "lblKey";
    lblKey.Size = new Size(84, 15);
            lblKey.TabIndex = 2;
    lblKey.Text = "Key (16 bytes):";
         // 
       // numKeyNumber
            // 
            numKeyNumber.Location = new Point(95, 28);
   numKeyNumber.Maximum = new decimal(new int[] { 13, 0, 0, 0 });
     numKeyNumber.Name = "numKeyNumber";
            numKeyNumber.Size = new Size(80, 23);
            numKeyNumber.TabIndex = 1;
            // 
   // lblKeyNumber
            // 
          lblKeyNumber.AutoSize = true;
            lblKeyNumber.Location = new Point(15, 31);
      lblKeyNumber.Name = "lblKeyNumber";
            lblKeyNumber.Size = new Size(76, 15);
     lblKeyNumber.TabIndex = 0;
   lblKeyNumber.Text = "Key Number:";
       // 
            // lblSelectedFile
            // 
        lblSelectedFile.AutoSize = true;
   lblSelectedFile.Location = new Point(505, 25);
            lblSelectedFile.Name = "lblSelectedFile";
   lblSelectedFile.Size = new Size(82, 15);
      lblSelectedFile.TabIndex = 1;
 lblSelectedFile.Text = "Selected File: -";
          // 
            // lblSelectedAID
  // 
            lblSelectedAID.AutoSize = true;
          lblSelectedAID.Location = new Point(15, 25);
            lblSelectedAID.Name = "lblSelectedAID";
            lblSelectedAID.Size = new Size(125, 15);
     lblSelectedAID.TabIndex = 0;
   lblSelectedAID.Text = "Selected Application: -";
         // 
            // statusStrip
    // 
            statusStrip.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel });
     statusStrip.Location = new Point(0, 618);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(984, 22);
        statusStrip.TabIndex = 3;
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
   ClientSize = new Size(984, 640);
      Controls.Add(statusStrip);
            Controls.Add(groupBoxOperations);
            Controls.Add(groupBoxSelection);
            Controls.Add(groupBoxReader);
            Name = "FormMain";
 Text = "Card Updater - Mifare DESFire";
        FormClosing += FormMain_FormClosing;
      Load += FormMain_Load;
  groupBoxReader.ResumeLayout(false);
            groupBoxReader.PerformLayout();
    groupBoxSelection.ResumeLayout(false);
 groupBoxSelection.PerformLayout();
            groupBoxOperations.ResumeLayout(false);
            groupBoxOperations.PerformLayout();
          groupBoxAuth.ResumeLayout(false);
   groupBoxAuth.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numKeyNumber).EndInit();
         statusStrip.ResumeLayout(false);
      statusStrip.PerformLayout();
   ResumeLayout(false);
      PerformLayout();
        }

        #endregion

     private GroupBox groupBoxReader;
        private Label lblReaderStatus;
        private Button btnConnect;
     private Button btnRefreshReaders;
        private ComboBox comboBoxReaders;
        private GroupBox groupBoxSelection;
    private ListBox listBoxFiles;
        private ListBox listBoxAIDs;
  private Label lblFiles;
      private Label lblAIDs;
        private Button btnReadAIDs;
        private GroupBox groupBoxOperations;
        private TextBox txtFileData;
        private Label lblFileData;
     private GroupBox groupBoxAuth;
        private Button btnAuthenticate;
    private TextBox txtKey;
        private Label lblKey;
        private NumericUpDown numKeyNumber;
        private Label lblKeyNumber;
        private Label lblSelectedFile;
        private Label lblSelectedAID;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel toolStripStatusLabel;
    }
}
