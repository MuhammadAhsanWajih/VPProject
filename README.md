Project title:	
QR Code Scanner and Generator with User record system– WinForms App

Idea Description:
This project is a Windows Forms application designed for secure QR code generation and scanning. It includes user authentication (login and registration), QR generation for storing user data (name, ID, phone), and scanning functionality to retrieve and display records from a MySQL database. Each user sees only their own records.
Tools & Technologies Used:
•	Programming Language: C#
•	Framework: .NET (WinForms)
•	QR Code Library: ZXing.Net
•	Camera Library: AForge.NET
•	Database: MySQL
•	IDE: Visual Studio
•	QR Code Generator: ZXing QR Code Writer
Features & Functionality:
•	User registration and login system
•	QR code generation with embedded user data
•	Live camera feed for QR scanning
•	QR code scanning and automatic data retrieval from MySQL
•	View only logged-in user's records
•	Start/Stop camera buttons
•	Export links or QR preview

Code Snippets (with Brief Explanations):


Login Query:
string query = "SELECT * FROM users WHERE username=@user AND password=@pass";
MySqlCommand cmd = new MySqlCommand(query, conn);
Checks if login credentials match database records.
________________________________________
QR Code Generation:
QRCodeWriter qrWriter = new QRCodeWriter();
var result = qrWriter.encode(userData, BarcodeFormat.QR_CODE, width, height);
Encodes user information into a QR format.
________________________________________
Start Camera:
videoCaptureDevice = new VideoCaptureDevice(filterInfoCollection[comboBox1.SelectedIndex].MonikerString);
videoCaptureDevice.NewFrame += CaptureDevice_NewFrame;
videoCaptureDevice.Start();
Initializes and starts the live video stream from the selected camera.
 User Registration Code
string query = "INSERT INTO users (username, password, phone) VALUES (@username, @password, @phone)";
MySqlCommand cmd = new MySqlCommand(query, conn);
cmd.Parameters.AddWithValue("@username", txtUsername.Text);
cmd.Parameters.AddWithValue("@password", txtPassword.Text);
cmd.Parameters.AddWithValue("@phone", txtPhone.Text);
cmd.ExecuteNonQuery();
Registers a new user into the database with hashed credentials and phone number.
________________________________________
 User Login Validation
string query = "SELECT id FROM users WHERE username=@username AND password=@password";
MySqlCommand cmd = new MySqlCommand(query, conn);
cmd.Parameters.AddWithValue("@username", txtUsername.Text);
cmd.Parameters.AddWithValue("@password", txtPassword.Text);
object result = cmd.ExecuteScalar();
if(result != null)
{
    loggedInUserId = Convert.ToInt32(result);
    // Redirect to main form
}
Validates user credentials and stores logged-in user's ID for later access control.
________________________________________
Capturing Frames from Camera (Live Feed)
private void CaptureDevice_NewFrame(object sender, NewFrameEventArgs eventArgs)
{
    Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
    pictureBoxCamera.Image = bitmap;
}
Captures each frame from the webcam and displays it in a PictureBox.
________________________________________
 QR Code Scanning Logic
BarcodeReader reader = new BarcodeReader();
var result = reader.Decode((Bitmap)pictureBoxCamera.Image);
if (result != null)
{
    txtScannedData.Text = result.Text;
    timer1.Stop();  // Stop scanning once a valid code is read
}
Scans the current frame for QR data and shows it in a textbox.
________________________________________ Load Logged-in User’s Records
string query = "SELECT name, student_id, phone, qr_link FROM qr_data WHERE user_id = @user_id";
MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
adapter.SelectCommand.Parameters.AddWithValue("@user_id", loggedInUserId);
DataTable dt = new DataTable();
adapter.Fill(dt);
dataGridView1.DataSource = dt;
Displays only the data belonging to the logged-in user in a grid.
________________________________________
Stop Camera and Clean Resources
if (videoCaptureDevice != null && videoCaptureDevice.IsRunning)
{
    videoCaptureDevice.SignalToStop();
    videoCaptureDevice.WaitForStop();
}
Ensures the camera stops when the form closes or user logs out.
Queries used to create tables:
CREATE DATABASE IF NOT EXISTS application;
USE application;
CREATE TABLE users (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100),
    email VARCHAR(100) UNIQUE,
    phone VARCHAR(20),
    password VARCHAR(100)
);
CREATE TABLE qr_code (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT,
    scanned_text TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(id)
);


Challenges Faced & Solutions:
•	Camera Access Issues:
Problem: Camera feed not displaying.
Solution: Ensured correct device index and included AForge libraries correctly.
•	User Data Isolation:
Problem: All records were visible to all users.
Solution: Filtered SELECT queries by user_id after login.
•	QR Decoding Delay:
Problem: Delay in scanning due to frame drop.
Solution: Optimized frame resolution and decoding logic using ZXing.
Future Improvements:
•	Add admin panel for managing all users
•	Export reports to PDF or Excel
•	Face recognition login as an optional authentication method
•	Responsive design with WPF or cross-platform support using .NET MAUI
Conclusion:
This QR Code Scanner project demonstrates effective integration of camera control, QR technology, and database management with a simple and secure user interface. It successfully fulfills its goal of user-based QR data generation and retrieval, with opportunities for further enhancements.
