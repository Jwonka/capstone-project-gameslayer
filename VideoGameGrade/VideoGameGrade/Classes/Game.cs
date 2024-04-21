using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VideoGameGrade.Classes
{
    [Table("gametable")]
    public class Game : ControllerContext
    {
      
        [Key]
        public int gameId { get; set; }
        

        [Required]
        [DisplayName("Game Title")]
        public string gameTitle { get; set; }

        public string gamePublisher{ get; set; }
        public string gameConsole { get; set; }
        public string gameCategory { get; set; }
        public string gameRating { get; set; }
        public string gameQuiz { get; set; }
        public byte[] gameImage { get; set; }
        public string gameAnswer { get; set; }
    }
}