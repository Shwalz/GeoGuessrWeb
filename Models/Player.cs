namespace GeoGuessrWeb.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string Nickname { get; set; }
        public List<GameSession> GameSessions { get; set; }
    }
}
