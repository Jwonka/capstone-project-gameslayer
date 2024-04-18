using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System;

namespace VideoGameGrade.Pages
{
    public class TestPageModel : PageModel
    {
        public bool DatabaseConnected { get; set; }

        public void OnGet()
        {
            // Construct the connection string
            string connectionString = "Server=videogamegrade.mysql.database.azure.com;Uid=gamegradeadmin;Pwd=capstone2024!;SslMode=Required;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    DatabaseConnected = connection.State == System.Data.ConnectionState.Open;
                }
                catch (Exception)
                {
                    DatabaseConnected = false;
                }
            }
        }
    }
}