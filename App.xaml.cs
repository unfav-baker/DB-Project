using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using QuestPDF.Infrastructure; // <-- ADD THIS LINE

namespace Adminn // Your application's namespace
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            // Set the QuestPDF license type here
            QuestPDF.Settings.License = LicenseType.Community;
            // If you have a commercial license, use:
            // QuestPDF.Settings.License = LicenseType.Pro; 
            // QuestPDF.Settings.License = LicenseType.Enterprise; 

            // InitializeComponent(); // This might already be here if you have an App_Startup event in App.xaml
            // or it might be implicitly called. If your app worked before,
            // don't worry about adding InitializeComponent() here unless needed.
        }

        // If you have an OnStartup method, you could also put it there:
        // protected override void OnStartup(StartupEventArgs e)
        // {
        //     base.OnStartup(e);
        //     QuestPDF.Settings.License = LicenseType.Community;
        // }
    }
}