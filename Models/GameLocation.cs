namespace GeoGuessrWeb.Models
{
    public class GameLocation
    {
        public int Id { get; set; }
        public string Continent { get; set; }
        public string Difficulty { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
