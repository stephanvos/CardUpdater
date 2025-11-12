namespace CardUpdater
{
    public partial class FormMain : Form
    {
        private string[] availableReaders;
        private int readerCount;
        private bool isConnected = false;

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
            if (cardPresent)
            {
                // Convert UID to hex string
                string uidString = BitConverter.ToString(uid, 0, length).Replace("-", " ");
                toolStripStatusLabel.Text = $"Card detected - UID: {uidString}";
                lblReaderStatus.Text = "Card present";
                lblReaderStatus.ForeColor = Color.Blue;
            }
            else
            {
                toolStripStatusLabel.Text = "No card in field";
                lblReaderStatus.Text = "Connected";
                lblReaderStatus.ForeColor = Color.Green;
            }
        }
    }
}
