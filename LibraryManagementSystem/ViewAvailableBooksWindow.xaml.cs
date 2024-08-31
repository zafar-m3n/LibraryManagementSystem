using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace LibraryManagementSystem
{
    public partial class ViewAvailableBooksWindow : Window
    {
        // Connection string to your SQL Server database
        private string connectionString = "Data Source=ZAFARULLAH-NAUS\\SQLEXPRESS;Initial Catalog=LibraryManagementSystem;Integrated Security=True;";
        private int memberId; // To track which member is borrowing the book

        public ViewAvailableBooksWindow(int memberId)
        {
            InitializeComponent();
            this.memberId = memberId;
            LoadAvailableBooks();
        }

        // Method to load available books data from the database
        private void LoadAvailableBooks()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Query to select books where IsAvailable is 1
                    string query = "SELECT BookID, Title, Author, Publisher, YearPublished FROM Books WHERE IsAvailable = 1";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    BooksDataGrid.ItemsSource = dataTable.DefaultView; // Set DataGrid source
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading available books: " + ex.Message);
            }
        }

        // Borrow Book Button Click Event
        private void BorrowBookButton_Click(object sender, RoutedEventArgs e)
        {
            Button borrowButton = (Button)sender;
            int bookId = Convert.ToInt32(borrowButton.Tag);

            // Check if the member has already borrowed 3 books
            if (HasReachedBorrowLimit())
            {
                MessageBox.Show("You have reached the maximum borrow limit of 3 books.", "Borrow Limit Reached", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Start a transaction to ensure all operations are atomic
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Insert the borrowing record into the Borrowings table
                            string borrowBookQuery = "INSERT INTO Borrowings (MemberID, BookID) VALUES (@MemberID, @BookID)";
                            SqlCommand borrowBookCommand = new SqlCommand(borrowBookQuery, connection, transaction);
                            borrowBookCommand.Parameters.AddWithValue("@MemberID", memberId);
                            borrowBookCommand.Parameters.AddWithValue("@BookID", bookId);
                            borrowBookCommand.ExecuteNonQuery();

                            // Update the book's availability status to not available
                            string updateBookAvailabilityQuery = "UPDATE Books SET IsAvailable = 0 WHERE BookID = @BookID";
                            SqlCommand updateBookAvailabilityCommand = new SqlCommand(updateBookAvailabilityQuery, connection, transaction);
                            updateBookAvailabilityCommand.Parameters.AddWithValue("@BookID", bookId);
                            updateBookAvailabilityCommand.ExecuteNonQuery();

                            // Increment the number of borrowed books for the member
                            string updateMemberBorrowCountQuery = "UPDATE Members SET NumberOfBorrowedBooks = NumberOfBorrowedBooks + 1 WHERE MemberID = @MemberID";
                            SqlCommand updateMemberBorrowCountCommand = new SqlCommand(updateMemberBorrowCountQuery, connection, transaction);
                            updateMemberBorrowCountCommand.Parameters.AddWithValue("@MemberID", memberId);
                            updateMemberBorrowCountCommand.ExecuteNonQuery();

                            // Commit the transaction if all updates succeed
                            transaction.Commit();
                            MessageBox.Show("Book borrowed successfully.");
                            LoadAvailableBooks(); // Refresh the list of available books
                        }
                        catch (Exception ex)
                        {
                            // Rollback the transaction if any command fails
                            transaction.Rollback();
                            MessageBox.Show("An error occurred while borrowing the book: " + ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while borrowing the book: " + ex.Message);
            }
        }

        // Method to check if the member has reached the borrow limit
        private bool HasReachedBorrowLimit()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT COUNT(*) FROM Borrowings WHERE MemberID = @MemberID AND ReturnDate IS NULL";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@MemberID", memberId);
                    int borrowedBooksCount = (int)command.ExecuteScalar();

                    return borrowedBooksCount >= 3;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while checking the borrow limit: " + ex.Message);
                return true; // Assume limit reached if error occurs
            }
        }

        // Back Button Click Event
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MemberDashboard memberDashboard = new MemberDashboard(memberId);
            memberDashboard.Show();
            this.Close();
        }
    }
}
