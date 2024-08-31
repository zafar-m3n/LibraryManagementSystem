using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace LibraryManagementSystem
{
    public partial class ViewBorrowedBooksWindow : Window
    {
        // Connection string to your SQL Server database
        private string connectionString = "Data Source=ZAFARULLAH-NAUS\\SQLEXPRESS;Initial Catalog=LibraryManagementSystem;Integrated Security=True;";
        private int memberId; // To track which member is viewing their borrowed books

        public ViewBorrowedBooksWindow(int memberId)
        {
            InitializeComponent();
            this.memberId = memberId;
            LoadBorrowedBooks();
        }

        // Method to load borrowed books data from the database
        private void LoadBorrowedBooks()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Query to select borrowed books for the member
                    string query = @"
                        SELECT b.BookID, b.Title, b.Author, br.BorrowDate, br.DueDate
                        FROM Borrowings br
                        INNER JOIN Books b ON br.BookID = b.BookID
                        WHERE br.MemberID = @MemberID AND br.ReturnDate IS NULL";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@MemberID", memberId);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    BorrowedBooksDataGrid.ItemsSource = dataTable.DefaultView; // Set DataGrid source
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading borrowed books: " + ex.Message);
            }
        }

        // Return Book Button Click Event
        private void ReturnBookButton_Click(object sender, RoutedEventArgs e)
        {
            Button returnButton = (Button)sender;
            int bookId = Convert.ToInt32(returnButton.Tag);

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
                            // Calculate late fee if applicable
                            string checkDueDateQuery = "SELECT DueDate FROM Borrowings WHERE MemberID = @MemberID AND BookID = @BookID AND ReturnDate IS NULL";
                            SqlCommand checkDueDateCommand = new SqlCommand(checkDueDateQuery, connection, transaction);
                            checkDueDateCommand.Parameters.AddWithValue("@MemberID", memberId);
                            checkDueDateCommand.Parameters.AddWithValue("@BookID", bookId);
                            DateTime dueDate = (DateTime)checkDueDateCommand.ExecuteScalar();

                            DateTime currentDate = DateTime.Now;
                            int lateDays = (currentDate - dueDate).Days;
                            decimal lateFee = 0;

                            if (lateDays > 0)
                            {
                                // Calculate late fee: 50 rupees per day
                                lateFee = lateDays * 50;
                                MessageBoxResult result = MessageBox.Show($"The book is {lateDays} days late. Pay {lateFee} rupees to return the book.", "Late Fee", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

                                if (result == MessageBoxResult.Cancel)
                                {
                                    // If user cancels, do not proceed with return
                                    return;
                                }
                            }

                            // Update the borrowing record to set the ReturnDate
                            string returnBookQuery = "UPDATE Borrowings SET ReturnDate = GETDATE() WHERE MemberID = @MemberID AND BookID = @BookID AND ReturnDate IS NULL";
                            SqlCommand returnBookCommand = new SqlCommand(returnBookQuery, connection, transaction);
                            returnBookCommand.Parameters.AddWithValue("@MemberID", memberId);
                            returnBookCommand.Parameters.AddWithValue("@BookID", bookId);
                            returnBookCommand.ExecuteNonQuery();

                            // Update the book's availability status to available
                            string updateBookAvailabilityQuery = "UPDATE Books SET IsAvailable = 1 WHERE BookID = @BookID";
                            SqlCommand updateBookAvailabilityCommand = new SqlCommand(updateBookAvailabilityQuery, connection, transaction);
                            updateBookAvailabilityCommand.Parameters.AddWithValue("@BookID", bookId);
                            updateBookAvailabilityCommand.ExecuteNonQuery();

                            // Decrement the number of borrowed books for the member
                            string updateMemberBorrowCountQuery = "UPDATE Members SET NumberOfBorrowedBooks = NumberOfBorrowedBooks - 1 WHERE MemberID = @MemberID";
                            SqlCommand updateMemberBorrowCountCommand = new SqlCommand(updateMemberBorrowCountQuery, connection, transaction);
                            updateMemberBorrowCountCommand.Parameters.AddWithValue("@MemberID", memberId);
                            updateMemberBorrowCountCommand.ExecuteNonQuery();

                            // Commit the transaction if all updates succeed
                            transaction.Commit();
                            MessageBox.Show("Book returned successfully.");
                            LoadBorrowedBooks(); // Refresh the list of borrowed books
                        }
                        catch (Exception ex)
                        {
                            // Rollback the transaction if any command fails
                            transaction.Rollback();
                            MessageBox.Show("An error occurred while returning the book: " + ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while returning the book: " + ex.Message);
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
