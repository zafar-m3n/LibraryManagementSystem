using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace LibraryManagementSystem
{
    public partial class TrackIssuedBooksWindow : Window
    {
        // Connection string to your SQL Server database
        private string connectionString = "Data Source=ZAFARULLAH-NAUS\\SQLEXPRESS;Initial Catalog=LibraryManagementSystem;Integrated Security=True;";

        public TrackIssuedBooksWindow()
        {
            InitializeComponent();
            LoadIssuedBooks();
        }

        // Method to load issued books data from the database
        private void LoadIssuedBooks()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Query to select issued books and the member who borrowed them
                    string query = @"
                        SELECT b.BookID, b.Title, b.Author, u.Name AS BorrowerName, br.BorrowDate, br.DueDate
                        FROM Borrowings br
                        INNER JOIN Books b ON br.BookID = b.BookID
                        INNER JOIN Members m ON br.MemberID = m.MemberID
                        INNER JOIN Users u ON m.UserID = u.UserID
                        WHERE br.ReturnDate IS NULL";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    IssuedBooksDataGrid.ItemsSource = dataTable.DefaultView; // Set DataGrid source
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading issued books: " + ex.Message);
            }
        }

        // Back Button Click Event
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            LibrarianDashboard librarianDashboard = new LibrarianDashboard();
            librarianDashboard.Show();
            this.Close();
        }
    }
}
