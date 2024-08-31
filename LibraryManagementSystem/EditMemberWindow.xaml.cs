using System;
using System.Data.SqlClient;
using System.Windows;

namespace LibraryManagementSystem
{
    public partial class EditMemberWindow : Window
    {
        private string connectionString = "Data Source=ZAFARULLAH-NAUS\\SQLEXPRESS;Initial Catalog=LibraryManagementSystem;Integrated Security=True;";
        private int userId;

        public EditMemberWindow(int userId)
        {
            InitializeComponent();
            this.userId = userId;
            LoadMemberDetails();
        }

        private void LoadMemberDetails()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT Name, Email FROM Users WHERE UserID = @UserID";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@UserID", userId);
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        NameTextBox.Text = reader["Name"].ToString();
                        EmailTextBox.Text = reader["Email"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading member details: " + ex.Message);
            }
        }

        private void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "UPDATE Users SET Name = @Name, Email = @Email WHERE UserID = @UserID";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@UserID", userId);
                    command.Parameters.AddWithValue("@Name", NameTextBox.Text);
                    command.Parameters.AddWithValue("@Email", EmailTextBox.Text);

                    command.ExecuteNonQuery();
                    MessageBox.Show("Member updated successfully.");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while updating the member: " + ex.Message);
            }
        }
    }
}
