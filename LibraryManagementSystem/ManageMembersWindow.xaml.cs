using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace LibraryManagementSystem
{
    public partial class ManageMembersWindow : Window
    {
        // Connection string to your SQL Server database
        private string connectionString = "Data Source=ZAFARULLAH-NAUS\\SQLEXPRESS;Initial Catalog=LibraryManagementSystem;Integrated Security=True;";

        public ManageMembersWindow()
        {
            InitializeComponent();
            LoadMembersData();
        }

        // Method to load members data from the database
        private void LoadMembersData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT Users.UserID, Users.Name, Users.Email, Members.NumberOfBorrowedBooks FROM Users INNER JOIN Members ON Users.UserID = Members.UserID";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    MembersDataGrid.ItemsSource = dataTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading members: " + ex.Message);
            }
        }

        // Add Member Button Click Event
        private void AddMemberButton_Click(object sender, RoutedEventArgs e)
        {
            AddMemberWindow addMemberWindow = new AddMemberWindow();
            addMemberWindow.ShowDialog();
            LoadMembersData();
        }

        // Edit Member Button Click Event
        private void EditMemberButton_Click(object sender, RoutedEventArgs e)
        {
            Button editButton = (Button)sender;
            int userId = Convert.ToInt32(editButton.Tag);

            EditMemberWindow editMemberWindow = new EditMemberWindow(userId);
            editMemberWindow.ShowDialog();
            LoadMembersData();
        }

        // Delete Member Button Click Event
        private void DeleteMemberButton_Click(object sender, RoutedEventArgs e)
        {
            Button deleteButton = (Button)sender;
            int userId = Convert.ToInt32(deleteButton.Tag);

            // Check if the member has borrowed books
            if (HasBorrowedBooks(userId))
            {
                MessageBox.Show("This member has borrowed books that need to be returned before deletion.", "Cannot Delete Member", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this member?", "Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        // Start a transaction to ensure both deletions succeed or fail together
                        using (SqlTransaction transaction = connection.BeginTransaction())
                        {
                            try
                            {
                                // First, delete the member entry from the Members table
                                string deleteMemberQuery = "DELETE FROM Members WHERE UserID = @UserID";
                                SqlCommand deleteMemberCommand = new SqlCommand(deleteMemberQuery, connection, transaction);
                                deleteMemberCommand.Parameters.AddWithValue("@UserID", userId);
                                deleteMemberCommand.ExecuteNonQuery();

                                // Then, delete the user entry from the Users table
                                string deleteUserQuery = "DELETE FROM Users WHERE UserID = @UserID";
                                SqlCommand deleteUserCommand = new SqlCommand(deleteUserQuery, connection, transaction);
                                deleteUserCommand.Parameters.AddWithValue("@UserID", userId);
                                deleteUserCommand.ExecuteNonQuery();

                                // Commit the transaction if both deletions succeed
                                transaction.Commit();
                                MessageBox.Show("Member deleted successfully.");
                                LoadMembersData();
                            }
                            catch (Exception ex)
                            {
                                // Rollback the transaction if any command fails
                                transaction.Rollback();
                                MessageBox.Show("An error occurred while deleting the member: " + ex.Message);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while deleting the member: " + ex.Message);
                }
            }
        }

        // Method to check if a member has borrowed books
        private bool HasBorrowedBooks(int userId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT COUNT(*) FROM Borrowings WHERE MemberID = (SELECT MemberID FROM Members WHERE UserID = @UserID) AND ReturnDate IS NULL";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@UserID", userId);
                    int borrowedBooksCount = (int)command.ExecuteScalar();

                    return borrowedBooksCount > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while checking borrowed books: " + ex.Message);
                return false;
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
