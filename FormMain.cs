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
                    }

                    lblCardInfo.Text = $"Found {aidCount} application(s)";
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
    }
}
