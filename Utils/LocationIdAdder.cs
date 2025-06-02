using System.Text.Json;
using GeoGuessrWeb.Models;

namespace GeoGuessrWeb.Utils;

public static class LocationIdAdder
{
    public static void AddIds()
    {
        var path = Path.Combine("Data", "locations.json");
        if (!File.Exists(path))
        {
            Console.WriteLine("Файл не найден: " + path);
            return;
        }

        var json = File.ReadAllText(path);
        var data = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, List<Location>>>>(json);

        if (data == null)
        {
            Console.WriteLine("Ошибка десериализации JSON");
            return;
        }

        var usedIds = new HashSet<int>();
        foreach (var c in data)
        {
            foreach (var d in c.Value)
            {
                foreach (var loc in d.Value)
                {
                    if (loc.Id != 0)
                        usedIds.Add(loc.Id);
                }
            }
        }

        int nextId = 1;
        foreach (var c in data)
        {
            foreach (var d in c.Value)
            {
                foreach (var loc in d.Value)
                {
                    if (loc.Id == 0)
                    {
                        while (usedIds.Contains(nextId)) nextId++;
                        loc.Id = nextId++;
                        usedIds.Add(loc.Id);
                    }
                }
            }
        }

        var newJson = JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(path, newJson);
        Console.WriteLine("ID добавлены ко всем локациям без ID.");
    }
}
