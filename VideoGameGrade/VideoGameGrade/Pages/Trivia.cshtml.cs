using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using String = System.String;

namespace VideoGameGrade.Pages
{
    public class TriviaModel : PageModel
    {
        public List<TriviaList> triviaGame = new List<TriviaList>();
        
        //to keep track of entered questions
        public List<int> EnteredIds {  get; set; }  = new List<int>();

        public void OnGet()
        {

            try
            {
            
                string connectionString = "Server=videogamegrade.mysql.database.azure.com;Database=videogamegrade_db;Uid=gamegradeadmin;Pwd=capstone2024!;SslMode=Required;";
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    String sql = "SELECT gametable.gameID, gametable.gameTitle, triviatable.gameQuiz, triviatable.gameAnswer, triviatable.quizID\r\n FROM triviatable\r\n INNER JOIN gametable ON gametable.gameID = triviatable.gameID";


                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
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
                                if (tList.gameIdValue.ToString() == gameId)
                                {
                                    triviaGame.Add(tList);
                                    if (triviaGame.Count == 0)
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
        public IActionResult SetCookie()
        {
            Response.Cookies.Append("CorrectCount", "value", new CookieOptions
            {
                Secure = true, // Mark the cookie as Secure
                HttpOnly = true // Mark the cookie as HttpOnly
            });

            return Page();
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
    
        public bool isCorrect { get; set; }

        public void OnPost(int quizId)
        { 
            string submitBtn = Request.Form["submitBtn"];
            string quizAnswer = Request.Form["quizAnswer"];

            if (Request.Form.ContainsKey("submitBtn" + quizId))
            {
                //if question was answered add to list
                EnteredIds.Add(quizId);

                if (string.IsNullOrWhiteSpace(quizAnswer))
                {
                    errorMsg = "Please enter an answer";
                    return;
                }
                //correct or not?
                
             

                try
                {


                    string connectionString = "Server=videogamegrade.mysql.database.azure.com;Database=videogamegrade_db;Uid=gamegradeadmin;Pwd=capstone2024!;SslMode=Required;";
                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        connection.Open();

                        string sql = "SELECT gameAnswer FROM triviatable WHERE quizID = @quizId";


                        using (MySqlCommand command = new MySqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@quizId", quizId);
                            //pulls correct Answer from the database
                            string correctAnswer = command.ExecuteScalar()?.ToString();
                            //check answer string
                            if (correctAnswer != null && quizAnswer.Trim().Equals(correctAnswer.Trim(), StringComparison.OrdinalIgnoreCase))
                            {
                                isCorrect = true;
                            }
                            else
                            {
                                isCorrect= false;
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
            //create and save new question
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


                // save new question to database
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
           
                newQuestion.gameQuiz = "";
                newQuestion.gameAnswer = "";

                OnGet();

                
            }
        }
      
    }
