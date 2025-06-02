using System.Text.Json;
using GeoGuessrWeb.Models;

public class LocationService
{
    private readonly string filePath = Path.Combine("Data", "locations.json");

    private Dictionary<string, Dictionary<string, List<Location>>> locations;

    public LocationService()
    {
        Load();
    }

    public void Load()
    {
        if (!File.Exists(filePath))
        {
            locations = new();
            return;
        }

        var json = File.ReadAllText(filePath);
        var raw = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, List<Location>>>>(json)
                  ?? new();

        locations = new Dictionary<string, Dictionary<string, List<Location>>>();

        foreach (var contPair in raw)
        {
            var normCont = Normalize(contPair.Key);
            if (!locations.ContainsKey(normCont))
                locations[normCont] = new();

            foreach (var diffPair in contPair.Value)
            {
                var normDiff = Normalize(diffPair.Key);
                if (!locations[normCont].ContainsKey(normDiff))
                    locations[normCont][normDiff] = new();

                locations[normCont][normDiff].AddRange(diffPair.Value);
            }
        }
    }


    public void Save()
    {
        var json = JsonSerializer.Serialize(locations, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
    }

    public List<Location> GetAll() =>
        locations.SelectMany(cont => cont.Value.SelectMany(diff => diff.Value
            .Select(loc =>
            {
                loc.Continent = cont.Key;
                loc.Difficulty = diff.Key;
                return loc;
            }))).ToList();

    public Location? GetById(int id) =>
        GetAll().FirstOrDefault(l => l.Id == id);

    public void Add(Location location)
    {
        var continentKey = Normalize(location.Continent);
        var difficultyKey = Normalize(location.Difficulty);

        if (!locations.ContainsKey(continentKey))
            locations[continentKey] = new();

        if (!locations[continentKey].ContainsKey(difficultyKey))
            locations[continentKey][difficultyKey] = new();

        location.Id = GenerateId();
        locations[continentKey][difficultyKey].Add(location);
        Save();
    }

    public void Update(int id, Location updated)
    {
        Delete(id);
        updated.Id = id;
        Add(updated);
    }

    public void Delete(int id)
    {
        foreach (var cont in locations)
        {
            foreach (var diff in cont.Value)
            {
                var loc = diff.Value.FirstOrDefault(x => x.Id == id);
                if (loc != null)
                {
                    diff.Value.Remove(loc);
                    Save();
                    return;
                }
            }
        }
    }

    public Location GetRandomLocation(string continent, string difficulty, List<int> usedIds)
    {
        var contKey = Normalize(continent);
        var diffKey = Normalize(difficulty);

        if (!locations.ContainsKey(contKey) || !locations[contKey].ContainsKey(diffKey))
            throw new Exception("No such category");

        var pool = locations[contKey][diffKey].Where(loc => !usedIds.Contains(loc.Id)).ToList();

        if (pool.Count == 0)
            throw new Exception("No new locations available");

        return pool[new Random().Next(pool.Count)];
    }

    private int GenerateId()
    {
        var allIds = GetAll().Select(l => l.Id).ToList();
        int id = 1;
        while (allIds.Contains(id)) id++;
        return id;
    }

    private string Normalize(string input) =>
        input.Trim().ToLowerInvariant();
}
