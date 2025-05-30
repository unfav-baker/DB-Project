using System;
using System.Windows;
using System.Windows.Controls;
// using System.Windows.Navigation; // Included in your original, keep if used elsewhere on the page
using MySql.Data.MySqlClient;
using System.Diagnostics;

namespace Dashboard
{
    public partial class Password : Page
    {
        private string _email;
        private readonly string? _connectionString;
        private const string DbConnectionStringEnvVar = "PRIMETECH_DB_CONN_STRING";

        public Password(string email)
        {
            InitializeComponent(); // This method generates fields from XAML based on x:Name
            this._email = email;

            _connectionString = Environment.GetEnvironmentVariable(DbConnectionStringEnvVar);

            // Attempt to find the button by its name from XAML
            // This requires the button in Password.xaml to have x:Name="btnSendResetEmail"
            Button? resetButton = this.FindName("btnSendResetEmail") as Button;

            if (string.IsNullOrEmpty(_connectionString))
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: Password.xaml.cs - Constructor - Connection string IS NULL or EMPTY. Env Var: '{DbConnectionStringEnvVar}'");
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
                    Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] WARNING: Password.xaml.cs - Constructor - Could not find button with x:Name='btnSendResetEmail' to disable. Ensure XAML is correct.");
                }
            }
            else
            {
                string connStringSnippet = _connectionString.Length > 30 ? _connectionString.Substring(0, 30) + "..." : _connectionString;
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: Password.xaml.cs - Constructor - Connection string FOUND (snippet): {connStringSnippet}");

                if (resetButton != null)
                {
                    resetButton.IsEnabled = true;
                }
                // No else needed here, if button isn't found, it just won't be explicitly enabled.
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

            string newPassword = txtpassword.Text.Trim(); // Assumes txtpassword exists in XAML with x:Name="txtpassword"
            string confirmPassword = txtconfirm.Text.Trim(); // Assumes txtconfirm exists in XAML with x:Name="txtconfirm"

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

            // CRUCIAL SECURITY REMINDER: Passwords MUST be hashed before storing.
            // The following code sends the newPassword as plain text to the database.
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
                        cmd.Parameters.AddWithValue("@Email", _email.ToLower());

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Password updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                            // Assuming Window2 is your login window or a relevant next window
                            Window2 newWindow = new Window2();
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
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] MySQL Error during password update: {myEx.ToString()}");
                MessageBox.Show("Error updating password: " + myEx.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Generic error during password update: {ex.ToString()}");
                MessageBox.Show("An unexpected error occurred: " + ex.Message, "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Navigate to Sign In page
        private void SignIn_Click(object sender, RoutedEventArgs e)
        {
            // Assuming Window5 is your sign-in window
            Window5 signInWindow = new Window5();
            signInWindow.Show();

            Window parentWindow = Window.GetWindow(this);
            if (parentWindow != null)
            {
                parentWindow.Close();
            }
        }
    }
}