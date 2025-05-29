// Add this code to the code-behind file for Window5 (Window5.xaml.cs)
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace Dashboard
{
    public partial class Window6 : Window
    {
        public Window6()
        {
            InitializeComponent();
            // Load saved values
            LoadSavedCredentials();
        }


        private bool isLoaded = false;
        private bool userCheckedRemember = false;


        private void LoadSavedCredentials()
        {
            string filePath = @"C:\Users\SilentWishMAFA\Documents\Database Project\rememberme1.txt";

            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                if (lines.Length >= 2)
                {
                    EmailInput.Text = lines[0];
                    PasswordInput.Text = lines[1];
                    RememberMeCheckbox.IsChecked = true;
                }
            }

            // Set flag after loading is done
            isLoaded = true;
        }


        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            // Only act if the user is manually interacting
            if (isLoaded)
            {
                userCheckedRemember = true;
                // No message shown here
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            string filePath = @"C:\Users\SilentWishMAFA\Documents\Database Project\rememberme1.txt";

            if (File.Exists(filePath))
                File.Delete(filePath);

            // No message shown here
        }


        private void ReturnToPreviousPage(object sender, RoutedEventArgs e)
        {
            Window3_b mainWindow = new()
            {
                //WindowState = this.WindowState // Inherit current state (Maximized, Normal, Minimized)
            };
            mainWindow.Show();
            this.Close();
        }

        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            // Simplified object initialization for newWindow  
            var newWindow = new Window4_b
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

        private void SignIn_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailInput.Text.Trim().ToLower();
            string password = PasswordInput.Text.Trim();
            string filePath = @"C:\Users\SilentWishMAFA\Documents\Database Project\rememberme1.txt";

            // 1. Validation
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter both Email and Password.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                EmailInput.Clear();
                PasswordInput.Clear();
                return;
            }

            // 2. Connection String
            string connectionString = "server=localhost;user id=root;password=VORTEX@20000;database=exportmanagementsystem;";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // 3. Query with parameters
                    string query = "SELECT * FROM login WHERE LOWER(email) = @Email AND password = @Password";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Password", password);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string dbEmail = reader["email"].ToString().Trim().ToLower();
                                string dbUserName = reader["user_name"].ToString();

                                // 4. Remember Me Logic (no message box here)
                                if (RememberMeCheckbox.IsChecked == true)
                                {
                                    File.WriteAllText(filePath, $"{email}\n{password}");
                                }
                                else if (File.Exists(filePath))
                                {
                                    File.Delete(filePath);
                                }

                                // 5. Role-based Access
                                if (dbEmail == "abubakermukhtar55@gmail.com")
                                {
                                    MessageBox.Show($"Welcome, {dbUserName}!", "Login Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                                    Employee.MainWindow   employeeWindow = new();
                                    employeeWindow.Show();
                                    this.Close();
                                }
                                else
                                {
                                    MessageBox.Show("Access denied. You are not allowed to log in here.", "Access Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
                                    EmailInput.Clear();
                                    PasswordInput.Clear();
                                }
                            }
                            else
                            {
                                MessageBox.Show("Incorrect Email or Password.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                                EmailInput.Clear();
                                PasswordInput.Clear();

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error connecting to database:\n{ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            // Simplified object initialization for newWindow  
            var newWindow = new FP_6
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string filePath = @"C:\Users\SilentWishMAFA\Documents\Database Project\rememberme1.txt";
            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                if (lines.Length >= 2)
                {
                    EmailInput.Text = lines[0].Trim();
                    PasswordInput.Text = lines[1].Trim();
                    RememberMeCheckbox.IsChecked = true;
                }
            }
        }


    }
}