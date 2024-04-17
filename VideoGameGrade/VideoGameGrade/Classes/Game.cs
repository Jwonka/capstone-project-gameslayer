using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace VideoGameGrade.Classes
{
    public class Game : ControllerContext
    {
        [Key]
        public int gameId { get; set; }

        [Required]
        [DisplayName("Game Title")]
        public string gameTitle { get; set; }

        public string gameCompany { get; set; }     
        public string gamePublisher { get; set; }
        public string gameDesc { get; set; }
        public string gameRating { get; set; }
        public string gameQuiz { get; set; }
        public string gameImage { get; set; }
        public string gameAnswer { get; set; }
    }
}
