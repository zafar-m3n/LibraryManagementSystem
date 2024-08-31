using System;
using System.Windows;

namespace LibraryManagementSystem
{
    /// <summary>
    /// Interaction logic for MemberDashboard.xaml
    /// </summary>
    public partial class MemberDashboard : Window
    {
        private int memberId; // Member ID to keep track of the logged-in member

        // Constructor that accepts a memberId
        public MemberDashboard(int memberId)
        {
            InitializeComponent();
            this.memberId = memberId; // Store the member ID for use in this class
        }

        // Logout Button Click Event
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        // View Available Books Button Click Event
        private void ViewAvailableBooksButton_Click(object sender, RoutedEventArgs e)
        {
            // Pass the memberId to the ViewAvailableBooksWindow
            ViewAvailableBooksWindow viewAvailableBooksWindow = new ViewAvailableBooksWindow(memberId);
            viewAvailableBooksWindow.Show();
            this.Close();
        }

        // View Borrowed Books Button Click Event
        private void ViewBorrowedBooksButton_Click(object sender, RoutedEventArgs e)
        {
            // You can implement the logic to view borrowed books here
        }
    }
}
