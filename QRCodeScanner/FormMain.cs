using System;
using System.Drawing;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using ZXing;
using MySql.Data.MySqlClient;

namespace QRCodeScanner
{
    public partial class FormMain : Form
    {
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoCaptureDevice;
        private string connectionString = "server=localhost;user=root;password=Ahsan.1007;database=application;";

        public FormMain()
        {
            InitializeComponent();
            this.FormClosing += Form1_FormClosing;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (videoDevices.Count == 0)
            {
                MessageBox.Show("No camera devices found.");
                btnStart2.Enabled = false;
                return;
            }

            btnStart2.Enabled = true;
            btnStop2.Enabled = false;
        }

        private void btnStart2_Click(object sender, EventArgs e)
        {
            if (videoCaptureDevice == null || !videoCaptureDevice.IsRunning)
            {
                videoCaptureDevice = new VideoCaptureDevice(videoDevices[0].MonikerString);
                videoCaptureDevice.NewFrame += VideoCaptureDevice_NewFrame;
                videoCaptureDevice.Start();

                btnStart2.Enabled = false;
                btnStop2.Enabled = true;
            }
        }

        private void btnStop2_Click_1(object sender, EventArgs e)
        {
            StopCamera();
            btnStart2.Enabled = true;
            btnStop2.Enabled = false;
        }

        private void StopCamera()
        {
            try
            {
                if (videoCaptureDevice != null)
                {
                    if (videoCaptureDevice.IsRunning)
                    {
                        videoCaptureDevice.SignalToStop();
                        videoCaptureDevice.WaitForStop();
                    }

                    videoCaptureDevice.NewFrame -= VideoCaptureDevice_NewFrame;
                    videoCaptureDevice = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while closing camera: " + ex.Message);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopCamera();
        }

        private void VideoCaptureDevice_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                Bitmap frame = (Bitmap)eventArgs.Frame.Clone();

                // Show camera feed
                if (pictureBox1.IsHandleCreated)
                {
                    pictureBox1.Invoke(new MethodInvoker(() =>
                    {
                        pictureBox1.Image?.Dispose();
                        pictureBox1.Image = (Bitmap)frame.Clone();
                    }));
                }

                // Try decode QR code
                BarcodeReader reader = new BarcodeReader();
                var result = reader.Decode(frame);

                if (result != null && txtResult.IsHandleCreated)
                {
                    txtResult.Invoke(new MethodInvoker(() =>
                    {
                        if (txtResult.Text != result.Text)
                        {
                            txtResult.Text = result.Text;
                            SaveToDatabase(result.Text);
                        }
                    }));
                }

                frame.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Frame processing error: " + ex.Message);
            }
        }

        private void SaveToDatabase(string scannedText)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO qr_code (user_id, scanned_text) VALUES (@uid, @text)";
                   

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    { cmd.Parameters.AddWithValue("@uid", Session.UserId);
                        cmd.Parameters.AddWithValue("@text", scannedText);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error: " + ex.Message);
            }
        }

        private void SaveGeneratedQRToDatabase(string qrText)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO qr_code (user_id, scanned_text) VALUES (@uid, @text)";
                    

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {cmd.Parameters.AddWithValue("@uid", Session.UserId);
                    cmd.Parameters.AddWithValue("@Text", qrText);
                       
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error: " + ex.Message);
            }
        }

        private void btnGenerate_Click_1(object sender, EventArgs e)
        {
            string qrText = txtInput.Text.Trim();

            if (string.IsNullOrEmpty(qrText))
            {
                MessageBox.Show("Enter text to generate QR code.");
                return;
            }

            BarcodeWriter writer = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new ZXing.Common.EncodingOptions
                {
                    Height = 250,
                    Width = 250
                }
            };

            try
            {
                pictureBox2.Image = writer.Write(qrText);
                SaveGeneratedQRToDatabase(qrText);
            }
            catch (Exception ex)
            {
                MessageBox.Show("QR generation error: " + ex.Message);
            }
        }

        private void btnViewMyRecords_Click(object sender, EventArgs e)
        {
           FormRecords records = new FormRecords();
            records.Show();
            this.Hide();
        }
    }
}
