using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace LibraryManagementSystem
{
    public partial class ManageBooksWindow : Window
    {
        // Connection string to your SQL Server database
        private string connectionString = "Data Source=ZAFARULLAH-NAUS\\SQLEXPRESS;Initial Catalog=LibraryManagementSystem;Integrated Security=True;";

        public ManageBooksWindow()
        {
            InitializeComponent();
            LoadBooksData();
        }

        // Method to load books data from the database
        private void LoadBooksData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM Books";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    BooksDataGrid.ItemsSource = dataTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading books: " + ex.Message);
            }
        }

        // Add Book Button Click Event
        private void AddBookButton_Click(object sender, RoutedEventArgs e)
        {
            AddBookWindow addBookWindow = new AddBookWindow();
            addBookWindow.ShowDialog();
            LoadBooksData();
        }

        // Edit Book Button Click Event
        private void EditBookButton_Click(object sender, RoutedEventArgs e)
        {
            Button editButton = (Button)sender;
            int bookId = Convert.ToInt32(editButton.Tag);

            EditBookWindow editBookWindow = new EditBookWindow(bookId);
            editBookWindow.ShowDialog();
            LoadBooksData();
        }

        // Delete Book Button Click Event
        private void DeleteBookButton_Click(object sender, RoutedEventArgs e)
        {
            Button deleteButton = (Button)sender;
            int bookId = Convert.ToInt32(deleteButton.Tag);

            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this book?", "Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = "DELETE FROM Books WHERE BookID = @BookID";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@BookID", bookId);

                        command.ExecuteNonQuery();
                        MessageBox.Show("Book deleted successfully.");
                        LoadBooksData();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while deleting the book: " + ex.Message);
                }
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
