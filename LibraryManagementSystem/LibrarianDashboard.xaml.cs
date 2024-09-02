using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace LibraryManagementSystem
{
    public partial class LibrarianDashboard : Window
    {
        // Connection string to your SQL Server database
        private string connectionString = "Data Source=ZAFARULLAH-NAUS\\SQLEXPRESS;Initial Catalog=LibraryManagementSystem;Integrated Security=True;";

        public LibrarianDashboard()
        {
            InitializeComponent();
        }

        private void ViewMembersButton_Click(object sender, RoutedEventArgs e)
        {
            ViewMembersWindow viewMembersWindow = new ViewMembersWindow();
            viewMembersWindow.Show();
            this.Close();
        }

        // Logout Button Click Event
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void ManageBooksButton_Click(object sender, RoutedEventArgs e)
        {
            ManageBooksWindow manageBooksWindow = new ManageBooksWindow();
            manageBooksWindow.Show();
            this.Close();
        }

        private void ManageMembersButton_Click(object sender, RoutedEventArgs e)
        {
            ManageMembersWindow manageMembersWindow = new ManageMembersWindow();
            manageMembersWindow.Show();
            this.Close();
        }

        private void TrackIssuedBooksButton_Click(object sender, RoutedEventArgs e)
        {
            // Open the TrackIssuedBooksWindow
            TrackIssuedBooksWindow trackIssuedBooksWindow = new TrackIssuedBooksWindow();
            trackIssuedBooksWindow.Show();
            this.Close();
        }
    }
}
