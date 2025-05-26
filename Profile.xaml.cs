// Profile.xaml.cs
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging; // For BitmapImage

namespace Supplier
{
    public partial class Profile : Page
    {
        public SupplierDataModel CurrentSupplier { get; set; }

        public Profile()
        {
            InitializeComponent();
            // Initialize and load data when the page is first created.
            // If CurrentSupplier can be set externally (e.g., passed via constructor for a specific supplier),
            // this initial load might be conditional.
            InitializeAndLoadDefaultSupplier(); // Create/load initial data

            // Subscribe to the Loaded event. This event fires every time the page is displayed,
            // including when navigating back to it from another page.
            this.Loaded += Profile_Loaded;
        }

        // Example constructor if you want to load a specific supplier by ID later
        // public Profile(string supplierIdToLoad)
        // {
        // InitializeComponent();
        // LoadSupplierById(supplierIdToLoad); // You'd implement this
        // this.Loaded += Profile_Loaded;
        // }

        private void InitializeAndLoadDefaultSupplier()
        {
            // In a real application, you would fetch this data from a database, API, or another service.
            // For this example, we're creating a new sample supplier or reusing if already created.
            // This strategy ensures that if we navigate away and back, CurrentSupplier isn't nullified
            // if the page instance is preserved by the navigation cache.
            if (CurrentSupplier == null) // Only create if it doesn't exist
            {
                CurrentSupplier = new SupplierDataModel
                {
                    FullName = "Global Tech Supplies Inc.",
                    Gender = "N/A",
                    DateOfBirth = new DateTime(1990, 7, 22),
                    Nationality = "Canadian",
                    Email = "contact@globaltech.com",
                    PhoneNumber = "+1-416-555-0199",
                    SupplierID = "SUP-GTS-0722",
                    LastLogin = DateTime.Now.AddHours(-5),
                    RegistrationDate = new DateTime(2015, 3, 10),
                    RoleOrType = "Wholesale Distributor",
                    AccountStatus = "Active",
                    VerificationStatus = "Verified",
                    LanguagePreference = "English (CA)",
                    TimeZone = "EST (Eastern Standard Time)",
                    ImagePath = "Images/dsd.png"
                };
            }
            DisplaySupplierData(); // Display the data
        }

        private void Profile_Loaded(object sender, RoutedEventArgs e)
        {
            // This method is called when the page is loaded, including when navigating back.
            // Since CurrentSupplier is passed by reference to EditProfilePage and modified there,
            // we just need to re-display its current state.
            if (CurrentSupplier != null)
            {
                DisplaySupplierData();
            }
            else
            {
                // This case might happen if the page is loaded without CurrentSupplier being initialized.
                // For safety, you could re-initialize or show an error.
                InitializeAndLoadDefaultSupplier();
            }
        }

        private void DisplaySupplierData()
        {
            if (CurrentSupplier == null) return;

            // Sidebar
            if (!string.IsNullOrEmpty(CurrentSupplier.ImagePath))
            {
                try
                {
                    // For images as 'Resource':
                    // SupplierImage.Source = new BitmapImage(new Uri($"pack://application:,,,/Supplier;component/{CurrentSupplier.ImagePath}", UriKind.Absolute));
                    // For images as 'Content' and 'Copy to Output':
                    SupplierImage.Source = new BitmapImage(new Uri(CurrentSupplier.ImagePath, UriKind.RelativeOrAbsolute));

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading image '{CurrentSupplier.ImagePath}': {ex.Message}");
                    // Optionally set a default image if loading fails
                    // SupplierImage.Source = new BitmapImage(new Uri("Images/default_supplier.png", UriKind.RelativeOrAbsolute));
                }
            }
            else
            {
                SupplierImage.Source = null; // Clear image if no path
            }
            SidebarSupplierName.Text = CurrentSupplier.FullName;
            SidebarSupplierEmail.Text = CurrentSupplier.Email;

            // Main Content - Personal Info
            FullNameTextBlock.Text = CurrentSupplier.FullName;
            GenderTextBlock.Text = CurrentSupplier.Gender;
            DobTextBlock.Text = CurrentSupplier.DateOfBirth?.ToString("MMMM dd, yyyy") ?? "N/A";
            NationalityTextBlock.Text = CurrentSupplier.Nationality;
            EmailTextBlock.Text = CurrentSupplier.Email;
            PhoneNumberTextBlock.Text = CurrentSupplier.PhoneNumber;

            // Main Content - Supplier Details
            SupplierIdTextBlock.Text = CurrentSupplier.SupplierID;
            LastLoginTextBlock.Text = CurrentSupplier.LastLogin?.ToString("g") ?? "N/A";
            RegistrationDateTextBlock.Text = CurrentSupplier.RegistrationDate?.ToString("MMMM dd, yyyy") ?? "N/A";
            RoleTextBlock.Text = CurrentSupplier.RoleOrType;
            AccountStatusTextBlock.Text = CurrentSupplier.AccountStatus;
            VerificationStatusTextBlock.Text = CurrentSupplier.VerificationStatus;

            // Main Content - Preferences
            LanguagePreferenceTextBlock.Text = CurrentSupplier.LanguagePreference;
            TimeZoneTextBlock.Text = CurrentSupplier.TimeZone;
        }

        private void EditProfileButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentSupplier == null)
            {
                MessageBox.Show("Supplier data is not loaded. Cannot edit.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (this.NavigationService != null)
            {
                // Pass the CurrentSupplier object to the EditProfilePage.
                // Since SupplierDataModel is a class (reference type),
                // changes made in EditProfilePage to this object will affect the CurrentSupplier here.
                EditProfilePage editPage = new EditProfilePage(CurrentSupplier);
                this.NavigationService.Navigate(editPage);
            }
            else
            {
                MessageBox.Show("Navigation service is not available.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}