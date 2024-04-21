using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using VideoGameGrade.Classes;

namespace VideoGameGrade.Pages
{
   
    public class TriviaModel : PageModel
    {
        public List<TriviaList> triviaGame = new List<TriviaList>();
        

        public void OnGet()
        {
            try
            {
                 string connectionString = "Server=videogamegrade.mysql.database.azure.com;Database=videogamegrade_db;Uid=gamegradeadmin;Pwd=capstone2024!;SslMode=Required;";
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "SELECT gametable.gameID, gametable.gameTitle, triviatable.gameQuiz, triviatable.gameAnswer\r\n FROM triviatable\r\n INNER JOIN gametable ON gametable.gameID = triviatable.gameID;";
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        using(MySqlDataReader reader = command.ExecuteReader())
                        {
                            while(reader.Read())
                            {
                                TriviaList tList = new TriviaList();
                                tList.gameId = reader.GetInt32(0);
                                tList.gameName = reader.GetString(1);
                                tList.gameQuiz = reader.GetString(2);
                                tList.gameAnswer= reader.GetString(3);

                                triviaGame.Add(tList);
                            }
                        }
                    }
                }
            }
            catch (Exception ex) 
            {
                
            }
        }

        public class TriviaList
        {
            public int gameId;
            public string gameName;
            public string gameQuiz;
            public string gameAnswer;
        }


        public TriviaList newQuestion = new TriviaList();
        public string errorMsg = "";
        public string successMsg = "";
        public void OnPost()
        {
            newQuestion.gameQuiz = Request.Form["gameQuiz"];
            newQuestion.gameAnswer = Request.Form["gameAnswer"];

            if(newQuestion.gameAnswer.Length == 0 || newQuestion.gameAnswer == null ||
                newQuestion.gameQuiz.Length == 0 || newQuestion.gameQuiz == null)
            {
                errorMsg = "All fields must be entered.";
                
                return;
                
            }
           
            // save to database
            try
            {
                string connectionString = "Server=videogamegrade.mysql.database.azure.com;Database=videogamegrade_db;Uid=gamegradeadmin;Pwd=capstone2024!;SslMode=Required;";
                using (MySqlConnection connect = new MySqlConnection( connectionString)) {
                    connect.Open();
                    String sql = "INSERT INTO triviatable " +
                        "(gameQuiz, gameAnswer) VALUES " +
                        "(@gameQuiz, @gameAnswer);";

                    using (MySqlCommand command = new MySqlCommand(sql, connect))
                    {
                        command.Parameters.AddWithValue("@gameQuiz", newQuestion.gameQuiz);
                        command.Parameters.AddWithValue("@gameAnswer", newQuestion.gameAnswer);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return;
            }


            newQuestion.gameQuiz = "";
            newQuestion.gameAnswer = "";

            successMsg = "New question was entered successfully!";
            OnGet();
        }

      

    }
}
