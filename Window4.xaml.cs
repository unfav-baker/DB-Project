using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;

namespace Dashboard
{
    /// <summary>
    /// Interaction logic for Window4.xaml
    /// </summary>
    public partial class Window4 : Window
    {

        string connectionString = "server=localhost;user id=root;password=VORTEX@20000;database=exportmanagementsystem;";

        public Window4()
        {
            InitializeComponent();
        }
        private void ReturnToPreviousPage(object sender, RoutedEventArgs e)
        {
            Window3 mainWindow = new Window3();
            //mainWindow.WindowState = this.WindowState; // Inherit current state (Maximized, Normal, Minimized)
            mainWindow.Show();
            this.Close();
        }

        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            string userName = NameTextBox.Text.Trim();
            string email = EmailTextBox.Text.Trim();
            string password = PasswordTextBox.Text.Trim();
            string confirmPassword = ConfirmPasswordTextBox.Text.Trim();

            // Validation
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("Please fill all fields.");
                return;
            }

            // Email validation
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Invalid email format.");
                return;
            }

            // Password length check
            if (password.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters.");
                return;
            }

            // Password match check
            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match.");
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Check for existing email
                    string checkQuery = "SELECT COUNT(*) FROM login WHERE email = @Email";
                    MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@Email", email);

                    int userExists = Convert.ToInt32(checkCmd.ExecuteScalar());
                    if (userExists > 0)
                    {
                        MessageBox.Show("An account with this email already exists.");
                        return;
                    }

                    // Insert new user
                    string query = "INSERT INTO login (user_name, email, password) VALUES (@UserName, @Email, @Password)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@UserName", userName);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password); // Remember to hash passwords in real apps

                    int result = cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        MessageBox.Show("Registration successful!");

                        Window2 window2 = new Window2();
                        window2.Show();

                        this.Hide(); // Just hide the signup window instead of closing it
                    }

                    else
                    {
                        MessageBox.Show("Registration failed. Try again.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }



        private void SignIn_Click(object sender, RoutedEventArgs e)
        {
            // Simplified object initialization for newWindow  
            var newWindow = new Window5
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

        private void ClearFields()
        {
            NameTextBox.Text = "";
            EmailTextBox.Text = "";
            PasswordTextBox.Text = "";
            ConfirmPasswordTextBox.Text = "";
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            NameTextBox.Focus();
        }

        

    }


}
