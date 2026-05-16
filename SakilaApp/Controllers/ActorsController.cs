using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SakilaApp.Models;

namespace SakilaApp.Controllers;

[Authorize]
public class ActorsController : Controller
{
    private readonly SakilaContext _context;
    private const int PageSize = 10;

    public ActorsController(SakilaContext context) => _context = context;

    public async Task<IActionResult> Index(string searchString, int page = 1)
    {
        var query = _context.Actors
            .Where(x => x.Active == 1)
            .AsQueryable();

        if (!string.IsNullOrEmpty(searchString))
        {
            query = query.Where(x => x.FirstName.Contains(searchString) || x.LastName.Contains(searchString));
            ViewData["SearchString"] = searchString;
        }

        query = query.OrderBy(x => x.LastName);
        var paginatedList = await PaginatedList<Actor>.CreateAsync(query, page, PageSize);
        return View(paginatedList);
    }

    public async Task<IActionResult> Details(int id)
    {
        var item = await _context.Actors.FindAsync(id);
        if (item == null || item.Active != 1) return NotFound();
        return View(item);
    }

    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Actor item)
    {
        if (!ModelState.IsValid) return View(item);
        item.LastUpdate = DateTime.Now;
        item.Active = 1;
        _context.Actors.Add(item);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Actor creado/a";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _context.Actors.FindAsync(id);
        if (item == null || item.Active != 1) return NotFound();
        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Actor item)
    {
        if (id != item.ActorId) return BadRequest();
        if (!ModelState.IsValid) return View(item);
        item.LastUpdate = DateTime.Now;
        _context.Actors.Update(item);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Actor actualizado/a";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.Actors.FindAsync(id);
        if (item == null || item.Active != 1) return NotFound();
        return View(item);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _context.Actors.FindAsync(id);
        if (item != null)
        {
            item.Active = 0;
            item.LastUpdate = DateTime.Now;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Actor eliminado/a";
        }
        return RedirectToAction(nameof(Index));
    }
}
