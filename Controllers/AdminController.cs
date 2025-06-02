using Microsoft.AspNetCore.Mvc;
using GeoGuessrWeb.Models;
using GeoGuessrWeb.Services;

namespace GeoGuessrWeb.Controllers;

public class AdminController : Controller
{
    private readonly LocationService _locationService;

    public AdminController(LocationService locationService)
    {
        _locationService = locationService;
    }

    [HttpGet("/Admin")]
    public IActionResult Index(string? searchTerm, string? sortOrder, int page = 1)
    {
        const int pageSize = 10;
        var locations = _locationService.GetAll();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            locations = locations
                .Where(l =>
                    (!string.IsNullOrEmpty(l.City) && l.City.ToLower().Contains(searchTerm)) ||
                    (!string.IsNullOrEmpty(l.Country) && l.Country.ToLower().Contains(searchTerm)))
                .ToList();
        }

        locations = sortOrder switch
        {
            "city_asc" => locations.OrderBy(l => l.City).ToList(),
            "city_desc" => locations.OrderByDescending(l => l.City).ToList(),
            "country_asc" => locations.OrderBy(l => l.Country).ToList(),
            "country_desc" => locations.OrderByDescending(l => l.Country).ToList(),
            "continent_asc" => locations.OrderBy(l => l.Continent).ToList(),
            "continent_desc" => locations.OrderByDescending(l => l.Continent).ToList(),
            _ => locations
        };

        var totalCount = locations.Count;
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        var paged = locations.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.CurrentSort = sortOrder;
        ViewBag.SearchTerm = searchTerm;

        return View(paged);
    }


    [HttpGet("/Admin/Add")]
    public IActionResult Add() => View(new Location());

    [HttpPost("/Admin/Add")]
    public IActionResult Add(Location location)
    {
        if (!ModelState.IsValid) return View(location);
        _locationService.Add(location);
        return RedirectToAction("Index");
    }

    [HttpGet("/Admin/Edit/{id}")]
    public IActionResult Edit(int id)
    {
        var location = _locationService.GetById(id);
        if (location == null) return NotFound();
        return View(location);
    }

    [HttpPost("/Admin/Edit/{id}")]
    public IActionResult Edit(int id, Location location)
    {
        if (!ModelState.IsValid) return View(location);
        _locationService.Update(id, location);
        return RedirectToAction("Index");
    }

    [HttpPost("/Admin/Delete")]
    public IActionResult Delete(int id)
    {
        _locationService.Delete(id);
        return RedirectToAction("Index");
    }

    [HttpGet("/Admin/Details/{id}")]
    public IActionResult Details(int id)
    {
        var location = _locationService.GetById(id);
        if (location == null) return NotFound();

        return View(location);
    }

    [HttpGet("/Admin/Dashboard")]
    public IActionResult Dashboard()
    {
        var locations = _locationService.GetAll();
        var leaderboard = LeaderboardStorage.Load();

        var topPlayers = leaderboard
            .OrderByDescending(p => p.Score)
            .Take(5)
            .ToList();

        var totalLocations = locations.Count;
        var totalPlayers = leaderboard.Count;
        var averageScore = leaderboard.Any() ? leaderboard.Average(p => p.Score) : 0;

        ViewBag.TopPlayers = topPlayers;
        ViewBag.TotalLocations = totalLocations;
        ViewBag.TotalPlayers = totalPlayers;
        ViewBag.AverageScore = Math.Round(averageScore, 2);

        return View();
    }

}
