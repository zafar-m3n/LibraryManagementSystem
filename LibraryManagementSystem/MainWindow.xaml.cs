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
                    string query = "SELECT Role FROM Users WHERE Email = @Email AND Password = @Password";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", password);

                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        reader.Read();
                        string role = reader["Role"].ToString();

                        if (role == "Librarian")
                        {
                            // Redirect to Librarian Dashboard
                            LibrarianDashboard librarianDashboard = new LibrarianDashboard();
                            librarianDashboard.Show();
                            this.Close();
                        }
                        else if (role == "Member")
                        {
                            // Redirect to Member Dashboard
                            MemberDashboard memberDashboard = new MemberDashboard();
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
