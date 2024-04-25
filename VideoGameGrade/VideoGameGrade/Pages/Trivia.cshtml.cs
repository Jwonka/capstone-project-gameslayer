using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System;
using VideoGameGrade.Classes;
using static VideoGameGrade.Pages.GameCollectionModel;

namespace VideoGameGrade.Pages
{
    public class TriviaModel : PageModel
    {
        public List<TriviaList> triviaGame = new List<TriviaList>();
         public List<TriviaList> gameResult = new List<TriviaList>();
      
        public void OnGet()
        {
            
            try
            {
                // int curGameID = int.Parse(Request.Form["gameIdValue"]);
               
                string connectionString = "Server=videogamegrade.mysql.database.azure.com;Database=videogamegrade_db;Uid=gamegradeadmin;Pwd=capstone2024!;SslMode=Required;";
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    String sql = "SELECT gametable.gameID, gametable.gameTitle, triviatable.gameQuiz, triviatable.gameAnswer, triviatable.quizID\r\n FROM triviatable\r\n INNER JOIN gametable ON gametable.gameID = triviatable.gameID";


                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        //command.Parameters.AddWithValue("@gameIdValue", gameIdValue);
                        using(MySqlDataReader reader = command.ExecuteReader())
                        {
                           
                            while (reader.Read())
                                {
                                var gameId = Request.Query["id"];
                                TriviaList tList = new TriviaList();
                                
                                tList.gameIdValue = reader.GetInt32(0);
                                    tList.gameName = reader.GetString(1);
                                    tList.gameQuiz = reader.GetString(2);
                                    tList.gameAnswer = reader.GetString(3);
                                    tList.quizId = reader.GetInt32(4);
                                    if(tList.gameIdValue.ToString() == gameId)
                                {
                                    triviaGame.Add(tList);
                                   if(triviaGame.Count == 0)
                                    {
                                        Console.WriteLine("No questions to show");
                                    }
                                }
                             
                            }
                          
                        }
                    }
                }
             
            }
            catch (Exception ex) 
            {
                Console.WriteLine("Exception: " + ex.ToString());
                errorMsg = "Exception: " + ex.Message;
                 BadRequest();
            }
           
            }


      
    public class TriviaList
        {
           public int gameIdValue;
            public string gameName;
            public string gameQuiz;
            public string gameAnswer;
            public int quizId;
        }

        public TriviaList newQuestion = new TriviaList();
        public string errorMsg = "";
        public string successMsg = "";
        public int gameIdValue;

        public void OnPost()
        {
            string submitBtn = Request.Form["submitBtn"];
             string quizAnswer = HttpContext.Request.Query["quizAnswer"];
            if (!Request.Query.TryGetValue("id", out var gameID))
            {
                errorMsg = "Game ID is missing.";
                return;
            }

            if (!int.TryParse(gameID, out gameIdValue))
            {
                errorMsg = "Invalid Game ID format.";
                return;
            }
            if (submitBtn == "create")
            {
                newQuestion.gameQuiz = Request.Form["gameIdValue"];
                newQuestion.gameQuiz = Request.Form["gameQuiz"];
                newQuestion.gameAnswer = Request.Form["gameAnswer"];

                if (newQuestion.gameAnswer.Length == 0 || newQuestion.gameAnswer == null ||
                    newQuestion.gameQuiz.Length == 0 || newQuestion.gameQuiz == null)
                {
                    errorMsg = "All fields must be entered.";
                    OnGet();
                    return;
                }


                // save to database
                try
                {
                    string connectionString = "Server=videogamegrade.mysql.database.azure.com;Database=videogamegrade_db;Uid=gamegradeadmin;Pwd=capstone2024!;SslMode=Required;";
                    using (MySqlConnection connect = new MySqlConnection(connectionString))
                    {
                        connect.Open();
                        String sql = "INSERT INTO triviatable " +
                            "(gameQuiz, gameAnswer, gameID) VALUES " +
                            "(@gameQuiz, @gameAnswer,@gameIdValue);";

                        using (MySqlCommand command = new MySqlCommand(sql, connect))
                        {
                            command.Parameters.AddWithValue("@gameIdValue", gameIdValue);
                            command.Parameters.AddWithValue("@gameQuiz", newQuestion.gameQuiz);
                            command.Parameters.AddWithValue("@gameAnswer", newQuestion.gameAnswer);

                            command.ExecuteNonQuery();
                        }
                    }

                    successMsg = "New question was entered successfully!";
                }
                catch (Exception ex)
                {
                    errorMsg = ex.Message;
                    return;
                }
            }
            //CHeck answers after "submit"
           

            if (submitBtn == "quizSubmit")
            {
                int quizId;
                
                if (int.TryParse(Request.Form["quizId"], out quizId))
                {
                    TriviaList currentQuestion = triviaGame.FirstOrDefault(q => q.quizId == quizId);
                    if (currentQuestion != null)
                    {
                        if(quizAnswer.Trim().Equals(currentQuestion.gameAnswer.Trim(), StringComparison.OrdinalIgnoreCase)) {
                            successMsg = "Correct";
                        }
                        else
                        {
                            errorMsg = "Question not found";
                        }
                    }
                    else
                    {
                        errorMsg = "Invalid Quiz Id format";
                    }
                }

                
            }
            //quizAnswer = "";
            newQuestion.gameQuiz = "";
            newQuestion.gameAnswer = "";

            OnGet();
        }
    }
}
