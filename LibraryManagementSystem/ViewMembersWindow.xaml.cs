using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace LibraryManagementSystem
{
    public partial class ViewMembersWindow : Window
    {
        private string connectionString = "Data Source=ZAFARULLAH-NAUS\\SQLEXPRESS;Initial Catalog=LibraryManagementSystem;Integrated Security=True;";

        public ViewMembersWindow()
        {
            InitializeComponent();
            LoadMembersData();
        }

        private void LoadMembersData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT Users.Name, Users.Email, Members.NumberOfBorrowedBooks FROM Users INNER JOIN Members ON Users.UserID = Members.UserID";
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

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            LibrarianDashboard librarianDashboard = new LibrarianDashboard();
            librarianDashboard.Show();
            this.Close();
        }
    }
}
