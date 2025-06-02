namespace GeoGuessrWeb.Models
{
    public class GameSession
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public Player Player { get; set; }
        public int Score { get; set; }
        public DateTime PlayedAt { get; set; }
        public List<GameRound> Rounds { get; set; }
    }
}
