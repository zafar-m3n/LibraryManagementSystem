using System;
using System.Data.SqlClient;
using System.Windows;

namespace LibraryManagementSystem
{
    public partial class EditBookWindow : Window
    {
        private string connectionString = "Data Source=ZAFARULLAH-NAUS\\SQLEXPRESS;Initial Catalog=LibraryManagementSystem;Integrated Security=True;";
        private int bookId;

        public EditBookWindow(int bookId)
        {
            InitializeComponent();
            this.bookId = bookId;
            LoadBookDetails();
        }

        private void LoadBookDetails()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM Books WHERE BookID = @BookID";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@BookID", bookId);
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        TitleTextBox.Text = reader["Title"].ToString();
                        AuthorTextBox.Text = reader["Author"].ToString();
                        PublisherTextBox.Text = reader["Publisher"].ToString();
                        YearTextBox.Text = reader["YearPublished"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading book details: " + ex.Message);
            }
        }

        private void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "UPDATE Books SET Title = @Title, Author = @Author, Publisher = @Publisher, YearPublished = @YearPublished WHERE BookID = @BookID";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@BookID", bookId);
                    command.Parameters.AddWithValue("@Title", TitleTextBox.Text);
                    command.Parameters.AddWithValue("@Author", AuthorTextBox.Text);
                    command.Parameters.AddWithValue("@Publisher", PublisherTextBox.Text);
                    command.Parameters.AddWithValue("@YearPublished", YearTextBox.Text);

                    command.ExecuteNonQuery();
                    MessageBox.Show("Book updated successfully.");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while updating the book: " + ex.Message);
            }
        }
    }
}
