using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace LibraryManagementSystem
{
    public partial class MainWindow : Window
    {
        // Connection string to your SQL Server database
        private string connectionString = "Data Source=ZAFARULLAH-NAUS\\SQLEXPRESS;Initial Catalog=LibraryManagementSystem;Integrated Security=True;";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ErrorMessageTextBlock.Text = "Please enter both email and password.";
                return;
            }

            // Attempt to log in
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT Role, UserID FROM Users WHERE Email = @Email AND Password = @Password";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", password);

                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        reader.Read();
                        string role = reader["Role"].ToString();
                        int userId = Convert.ToInt32(reader["UserID"]); // Retrieve UserID

                        if (role == "Librarian")
                        {
                            // Redirect to Librarian Dashboard
                            LibrarianDashboard librarianDashboard = new LibrarianDashboard();
                            librarianDashboard.Show();
                            this.Close();
                        }
                        else if (role == "Member")
                        {
                            reader.Close(); // Close the reader before executing another command

                            // Retrieve the MemberID for the logged-in user
                            string memberQuery = "SELECT MemberID FROM Members WHERE UserID = @UserID";
                            SqlCommand memberCommand = new SqlCommand(memberQuery, connection);
                            memberCommand.Parameters.AddWithValue("@UserID", userId);

                            int memberId = Convert.ToInt32(memberCommand.ExecuteScalar());

                            // Redirect to Member Dashboard with memberId
                            MemberDashboard memberDashboard = new MemberDashboard(memberId);
                            memberDashboard.Show();
                            this.Close();
                        }
                    }
                    else
                    {
                        ErrorMessageTextBlock.Text = "Invalid email or password.";
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessageTextBlock.Text = "An error occurred: " + ex.Message;
            }
        }
    }
}
