namespace CardUpdater
{
    public partial class FormMain : Form
    {
        private string[] availableReaders;
        private int readerCount;
        private bool isConnected = false;
        private bool cardPresent = false;
        private byte[] currentCardUID;
        private int currentCardUIDLength;
        private byte[] selectedAID;  // Currently selected AID
        private bool isAuthenticated = false;  // Authentication status

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            // Subscribe to card present event
            UsbReader.EventCardPresent += OnCardPresent;

            // Auto-refresh readers on form load
            RefreshReaders();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Unsubscribe from event
            UsbReader.EventCardPresent -= OnCardPresent;

            // Cleanup if needed
            if (isConnected)
            {
                // Timer will be stopped automatically
                isConnected = false;
            }
        }

        private void btnRefreshReaders_Click(object sender, EventArgs e)
        {
            RefreshReaders();
        }

        private void RefreshReaders()
        {
            try
            {
                // Initialize array for readers (max 10 readers)
                availableReaders = new string[10];
                readerCount = 0;

                // Get list of readers
                bool success = UsbReader.InitReader(ref availableReaders, ref readerCount);

                // Clear combobox
                comboBoxReaders.Items.Clear();

                if (success && readerCount > 0)
                {
                    // Add readers to combobox
                    for (int i = 0; i < readerCount; i++)
                    {
                        comboBoxReaders.Items.Add(availableReaders[i]);
                    }

                    // Select first reader by default
                    comboBoxReaders.SelectedIndex = 0;

                    // Enable connect button
                    btnConnect.Enabled = true;

                    // Update status
                    toolStripStatusLabel.Text = $"{readerCount} reader(s) found";
                    lblReaderStatus.Text = "Not connected";
                }
                else
                {
                    // No readers found
                    btnConnect.Enabled = false;
                    toolStripStatusLabel.Text = "No readers found";
                    lblReaderStatus.Text = "No readers";
                    MessageBox.Show("No card readers found. Please connect a reader and try again.",
                            "No Readers", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                toolStripStatusLabel.Text = "Error reading readers";
                MessageBox.Show($"Error while reading card readers:\n{ex.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (comboBoxReaders.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a reader first.", "No Reader Selected",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (!isConnected)
                {
                    // Get selected reader name
                    string selectedReader = comboBoxReaders.SelectedItem.ToString();

                    // Select the reader
                    UsbReader.SelectReader(selectedReader);

                    // Connect to reader (starts timer)
                    UsbReader.Connect();

                    // Update UI
                    isConnected = true;
                    btnConnect.Text = "Disconnect";
                    lblReaderStatus.Text = "Connected";
                    lblReaderStatus.ForeColor = Color.Green;
                    comboBoxReaders.Enabled = false;
                    btnRefreshReaders.Enabled = false;
                    toolStripStatusLabel.Text = $"Connected to {selectedReader}";
                }
                else
                {
                    // Disconnect
                    // Note: UsbReader doesn't have explicit disconnect, but we can stop monitoring
                    isConnected = false;
                    btnConnect.Text = "Connect";
                    lblReaderStatus.Text = "Not connected";
                    lblReaderStatus.ForeColor = SystemColors.ControlText;
                    comboBoxReaders.Enabled = true;
                    btnRefreshReaders.Enabled = true;
                    toolStripStatusLabel.Text = "Disconnected";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while connecting to reader:\n{ex.Message}",
                "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnCardPresent(bool cardPresent, byte[] uid, int length)
        {
            // This event is raised from timer thread, so we need to invoke to UI thread
            if (InvokeRequired)
            {
                Invoke(new Action(() => OnCardPresent(cardPresent, uid, length)));
                return;
            }

            // Update UI based on card presence
            this.cardPresent = cardPresent;
         
            if (cardPresent)
            {
                // Store card UID
                currentCardUID = new byte[length];
                Array.Copy(uid, currentCardUID, length);
                currentCardUIDLength = length;
         
                // Convert UID to hex string
                string uidString = BitConverter.ToString(uid, 0, length).Replace("-", " ");
                toolStripStatusLabel.Text = $"Card detected - UID: {uidString}";
                lblReaderStatus.Text = "Card present";
                lblReaderStatus.ForeColor = Color.Blue;
                
                // Enable card operations
                groupBoxCard.Enabled = true;
                lblCardInfo.Text = $"Card UID: {uidString}";
            }
            else
            {
                currentCardUID = null;
                currentCardUIDLength = 0;
            
                toolStripStatusLabel.Text = "No card in field";
                lblReaderStatus.Text = "Connected";
                lblReaderStatus.ForeColor = Color.Green;
     
                // Disable card operations
                groupBoxCard.Enabled = false;
                lblCardInfo.Text = "Place card on reader and click Read";
                listBoxAIDs.Items.Clear();
            }
        }

        private void btnReadAIDs_Click(object sender, EventArgs e)
        {
            if (!cardPresent)
            {
                MessageBox.Show("No card detected. Please place a card on the reader.",
                "No Card", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                listBoxAIDs.Items.Clear();
                listBoxFiles.Items.Clear();
                txtFileData.Clear();
                selectedAID = null;
    
                lblCardInfo.Text = "Reading Application IDs...";
                toolStripStatusLabel.Text = "Reading AIDs from card...";
                Application.DoEvents();

                byte[] aids = new byte[255];  // Max buffer for AIDs
                int aidCount = 0;

                bool success = CardCommExtensions.GetApplicationIDs(ref aids, ref aidCount);

                if (success && aidCount > 0)
                {
                    // Each AID is 3 bytes
                    for (int i = 0; i < aidCount; i++)
                    {
                        int aidIndex = i * 3;
                        byte[] aid = new byte[3];
                        aid[0] = aids[aidIndex];
                        aid[1] = aids[aidIndex + 1];
                        aid[2] = aids[aidIndex + 2];

                        string aidHex = BitConverter.ToString(aid).Replace("-", " ");
      
                        // Also show as integer (little endian)
                        int aidValue = aid[0] | (aid[1] << 8) | (aid[2] << 16);
    
                        listBoxAIDs.Items.Add($"AID {i + 1}: 0x{aid[2]:X2}{aid[1]:X2}{aid[0]:X2} ({aidValue})");
                        // Store AID as tag for later use
                        listBoxAIDs.Items[i] = new AIDItem(aid, $"AID {i + 1}: 0x{aid[2]:X2}{aid[1]:X2}{aid[0]:X2} ({aidValue})");
                     }

                    lblCardInfo.Text = $"Found {aidCount} application(s) - Click an AID to view files";
                    toolStripStatusLabel.Text = $"Successfully read {aidCount} AIDs";
                }
                else if (success && aidCount == 0)
                {
                    lblCardInfo.Text = "No applications found on card (blank card)";
                    toolStripStatusLabel.Text = "Card has no applications";
                    MessageBox.Show("No applications found on this card.\n\nThis might be a blank/formatted card.",
                    "No Applications", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    lblCardInfo.Text = "Failed to read AIDs";
                    toolStripStatusLabel.Text = "Error reading AIDs";
                    MessageBox.Show("Failed to read Application IDs from card.\n\nMake sure the card is properly positioned on the reader.",
                    "Read Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                lblCardInfo.Text = "Error occurred";
                toolStripStatusLabel.Text = "Error reading card";
                MessageBox.Show($"Error while reading Application IDs:\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void listBoxAIDs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxAIDs.SelectedIndex < 0 || listBoxAIDs.SelectedItem == null)
                return;

            try
            {
                AIDItem selectedItem = listBoxAIDs.SelectedItem as AIDItem;
                if (selectedItem == null)
                    return;

                selectedAID = selectedItem.AID;
                isAuthenticated = false;  // Reset auth when switching apps

                // Select the application on the card
                bool success = CardComm.SelectApplication(selectedAID);
       
                if (!success)
                {
                    MessageBox.Show("Failed to select application on card.",
                        "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Read file IDs from selected application
                byte[] fileIds = new byte[32];
                int fileCount = 0;

                success = CardCommExtensions.GetFileIDs(ref fileIds, ref fileCount);

                listBoxFiles.Items.Clear();
                txtFileData.Clear();

                if (success && fileCount > 0)
                {
                    for (int i = 0; i < fileCount; i++)
                    {
                        listBoxFiles.Items.Add($"File {fileIds[i]:D2} (0x{fileIds[i]:X2})");
                        listBoxFiles.Items[i] = new FileItem(fileIds[i], $"File {fileIds[i]:D2} (0x{fileIds[i]:X2})");
                    }

                    lblFileInfo.Text = $"Found {fileCount} file(s) - Authenticate if needed, then click a file";
                    toolStripStatusLabel.Text = $"Application selected: {fileCount} files found";
                }
                else if (success && fileCount == 0)
                {
                    lblFileInfo.Text = "No files in this application";
                    toolStripStatusLabel.Text = "Application selected: no files found";
                }
                else
                {
                    lblFileInfo.Text = "Failed to read files";
                    toolStripStatusLabel.Text = "Error reading file list";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting application:\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAuthenticate_Click(object sender, EventArgs e)
        {
          if (selectedAID == null)
     {
       MessageBox.Show("Please select an application first.",
   "No Application Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
         return;
   }

     try
      {
           // Parse key number
   byte keyNumber = (byte)numKeyNumber.Value;

 // Parse key from hex string
         byte[] key = ParseHexKey(txtKey.Text);
     if (key == null || key.Length != 16)
         {
      MessageBox.Show("Invalid key format. Please enter 16 hex bytes (e.g., 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00)",
            "Invalid Key", MessageBoxButtons.OK, MessageBoxIcon.Warning);
   return;
       }

      toolStripStatusLabel.Text = "Authenticating...";
     Application.DoEvents();

                // Try AES authentication first
          bool success = CardComm.StartSessionAES(key, keyNumber);

if (success)
                {
           isAuthenticated = true;
         btnAuthenticate.BackColor = Color.LightGreen;
          btnAuthenticate.Text = "? Auth";
   toolStripStatusLabel.Text = $"Authenticated with key {keyNumber} (AES)";
  lblFileInfo.Text = "Authenticated - Click a file to read encrypted data";
                MessageBox.Show($"Successfully authenticated with AES key {keyNumber}",
            "Authentication Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
       }
     else
         {
           isAuthenticated = false;
           btnAuthenticate.BackColor = SystemColors.Control;
           btnAuthenticate.Text = "Auth";
      toolStripStatusLabel.Text = "Authentication failed";
                    MessageBox.Show("Authentication failed. Please check the key number and key value.\n\nMake sure:\n- The key number exists in the application\n- The key value is correct\n- The card is still present",
         "Authentication Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
           }
 }
          catch (Exception ex)
            {
    isAuthenticated = false;
                btnAuthenticate.BackColor = SystemColors.Control;
   btnAuthenticate.Text = "Auth";
                MessageBox.Show($"Error during authentication:\n{ex.Message}",
         "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
 }

        private void listBoxFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
        if (listBoxFiles.SelectedIndex < 0 || listBoxFiles.SelectedItem == null)
          return;

            try
   {
          FileItem selectedFile = listBoxFiles.SelectedItem as FileItem;
     if (selectedFile == null)
   return;

           byte fileId = selectedFile.FileId;

                lblFileInfo.Text = $"Reading file {fileId}...";
   Application.DoEvents();

       byte[] fileData = new byte[8192];
     int actualLength = 0;
        bool success = false;

   // Try authenticated read first if we're authenticated
 if (isAuthenticated)
 {
       // Use encrypted read method
      success = CardComm.ReadDataFile(fileId, 0, 0, ref fileData);
     
       if (success)
     {
             // Find actual length (remove padding)
             actualLength = fileData.Length;
for (int i = fileData.Length - 1; i >= 0; i--)
      {
     if (fileData[i] != 0)
               {
                 actualLength = i + 1;
   break;
        }
  }
       }
                }
      else
                {
 // Try plain read
              success = CardCommExtensions.ReadPlainDataFile(fileId, 0, 0, ref fileData, ref actualLength);
          }

     if (success && actualLength > 0)
          {
        DisplayFileData(fileId, fileData, actualLength, isAuthenticated);
           }
     else if (!success && !isAuthenticated)
         {
        txtFileData.Text = $"Failed to read file {fileId}.\r\n\r\n" +
  "This file might be encrypted and requires authentication.\r\n\r\n" +
   "Steps:\r\n" +
           "1. Enter the Key Number (0-13)\r\n" +
    "2. Enter the 16-byte Key in hex format\r\n" +
        "3. Click 'Auth' button\r\n" +
             "4. Try reading the file again";
    lblFileInfo.Text = $"File {fileId} requires authentication";
 toolStripStatusLabel.Text = "Authentication required to read this file";
         }
         else
           {
       txtFileData.Text = $"Failed to read file {fileId}.\r\n\r\nThe file might be empty or there was an error.";
             lblFileInfo.Text = $"Failed to read file {fileId}";
        toolStripStatusLabel.Text = "Error reading file";
           }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading file:\n{ex.Message}",
           "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
  }
        }

 private void DisplayFileData(byte fileId, byte[] fileData, int actualLength, bool wasEncrypted)
        {
 // Display as hex and try to decode as text
            string hexString = BitConverter.ToString(fileData, 0, actualLength).Replace("-", " ");
        string textString = System.Text.Encoding.UTF8.GetString(fileData, 0, actualLength);

            // Check if it's printable text
      bool isPrintable = true;
    for (int i = 0; i < actualLength; i++)
            {
         if (fileData[i] < 32 && fileData[i] != 10 && fileData[i] != 13 && fileData[i] != 9)
        {
    isPrintable = false;
             break;
     }
  }

    txtFileData.Clear();
          txtFileData.AppendText($"File {fileId} - Length: {actualLength} bytes");
    if (wasEncrypted)
            {
       txtFileData.AppendText(" [ENCRYPTED - Decrypted with AES]");
   }
            txtFileData.AppendText($"\r\n{'=',60}\r\n\r\n");
            txtFileData.AppendText($"HEX:\r\n{hexString}\r\n\r\n");

 if (isPrintable)
    {
        txtFileData.AppendText($"TEXT:\r\n{textString}\r\n");
            }
            else
            {
                txtFileData.AppendText($"TEXT: (binary data, not printable)\r\n");
            }

    lblFileInfo.Text = $"File {fileId}: {actualLength} bytes read" + (wasEncrypted ? " (encrypted)" : " (plain)");
            toolStripStatusLabel.Text = $"Successfully read file {fileId}" + (wasEncrypted ? " with decryption" : "");
        }

        private byte[] ParseHexKey(string hexString)
        {
            try
      {
// Remove spaces and validate
          string cleaned = hexString.Replace(" ", "").Replace("-", "").Trim();
           
    if (cleaned.Length != 32) // 16 bytes = 32 hex chars
        return null;

      byte[] key = new byte[16];
     for (int i = 0; i < 16; i++)
       {
                key[i] = Convert.ToByte(cleaned.Substring(i * 2, 2), 16);
            }
    return key;
 }
            catch
         {
   return null;
       }
        }

        // Helper classes to store AID and File info
        private class AIDItem
        {
            public byte[] AID { get; set; }
            public string Display { get; set; }

            public AIDItem(byte[] aid, string display)
            {
                AID = aid;
                Display = display;
            }

            public override string ToString()
            {
                return Display;
            }
        }

        private class FileItem
        {
            public byte FileId { get; set; }
            public string Display { get; set; }

            public FileItem(byte fileId, string display)
            {
                FileId = fileId;
                Display = display;
            }

            public override string ToString()
            {
                return Display;
            }
        }
    }
}
