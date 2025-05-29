// Add this code to the code-behind file for Window5 (Window5.xaml.cs)
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.IO;
using System.Windows;





namespace Dashboard
{
    public partial class Window5 : Window
    {
        public Window5()
        {
            InitializeComponent();
            // Load saved values
            LoadSavedCredentials();


        }

        private bool isLoaded = false;
        private bool userCheckedRemember = false;


        private void LoadSavedCredentials()
        {
            string filePath = @"C:\Users\SilentWishMAFA\Documents\Database Project\rememberme.txt";

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
            string filePath = @"C:\Users\SilentWishMAFA\Documents\Database Project\rememberme.txt";

            if (File.Exists(filePath))
                File.Delete(filePath);

            // No message shown here
        }


        private void ReturnToPreviousPage(object sender, RoutedEventArgs e)
        {
            Window3 mainWindow = new()
            {
                //WindowState = this.WindowState // Inherit current state (Maximized, Normal, Minimized)
            };
            mainWindow.Show();
            this.Close();
        }

        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            // Simplified object initialization for newWindow  
            var newWindow = new Window4
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
            string filePath = @"C:\Users\SilentWishMAFA\Documents\Database Project\rememberme.txt";

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
                                if (dbEmail == "manakhanoffical@gmail.com")
                                {
                                    MessageBox.Show($"Welcome, {dbUserName}!", "Login Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                                    Adminn.MainWindow adminWindow = new();
                                    adminWindow.Show();
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
            var newWindow = new FP_5
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
            string filePath = @"C:\Users\SilentWishMAFA\Documents\Database Project\rememberme.txt";
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