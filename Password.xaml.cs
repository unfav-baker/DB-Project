using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using MySql.Data.MySqlClient;

namespace Dashboard
{
    public partial class Password : Page
    {
        private string email;


        public Password(string email)
        {
            InitializeComponent();
            this.email = email;
        }

        private void btnSendResetEmail_Click(object sender, RoutedEventArgs e)
        {
            string newPassword = txtpassword.Text.Trim();
            string confirmPassword = txtconfirm.Text.Trim();

            if (string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                MessageBox.Show("Please fill in both password fields.", "Input Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Passwords do not match!", "Mismatch", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string connectionString = "server=localhost;user id=root;password=VORTEX@20000;database=exportmanagementsystem;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE login SET password = @Password WHERE LOWER(email) = @Email";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Password", newPassword);
                        cmd.Parameters.AddWithValue("@Email", email.ToLower());

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Password updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                            Window2 newWindow = new Window2();
                            newWindow.Show();

                            // Close the parent window that hosts the page
                            Window parentWindow = Window.GetWindow(this);
                            if (parentWindow != null)
                                parentWindow.Close();
                        }
                        else
                        {
                            MessageBox.Show("No matching user found. Password update failed.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }





        // Navigate to Sign In page (assuming Window7 is a Window)
        private void SignIn_Click(object sender, RoutedEventArgs e)
        {
            Window5 signInWindow = new Window5();
            signInWindow.Show();

            // Close the current window that hosts this page (optional)
            Window parentWindow = Window.GetWindow(this);
            if (parentWindow != null)
                parentWindow.Close();
        }





    }
}
    
