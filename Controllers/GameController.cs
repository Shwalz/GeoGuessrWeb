using Microsoft.AspNetCore.Mvc;
using GeoGuessrWeb.Models;
using GeoGuessrWeb.Services;
using System.Globalization;

namespace GeoGuessrWeb.Controllers
{
    public class GameController : Controller
    {
        private readonly LocationService _locationService;
        private static List<int> usedIds = new();
        private static int round = 1;
        private static int score = 0;
        private static int totalRounds = 5;
        private static Location? currentLocation;
        private static Player? currentPlayer;

        public GameController(LocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpGet("/Game/Start")]
        public IActionResult Start() => View();

        [HttpPost("/Game/Start")]
        public IActionResult Start(string nickname, string continent, string difficulty, int rounds)
        {
            currentPlayer = new Player { Nickname = nickname };
            round = 1;
            score = 0;
            totalRounds = rounds;
            usedIds.Clear();
            currentLocation = null;

            HttpContext.Session.SetString("continent", continent);
            HttpContext.Session.SetString("difficulty", difficulty);

            return RedirectToAction("Play");
        }

        [HttpGet("/Game/Play")]
        public IActionResult Play()
        {
            var continent = HttpContext.Session.GetString("continent");
            var difficulty = HttpContext.Session.GetString("difficulty");

            if (string.IsNullOrEmpty(continent) || string.IsNullOrEmpty(difficulty))
                return RedirectToAction("Start");

            if (round > totalRounds)
                return RedirectToAction("Finish");

            bool resultVisible = TempData["resultVisible"]?.ToString() == "true";

            if (!resultVisible && currentLocation == null)
            {
                try
                {
                    currentLocation = _locationService.GetRandomLocation(continent, difficulty, usedIds);
                    usedIds.Add(currentLocation.Id);
                }
                catch
                {
                    return RedirectToAction("Finish");
                }
            }

            ViewBag.Location = currentLocation;
            ViewBag.Round = round;
            ViewBag.TotalRounds = totalRounds;

            ViewBag.ResultVisible = resultVisible;
            ViewBag.Distance = TempData["distance"];
            ViewBag.RoundScore = TempData["roundScore"];
            ViewBag.TotalScore = TempData["totalScore"];
            ViewBag.GuessedLat = TempData["guessedLat"];
            ViewBag.GuessedLng = TempData["guessedLng"];
            ViewBag.CorrectLat = currentLocation?.Latitude.ToString(CultureInfo.InvariantCulture);
            ViewBag.CorrectLng = currentLocation?.Longitude.ToString(CultureInfo.InvariantCulture);

            return View();
        }

        [HttpPost("/Game/Guess")]
        public IActionResult Guess(string guessedLat, string guessedLng)
        {
            var continent = HttpContext.Session.GetString("continent");
            var difficulty = HttpContext.Session.GetString("difficulty");

            if (currentLocation == null || continent == null || difficulty == null)
                return RedirectToAction("Start");

            // Преобразование координат вручную
            double guessedLatVal = double.TryParse(guessedLat, CultureInfo.InvariantCulture, out var latVal) ? latVal : 0;
            double guessedLngVal = double.TryParse(guessedLng, CultureInfo.InvariantCulture, out var lngVal) ? lngVal : 0;

            // Если координаты не выбраны — возвращаем ошибку
            if (guessedLatVal == 0 && guessedLngVal == 0)
            {
                TempData["error"] = "Invalid guess: coordinates not selected.";
                return RedirectToAction("Play");
            }

            double distance = GeoUtils.CalculateDistance(currentLocation.Latitude, currentLocation.Longitude, guessedLatVal, guessedLngVal);
            int roundScore = ScoreCalculator.CalculateScore(distance);
            score += roundScore;

            TempData["distance"] = distance.ToString("0.00", CultureInfo.InvariantCulture);
            TempData["roundScore"] = roundScore.ToString();
            TempData["totalScore"] = score.ToString();
            TempData["correctLat"] = currentLocation.Latitude.ToString(CultureInfo.InvariantCulture);
            TempData["correctLng"] = currentLocation.Longitude.ToString(CultureInfo.InvariantCulture);
            TempData["guessedLat"] = guessedLatVal.ToString(CultureInfo.InvariantCulture);
            TempData["guessedLng"] = guessedLngVal.ToString(CultureInfo.InvariantCulture);
            TempData["resultVisible"] = "true";

            return RedirectToAction("Play");
        }


        [HttpGet("/Game/Next")]
        public IActionResult Next()
        {
            round++;
            currentLocation = null;
            TempData["resultVisible"] = "false";
            return RedirectToAction("Play");
        }

        [HttpGet("/Game/Finish")]
        public IActionResult Finish()
        {
            var leaderboard = LeaderboardStorage.Load();

            if (currentPlayer != null)
            {
                var existing = leaderboard.FirstOrDefault(x => x.Nickname == currentPlayer.Nickname);

                if (existing != null)
                {
                    if (score > existing.Score)
                    {
                        existing.Score = score; // Обновить, если новый результат выше
                    }
                }
                else
                {
                    leaderboard.Add(new LeaderboardEntry
                    {
                        Nickname = currentPlayer.Nickname,
                        Score = score
                    });
                }

                LeaderboardStorage.Save(leaderboard);
            }

            ViewBag.Nickname = currentPlayer?.Nickname ?? "Unknown";
            ViewBag.Score = score;

            ViewBag.Leaderboard = leaderboard
                .GroupBy(x => x.Nickname)
                .Select(g => new LeaderboardEntry
                {
                    Nickname = g.Key,
                    Score = g.Max(e => e.Score)
                })
                .OrderByDescending(e => e.Score)
                .Take(10)
                .ToList();

            return View();
        }

    }
}
