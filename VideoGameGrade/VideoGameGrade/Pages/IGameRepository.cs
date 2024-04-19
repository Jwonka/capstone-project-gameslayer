using VideoGameGrade.Classes;

namespace VideoGameGrade.Pages
{
    public interface IGameRepository
    {
        IEnumerable<Game> GetAllGames();

    }
}