using System;
using System.Collections.Generic; // Kept from your original using statements
using System.Linq; // Kept from your original using statements
using System.Text; // Kept from your original using statements
using System.Threading.Tasks; // Kept from your original using statements
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data; // Kept from your original using statements
using System.Windows.Documents; // Kept from your original using statements
using System.Windows.Input; // Kept from your original using statements
using System.Windows.Media; // Kept from your original using statements
using System.Windows.Media.Imaging; // Kept from your original using statements
using System.Windows.Navigation; // Kept from your original using statements
using System.Windows.Shapes; // Kept from your original using statements
using MySql.Data.MySqlClient;
using System.Diagnostics; // Added for Debug.WriteLine

namespace Dashboard
{
    public partial class Password_c : Page
    {
        private string _email; // Renamed for common C# private field convention
        private readonly string? _connectionString;
        private const string DbConnectionStringEnvVar = "PRIMETECH_DB_CONN_STRING"; // Consistent environment variable name

        public Password_c(string email)
        {
            InitializeComponent();
            this._email = email; // Use the renamed field

            _connectionString = Environment.GetEnvironmentVariable(DbConnectionStringEnvVar);

            // Attempt to find the button by its name from XAML
            // This requires the button in Password_c.xaml to have x:Name="btnSendResetEmail"
            Button? resetButton = this.FindName("btnSendResetEmail") as Button;

            if (string.IsNullOrEmpty(_connectionString))
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: Password_c.xaml.cs - Constructor - Connection string IS NULL or EMPTY. Env Var: '{DbConnectionStringEnvVar}'");
                MessageBox.Show($"Database connection string environment variable '{DbConnectionStringEnvVar}' was not found or is empty. " +
                                "Please ensure it is correctly set up.\n\n" +
                                "Password cannot be updated.",
                                "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);

                if (resetButton != null)
                {
                    resetButton.IsEnabled = false;
                }
                else
                {
                    Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] WARNING: Password_c.xaml.cs - Constructor - Could not find button with x:Name='btnSendResetEmail' to disable. Ensure XAML is correct.");
                }
            }
            else
            {
                string connStringSnippet = _connectionString.Length > 30 ? _connectionString.Substring(0, 30) + "..." : _connectionString;
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: Password_c.xaml.cs - Constructor - Connection string FOUND (snippet): {connStringSnippet}");

                if (resetButton != null)
                {
                    resetButton.IsEnabled = true;
                }
            }
        }

        private void btnSendResetEmail_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                MessageBox.Show($"Database connection is not configured. Cannot update password.",
                                "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Assumes txtpassword exists in Password_c.xaml with x:Name="txtpassword"
            string newPassword = txtpassword.Text.Trim();
            // Assumes txtconfirm exists in Password_c.xaml with x:Name="txtconfirm"
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

            // 🛡️ CRUCIAL SECURITY REMINDER: Passwords MUST be hashed before storing.
            // Implement a strong hashing mechanism (e.g., PBKDF2, bcrypt, Argon2).
            // Example: string hashedPassword = YourPasswordHashingFunction(newPassword);
            // Then use hashedPassword in cmd.Parameters.AddWithValue("@Password", hashedPassword);

            try
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "UPDATE login SET password = @Password WHERE LOWER(email) = @Email";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // Replace newPassword with its hashed version in a real application!
                        cmd.Parameters.AddWithValue("@Password", newPassword); // Storing plain text - NOT RECOMMENDED
                        cmd.Parameters.AddWithValue("@Email", _email.ToLower()); // Use the renamed field

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Password updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                            Window2 newWindow = new Window2(); // Assuming Window2 is the intended next window
                            newWindow.Show();

                            Window parentWindow = Window.GetWindow(this);
                            if (parentWindow != null)
                            {
                                parentWindow.Close();
                            }
                        }
                        else
                        {
                            MessageBox.Show("No matching user found or password was not changed. Password update failed.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (MySqlException myEx)
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] MySQL Error during password update (Password_c): {myEx.ToString()}");
                MessageBox.Show("Error updating password: " + myEx.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Generic error during password update (Password_c): {ex.ToString()}");
                MessageBox.Show("An unexpected error occurred: " + ex.Message, "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SignIn_Click(object sender, RoutedEventArgs e)
        {
            Window7 signInWindow = new Window7(); // Assuming Window7 is your sign-in window
            signInWindow.Show();

            Window parentWindow = Window.GetWindow(this);
            if (parentWindow != null)
            {
                parentWindow.Close();
            }
        }
    }
}