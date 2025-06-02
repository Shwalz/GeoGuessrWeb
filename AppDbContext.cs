using GeoGuessrWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace GeoGuessrWeb
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Player> Players { get; set; }
        public DbSet<GameSession> GameSessions { get; set; }
        public DbSet<GameRound> GameRounds { get; set; }
        public DbSet<GameLocation> GameLocations { get; set; }
    }

}
