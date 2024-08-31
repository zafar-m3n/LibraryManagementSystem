using System;
using System.Data.SqlClient;
using System.Windows;

namespace LibraryManagementSystem
{
    public partial class AddMemberWindow : Window
    {
        private string connectionString = "Data Source=ZAFARULLAH-NAUS\\SQLEXPRESS;Initial Catalog=LibraryManagementSystem;Integrated Security=True;";

        public AddMemberWindow()
        {
            InitializeComponent();
        }

        private void AddMemberButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO Users (Name, Email, Password, Role) VALUES (@Name, @Email, 'password', 'Member')";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Name", NameTextBox.Text);
                    command.Parameters.AddWithValue("@Email", EmailTextBox.Text);

                    command.ExecuteNonQuery();
                    MessageBox.Show("Member added successfully.");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while adding the member: " + ex.Message);
            }
        }
    }
}
