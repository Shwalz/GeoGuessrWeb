namespace GeoGuessrWeb.Models
{
    public class GameRound
    {
        public int Id { get; set; }
        public int GameSessionId { get; set; }
        public GameSession GameSession { get; set; }
        public int GameLocationId { get; set; }
        public GameLocation GameLocation { get; set; }
        public double GuessedLat { get; set; }
        public double GuessedLng { get; set; }
        public double DistanceKm { get; set; }
        public int RoundScore { get; set; }
    }
}
