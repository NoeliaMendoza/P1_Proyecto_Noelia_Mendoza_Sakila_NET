using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SakilaApp.Models;

namespace SakilaApp.Controllers;

[Authorize]
public class RentalsController : Controller
{
    private readonly SakilaContext _context;
    private const int PageSize = 10;

    public RentalsController(SakilaContext context) => _context = context;

    public async Task<IActionResult> Index(string searchString, int page = 1)
    {
        var query = _context.Rentals
            .Where(r => r.Active == 1)
            .AsQueryable();

        if (!string.IsNullOrEmpty(searchString))
        {
            if (int.TryParse(searchString, out int id))
            {
                query = query.Where(r => r.CustomerId == id || r.InventoryId == id);
            }
            ViewData["SearchString"] = searchString;
        }

        query = query.OrderByDescending(r => r.RentalDate);
        var paginatedList = await PaginatedList<Rental>.CreateAsync(query, page, PageSize);
        return View(paginatedList);
    }

    public async Task<IActionResult> Details(int id)
    {
        var rental = await _context.Rentals.FindAsync(id);
        if (rental == null || rental.Active != 1) return NotFound();
        return View(rental);
    }

    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Rental rental)
    {
        if (!ModelState.IsValid) return View(rental);
        rental.LastUpdate = DateTime.Now;
        rental.Active = 1;
        _context.Rentals.Add(rental);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Alquiler creado";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var rental = await _context.Rentals.FindAsync(id);
        if (rental == null || rental.Active != 1) return NotFound();
        return View(rental);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Rental rental)
    {
        if (id != rental.RentalId) return BadRequest();
        if (!ModelState.IsValid) return View(rental);
        rental.LastUpdate = DateTime.Now;
        _context.Rentals.Update(rental);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Alquiler actualizado";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var rental = await _context.Rentals.FindAsync(id);
        if (rental == null || rental.Active != 1) return NotFound();
        return View(rental);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var rental = await _context.Rentals.FindAsync(id);
        if (rental != null)
        {
            rental.Active = 0;
            rental.LastUpdate = DateTime.Now;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Alquiler eliminado";
        }
        return RedirectToAction(nameof(Index));
    }
}
