using System.ComponentModel.DataAnnotations;

namespace GeoGuessrWeb.Models
{
    public class Location
    {
        public int Id { get; set; }

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        [Required]
        [StringLength(100)]
        public string City { get; set; }

        [Required]
        [StringLength(100)]
        public string Country { get; set; }

        [Required]
        [StringLength(100)]
        public string Continent { get; set; }

        [Required]
        [StringLength(50)]
        public string Difficulty { get; set; }
    }
}
