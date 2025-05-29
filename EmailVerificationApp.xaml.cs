using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System;
using System.Net;
using System.Net.Mail;
using MySql.Data.MySqlClient;

namespace Dashboard
{
    /// <summary>
    /// Interaction logic for EmailVerificationApp.xaml
    /// </summary>
    public partial class EmailVerificationApp : Window
    {

        string connectionString = "server=localhost;user=root;password=VORTEX@20000;database=exportmanagementsystem;";
        string sentCode = "";

        public EmailVerificationApp()
        {
            InitializeComponent();
        }


        private void SendCode_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailBox.Text;
            sentCode = new Random().Next(100000, 999999).ToString();

            // Save to DB
            using (var con = new MySqlConnection(connectionString))
            {
                con.Open();
                var cmd = new MySqlCommand("INSERT INTO email_verification (email, verification_code) VALUES (@email, @code)", con);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@code", sentCode);
                cmd.ExecuteNonQuery();
            }

            // Send email
            MailMessage message = new MailMessage("yourEmail@gmail.com", email, "Your Code", $"Your OTP code is: {sentCode}");
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("manakhanoffical@gmail.com", "gqjm dboo glgs bcuf"),
                EnableSsl = true
            };

            try
            {
                smtp.Send(message);
                ResultText.Text = "Code sent successfully!";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error sending email: " + ex.Message);
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
                    if (reader.Read())
                    {
                        ResultText.Text = "✅ Code verified!";
                        ResultText.Foreground = System.Windows.Media.Brushes.Green;
                    }
                    else
                    {
                        ResultText.Text = "❌ Invalid code!";
                        ResultText.Foreground = System.Windows.Media.Brushes.Red;
                    }
                }
            }
        }

    }
}
