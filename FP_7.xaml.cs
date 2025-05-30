using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;

namespace Dashboard
{
   
    public partial class FP_7 : Window
    {

        string connectionString = "server=127.0.0.1;Port=3306;Database=prime_tech;Uid=root;Pwd=Abubaker85@@;";
        string sentCode = "";

        public FP_7()
        {
            InitializeComponent();


        }

        private void ReturnToPreviousPage(object sender, RoutedEventArgs e)
        {
            Window7 mainWindow = new();
            //mainWindow.WindowState = this.WindowState; // Inherit current state (Maximized, Normal, Minimized)
            mainWindow.Show();
            this.Close();
        }


        private void SignIn_Click(object sender, RoutedEventArgs e)
        {
            // Simplified object initialization for newWindow  
            var newWindow = new Window7
            {
                WindowState = this.WindowState
            };

            if (this.WindowState == WindowState.Normal)
            {
                newWindow.Left = this.Left;
                newWindow.Top = this.Top;
                newWindow.Width = this.Width;
                newWindow.Height = this.Height;
            }

            newWindow.Show();
            this.Close();
        }


        private void SendCode_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailBox.Text.Trim(); // Clean extra spaces
            sentCode = new Random().Next(100000, 999999).ToString();

            // ✅ Check if the entered email is your official email
            if (email.ToLower() != "abubakerbarkat45@gmail.com")
            {
                MessageBox.Show("This email is not allowed. Please enter a valid authorized email.", "Unauthorized Email", MessageBoxButton.OK, MessageBoxImage.Warning);
                ResultText.Text = "❌ Only the official email is allowed!";
                ResultText.Foreground = System.Windows.Media.Brushes.Red;
                return;
            }

            // Save to DB
            using (var con = new MySqlConnection(connectionString))
            {
                con.Open();
                var cmd = new MySqlCommand("INSERT INTO email_verification (email, verification_code) VALUES (@email, @code)", con);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@code", sentCode);
                cmd.ExecuteNonQuery();
            }

            // Prepare email
            MailMessage message = new MailMessage(
                "abubakerbarkat45@gmail.com", // FROM
                email,                       // TO
                "Your OTP Code",             // Subject
                $"Your OTP code is: {sentCode}" // Body
            );

            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("abubakerbarkat45@gmail.com", "qynv eivl frsx qkdj"),
                EnableSsl = true
            };

            try
            {
                smtp.Send(message);
                MessageBox.Show("Code sent successfully! Please check your email.", "Code Sent", MessageBoxButton.OK, MessageBoxImage.Information);
                ResultText.Text = "Code sent successfully! Check your email.";
                ResultText.Foreground = System.Windows.Media.Brushes.Green;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error sending email: " + ex.Message, "Email Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void VerifyCode_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailBox.Text;
            string enteredCode = CodeBox.Text;

            using (var con = new MySqlConnection(connectionString))
            {
                con.Open();
                var cmd = new MySqlCommand("SELECT * FROM email_verification WHERE email=@e AND verification_code=@c", con);
                cmd.Parameters.AddWithValue("@e", email);
                cmd.Parameters.AddWithValue("@c", enteredCode);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        // Show popup that code verified
                        MessageBox.Show("Code verified successfully!", "Verified", MessageBoxButton.OK, MessageBoxImage.Information);

                        ResultText.Text = "✅ Code verified!";
                        ResultText.Foreground = System.Windows.Media.Brushes.Green;

                        // Navigate to Password page
                        Dashboard.Password passwordPage = new Dashboard.Password(email);
                        NavigationService nav = NavigationService.GetNavigationService(this);

                        if (nav != null)
                        {
                            nav.Navigate(passwordPage);
                        }
                        else
                        {
                            this.Content = passwordPage;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid verification code. Please try again.", "Invalid Code", MessageBoxButton.OK, MessageBoxImage.Warning);

                        ResultText.Text = "❌ Invalid code!";
                        ResultText.Foreground = System.Windows.Media.Brushes.Red;
                    }
                }
            }
        }

        private void CodeBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }


}



