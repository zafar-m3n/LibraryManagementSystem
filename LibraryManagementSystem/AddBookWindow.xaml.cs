using System;
using System.Data.SqlClient;
using System.Windows;

namespace LibraryManagementSystem
{
    public partial class AddBookWindow : Window
    {
        private string connectionString = "Data Source=ZAFARULLAH-NAUS\\SQLEXPRESS;Initial Catalog=LibraryManagementSystem;Integrated Security=True;";

        public AddBookWindow()
        {
            InitializeComponent();
        }

        private void AddBookButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO Books (Title, Author, Publisher, YearPublished) VALUES (@Title, @Author, @Publisher, @YearPublished)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Title", TitleTextBox.Text);
                    command.Parameters.AddWithValue("@Author", AuthorTextBox.Text);
                    command.Parameters.AddWithValue("@Publisher", PublisherTextBox.Text);
                    command.Parameters.AddWithValue("@YearPublished", YearTextBox.Text);

                    command.ExecuteNonQuery();
                    MessageBox.Show("Book added successfully.");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while adding the book: " + ex.Message);
            }
        }
    }
}
