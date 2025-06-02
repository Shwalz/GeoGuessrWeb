using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using GeoGuessrWeb.Models;

namespace GeoGuessrWeb.Services
{
    public static class LeaderboardStorage
    {
        public static string Path => System.IO.Path.Combine("Data", "leaderboard.json");

        public static List<LeaderboardEntry> Load()
        {
            if (!File.Exists(Path))
                return new List<LeaderboardEntry>();

            var json = File.ReadAllText(Path);
            return JsonSerializer.Deserialize<List<LeaderboardEntry>>(json) ?? new();
        }


        public static void Save(List<LeaderboardEntry> entries)
        {
            var directory = System.IO.Path.GetDirectoryName(Path);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory!);

            var json = JsonSerializer.Serialize(entries, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(Path, json);
        }

    }
}
