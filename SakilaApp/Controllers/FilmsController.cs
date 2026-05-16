using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SakilaApp.Models;

namespace SakilaApp.Controllers;

[Authorize]
public class FilmsController : Controller
{
    private readonly SakilaContext _context;
    private const int PageSize = 10;

    public FilmsController(SakilaContext context) => _context = context;

    public async Task<IActionResult> Index(string searchString, int pageNumber = 1)
    {
        var films = _context.Films
            .Where(f => f.Active == 1)
            .AsQueryable();

        if (!string.IsNullOrEmpty(searchString))
        {
            films = films.Where(f => f.Title.Contains(searchString));
            ViewData["SearchString"] = searchString;
        }

        var paginatedFilms = await PaginatedList<Film>.CreateAsync(
            films.OrderBy(f => f.Title), pageNumber, PageSize);

        return View(paginatedFilms);
    }

    public async Task<IActionResult> Details(int id)
    {
        var film = await _context.Films
            .Include(f => f.FilmActors)
            .ThenInclude(fa => fa.Actor)
            .FirstOrDefaultAsync(f => f.FilmId == id);
        if (film == null || film.Active != 1) return NotFound();
        return View(film);
    }

    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Film film)
    {
        if (!ModelState.IsValid) return View(film);
        film.LastUpdate = DateTime.Now;
        film.LanguageId = 1;
        film.Active = 1;
        _context.Films.Add(film);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Película creada";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var film = await _context.Films.FindAsync(id);
        if (film == null || film.Active != 1) return NotFound();
        return View(film);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Film film)
    {
        if (id != film.FilmId) return BadRequest();
        if (!ModelState.IsValid) return View(film);
        film.LastUpdate = DateTime.Now;
        film.LanguageId = 1;
        _context.Films.Update(film);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Película actualizada";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var film = await _context.Films.FindAsync(id);
        if (film == null || film.Active != 1) return NotFound();
        return View(film);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var film = await _context.Films.FindAsync(id);
        if (film != null)
        {
            film.Active = 0;
            film.LastUpdate = DateTime.Now;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Película eliminada";
        }
        return RedirectToAction(nameof(Index));
    }
}