using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Adminn
{
    public class ProfileData : INotifyPropertyChanged
    {
        // Fields from 'admin' table
        private int _adminId;
        public int AdminId { get => _adminId; set { _adminId = value; OnPropertyChanged(); } }

        private string _fullName = string.Empty;
        public string FullName { get => _fullName; set { _fullName = value; OnPropertyChanged(); DisplayName = value; } }

        private string _displayName = string.Empty;
        public string DisplayName { get => _displayName; set { _displayName = value; OnPropertyChanged(); } }

        private DateTime? _dateOfBirth;
        public DateTime? DateOfBirth { get => _dateOfBirth; set { _dateOfBirth = value; OnPropertyChanged(); } }

        private string _gender = string.Empty;
        public string Gender { get => _gender; set { _gender = value; OnPropertyChanged(); } }

        private string _nationality = string.Empty;
        public string Nationality { get => _nationality; set { _nationality = value; OnPropertyChanged(); } }

        private string _address = string.Empty;
        public string Address { get => _address; set { _address = value; OnPropertyChanged(); } }

        private string _phoneNumber = string.Empty;
        public string PhoneNumber { get => _phoneNumber; set { _phoneNumber = value; OnPropertyChanged(); } }

        private string _email = string.Empty;
        public string Email { get => _email; set { _email = value; OnPropertyChanged(); } }

        private DateTime _accountCreated;
        public DateTime AccountCreated { get => _accountCreated; set { _accountCreated = value; OnPropertyChanged(); } }

        private string _accountVerification = string.Empty;
        public string AccountVerification { get => _accountVerification; set { _accountVerification = value; OnPropertyChanged(); } }

        private string _role = string.Empty;
        public string Role { get => _role; set { _role = value; OnPropertyChanged(); } }

        private string _username = string.Empty;
        public string Username { get => _username; set { _username = value; OnPropertyChanged(); } }

        private string _languagePreference = "en-US";
        public string LanguagePreference { get => _languagePreference; set { _languagePreference = value; OnPropertyChanged(); } }

        private string _timeZone = string.Empty;
        public string TimeZone { get => _timeZone; set { _timeZone = value; OnPropertyChanged(); } }

        private string _statusDb = string.Empty;
        public string StatusFromDB { get => _statusDb; set { _statusDb = value; OnPropertyChanged(); } }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // --- Database Interaction Logic ---
        private static string? GetConnectionString() => Environment.GetEnvironmentVariable("PRIMETECH_DB_CONN_STRING");

        public static async Task<ProfileData?> LoadAdminProfileAsync(int adminId)
        {
            string? connectionString = GetConnectionString();
            if (string.IsNullOrEmpty(connectionString))
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] ERROR: ProfileData.LoadAdminProfileAsync - Connection string is missing for Admin_ID: {adminId}.");
                // The caller (Page code-behind) will handle showing a MessageBox.
                return null;
            }

            ProfileData? profile = null;
            try
            {
                using MySqlConnection connection = new(connectionString);
                await connection.OpenAsync();

                string query = "SELECT Admin_ID, Name, Phone_Number, Role, Username, Gender, Date_Of_Birth, Status, Email, Address, Nationality, Language_Preference, Time_Zone, Created_At FROM admin WHERE Admin_ID = @AdminId";
                using MySqlCommand command = new(query, connection);
                command.Parameters.AddWithValue("@AdminId", adminId);

                using MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    profile = new ProfileData
                    {
                        AdminId = reader.GetInt32(reader.GetOrdinal("Admin_ID")),
                        FullName = reader.IsDBNull(reader.GetOrdinal("Name")) ? string.Empty : reader.GetString(reader.GetOrdinal("Name")),
                        DisplayName = reader.IsDBNull(reader.GetOrdinal("Name")) ? string.Empty : reader.GetString(reader.GetOrdinal("Name")),
                        DateOfBirth = reader.IsDBNull(reader.GetOrdinal("Date_Of_Birth")) ? null : reader.GetDateTime(reader.GetOrdinal("Date_Of_Birth")),
                        Gender = reader.IsDBNull(reader.GetOrdinal("Gender")) ? string.Empty : reader.GetString(reader.GetOrdinal("Gender")),
                        Nationality = reader.IsDBNull(reader.GetOrdinal("Nationality")) ? string.Empty : reader.GetString(reader.GetOrdinal("Nationality")),
                        Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? string.Empty : reader.GetString(reader.GetOrdinal("Address")),
                        PhoneNumber = reader.IsDBNull(reader.GetOrdinal("Phone_Number")) ? string.Empty : reader.GetString(reader.GetOrdinal("Phone_Number")),
                        Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? string.Empty : reader.GetString(reader.GetOrdinal("Email")),
                        AccountCreated = reader.GetDateTime(reader.GetOrdinal("Created_At")),
                        AccountVerification = reader.IsDBNull(reader.GetOrdinal("Status")) ? "N/A" : reader.GetString(reader.GetOrdinal("Status")),
                        StatusFromDB = reader.IsDBNull(reader.GetOrdinal("Status")) ? "N/A" : reader.GetString(reader.GetOrdinal("Status")),
                        Role = reader.IsDBNull(reader.GetOrdinal("Role")) ? string.Empty : reader.GetString(reader.GetOrdinal("Role")),
                        Username = reader.IsDBNull(reader.GetOrdinal("Username")) ? string.Empty : reader.GetString(reader.GetOrdinal("Username")),
                        LanguagePreference = reader.IsDBNull(reader.GetOrdinal("Language_Preference")) ? "en-US" : reader.GetString(reader.GetOrdinal("Language_Preference")),
                        TimeZone = reader.IsDBNull(reader.GetOrdinal("Time_Zone")) ? string.Empty : reader.GetString(reader.GetOrdinal("Time_Zone"))
                    };
                }
            }
            catch (MySqlException myEx) // Catch specific DB exceptions
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] MYSQL ERROR: ProfileData.LoadAdminProfileAsync for Admin_ID {adminId} - {myEx.ToString()}");
                throw; // Re-throw for the caller to handle and show a specific DB error message
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] GENERIC ERROR: ProfileData.LoadAdminProfileAsync for Admin_ID {adminId} - {ex.ToString()}");
                throw; // Re-throw for the caller to handle
            }
            return profile;
        }

        public async Task<bool> SaveProfileAsync()
        {
            string? connectionString = GetConnectionString();
            if (string.IsNullOrEmpty(connectionString))
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] ERROR: ProfileData.SaveProfileAsync - Connection string is missing for Admin_ID: {this.AdminId}.");
                // The caller (Page code-behind) will handle showing a MessageBox.
                return false;
            }

            try
            {
                using MySqlConnection connection = new(connectionString);
                await connection.OpenAsync();

                string query = @"UPDATE admin SET
                                    Name = @Name,
                                    Date_Of_Birth = @DateOfBirth,
                                    Gender = @Gender,
                                    Nationality = @Nationality,
                                    Address = @Address,
                                    Phone_Number = @PhoneNumber,
                                    Email = @Email,
                                    Language_Preference = @LanguagePreference,
                                    Time_Zone = @TimeZone,
                                    Updated_At = @UpdatedAt
                                 WHERE Admin_ID = @AdminId;";

                using MySqlCommand command = new(query, connection);
                command.Parameters.AddWithValue("@AdminId", this.AdminId);
                command.Parameters.AddWithValue("@Name", this.FullName);
                command.Parameters.AddWithValue("@DateOfBirth", this.DateOfBirth as object ?? DBNull.Value);
                command.Parameters.AddWithValue("@Gender", string.IsNullOrWhiteSpace(this.Gender) ? DBNull.Value : (object)this.Gender);
                command.Parameters.AddWithValue("@Nationality", string.IsNullOrWhiteSpace(this.Nationality) ? DBNull.Value : (object)this.Nationality);
                command.Parameters.AddWithValue("@Address", string.IsNullOrWhiteSpace(this.Address) ? DBNull.Value : (object)this.Address);
                command.Parameters.AddWithValue("@PhoneNumber", string.IsNullOrWhiteSpace(this.PhoneNumber) ? DBNull.Value : (object)this.PhoneNumber);
                command.Parameters.AddWithValue("@Email", string.IsNullOrWhiteSpace(this.Email) ? DBNull.Value : (object)this.Email);
                command.Parameters.AddWithValue("@LanguagePreference", string.IsNullOrWhiteSpace(this.LanguagePreference) ? DBNull.Value : (object)this.LanguagePreference);
                command.Parameters.AddWithValue("@TimeZone", string.IsNullOrWhiteSpace(this.TimeZone) ? DBNull.Value : (object)this.TimeZone);
                command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                int rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch (MySqlException myEx)
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] MYSQL ERROR: ProfileData.SaveProfileAsync for Admin_ID {this.AdminId} - {myEx.ToString()}");
                throw; // Re-throw for the caller to handle
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] GENERIC ERROR: ProfileData.SaveProfileAsync for Admin_ID {this.AdminId} - {ex.ToString()}");
                throw; // Re-throw for the caller to handle
            }
        }
    }
}
