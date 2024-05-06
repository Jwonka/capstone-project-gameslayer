using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using VideoGameGrade.Classes;
using Dapper;
using System.Data;
using System.Security.Claims;

namespace VideoGameGrade.Pages
{
    public class TriviaModel : PageModel
    {
        public List<TriviaList> triviaGame = new List<TriviaList>();
        public List<int> EnteredIds { get; set; } = new List<int>();
        public bool isCorrect { get; set; }
        public string errorMsg = "";
        public string successMsg = "";
        public TriviaList newQuestion = new TriviaList();
        public int gameIdValue;

        public IDbConnection _db;

        public TriviaModel(IDbConnection db)
        {
            _db = db;
        }

        public void OnGet()
        {
            try
            {
                string connectionString = "Server=videogamegrade.mysql.database.azure.com;Database=videogamegrade_db;Uid=gamegradeadmin;Pwd=capstone2024!;SslMode=Required;";
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT gametable.gameID, gametable.gameTitle, triviatable.gameQuiz, triviatable.gameAnswer, triviatable.quizID " +
                                 "FROM triviatable " +
                                 "INNER JOIN gametable ON gametable.gameID = triviatable.gameID";

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
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMsg = "Exception: " + ex.Message;
                BadRequest();
            }
        }

        public void OnPost(int quizId)
        {
            string submitBtn = Request.Form["submitBtn"];
            string quizAnswer = Request.Form["quizAnswer"];

            if (Request.Form.ContainsKey("submitBtn" + quizId))
            {
                EnteredIds.Add(quizId);

                if (string.IsNullOrWhiteSpace(quizAnswer))
                {
                    errorMsg = "Please enter an answer";
                    return;
                }

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
                            string correctAnswer = command.ExecuteScalar()?.ToString();
                            correctAnswer = correctAnswer.Trim().ToLower().Replace("-", " ");
                            quizAnswer = quizAnswer.Trim().ToLower().Replace("-", " ");

                            isCorrect = correctAnswer != null && quizAnswer.Contains(correctAnswer);
                        }
                    }
                }
                catch (Exception ex)
                {
                    errorMsg = "Exception: " + ex.Message;
                    BadRequest();
                }

                var userEmail = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
                var userId = _db.QuerySingleOrDefault<int>("SELECT userId FROM usertable WHERE userName = @UserName", new { UserName = userEmail });

                if (userId == 0)
                {
                    errorMsg = "User not found.";
                    return;
                }

                try
                {
                    string connectionString = "Server=videogamegrade.mysql.database.azure.com;Database=videogamegrade_db;Uid=gamegradeadmin;Pwd=capstone2024!;SslMode=Required;";
                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        connection.Open();

                        string checkSql = "SELECT COUNT(*) FROM user_trivia_stats WHERE userId = @UserId";
                        int exists = connection.ExecuteScalar<int>(checkSql, new { UserId = userId });

                        if (exists == 0)
                        {
                            string insertSql = "INSERT INTO user_trivia_stats (userId, answered_count, correct_count) VALUES (@UserId, 1, @CorrectCount)";
                            connection.Execute(insertSql, new { UserId = userId, CorrectCount = isCorrect ? 1 : 0 });
                        }
                        else
                        {
                            string updateSql = "UPDATE user_trivia_stats SET answered_count = answered_count + 1, correct_count = correct_count + @CorrectIncrement WHERE userId = @UserId";
                            connection.Execute(updateSql, new { UserId = userId, CorrectIncrement = isCorrect ? 1 : 0 });
                        }
                    }
                }
                catch (Exception ex)
                {
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

            if (submitBtn == "create")
            {
                newQuestion.gameQuiz = Request.Form["gameQuiz"];
                newQuestion.gameAnswer = Request.Form["gameAnswer"];

                if (string.IsNullOrEmpty(newQuestion.gameAnswer) || string.IsNullOrEmpty(newQuestion.gameQuiz))
                {
                    errorMsg = "All fields must be entered.";
                    OnGet();
                    return;
                }

                var userEmail = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
                var userId = _db.QuerySingleOrDefault<int>("SELECT userId FROM usertable WHERE userName = @UserName", new { UserName = userEmail });

                if (userId == 0)
                {
                    errorMsg = "User not found.";
                    return;
                }

                try
                {
                    string connectionString = "Server=videogamegrade.mysql.database.azure.com;Database=videogamegrade_db;Uid=gamegradeadmin;Pwd=capstone2024!;SslMode=Required;";
                    using (MySqlConnection connect = new MySqlConnection(connectionString))
                    {
                        connect.Open();
                        string sql = "INSERT INTO triviatable (gameQuiz, gameAnswer, gameID, userId) VALUES (@gameQuiz, @gameAnswer, @gameIdValue, @userId)";

                        using (MySqlCommand command = new MySqlCommand(sql, connect))
                        {
                            command.Parameters.AddWithValue("@gameIdValue", gameIdValue);
                            command.Parameters.AddWithValue("@gameQuiz", newQuestion.gameQuiz);
                            command.Parameters.AddWithValue("@gameAnswer", newQuestion.gameAnswer);
                            command.Parameters.AddWithValue("@userId", userId);

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

        public (int answeredCount, int correctCount) GetUserTriviaStats(int userId)
        {
            string sql = "SELECT answered_count, correct_count FROM user_trivia_stats WHERE userId = @UserId";
            return _db.QuerySingleOrDefault<(int, int)>(sql, new { UserId = userId });
        }

        public class TriviaList
        {
            public int gameIdValue;
            public string gameName;
            public string gameQuiz;
            public string gameAnswer;
            public int quizId;
        }
    }
}