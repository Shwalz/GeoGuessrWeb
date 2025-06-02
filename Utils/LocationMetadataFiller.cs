using System.Text.Json;
using GeoGuessrWeb.Models;

namespace GeoGuessrWeb.Utils
{
    public static class LocationMetadataFiller
    {
        private static readonly string FilePath = Path.Combine("Data", "locations.json");

        public static void FillContinentAndDifficulty()
        {
            if (!File.Exists(FilePath))
            {
                Console.WriteLine("Файл locations.json не найден по пути: " + FilePath);
                return;
            }

            var json = File.ReadAllText(FilePath);
            var data = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, List<Location>>>>(json);

            if (data == null)
            {
                Console.WriteLine("Ошибка десериализации JSON.");
                return;
            }

            foreach (var (continent, difficulties) in data)
            {
                foreach (var (difficulty, locations) in difficulties)
                {
                    foreach (var loc in locations)
                    {
                        loc.Continent = continent;
                        loc.Difficulty = difficulty;
                    }
                }
            }

            var options = new JsonSerializerOptions { WriteIndented = true };
            var updatedJson = JsonSerializer.Serialize(data, options);
            File.WriteAllText(FilePath, updatedJson);

            Console.WriteLine("✅ Континент и сложность успешно проставлены во всех локациях.");
        }
    }
}
