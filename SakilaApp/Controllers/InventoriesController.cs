using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SakilaApp.Models;

namespace SakilaApp.Controllers;

[Authorize]
public class InventoriesController : Controller
{
    private readonly SakilaContext _context;
    private const int PageSize = 10;

    public InventoriesController(SakilaContext context) => _context = context;

    public async Task<IActionResult> Index(string searchString, int page = 1)
    {
        var query = _context.Inventories
            .Where(x => x.Active == 1)
            .AsQueryable();

        if (!string.IsNullOrEmpty(searchString))
        {
            if (int.TryParse(searchString, out int id))
            {
                query = query.Where(x => x.FilmId == id || x.StoreId == id);
            }
            ViewData["SearchString"] = searchString;
        }

        query = query.OrderBy(x => x.InventoryId);
        var paginatedList = await PaginatedList<Inventory>.CreateAsync(query, page, PageSize);
        return View(paginatedList);
    }

    public async Task<IActionResult> Details(int id)
    {
        var item = await _context.Inventories.FindAsync(id);
        if (item == null || item.Active != 1) return NotFound();
        return View(item);
    }

    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Inventory item)
    {
        if (!ModelState.IsValid) return View(item);
        item.LastUpdate = DateTime.Now;
        item.Active = 1;
        _context.Inventories.Add(item);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Inventario creado/a";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _context.Inventories.FindAsync(id);
        if (item == null || item.Active != 1) return NotFound();
        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Inventory item)
    {
        if (id != item.InventoryId) return BadRequest();
        if (!ModelState.IsValid) return View(item);
        item.LastUpdate = DateTime.Now;
        _context.Inventories.Update(item);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Inventario actualizado/a";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.Inventories.FindAsync(id);
        if (item == null || item.Active != 1) return NotFound();
        return View(item);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _context.Inventories.FindAsync(id);
        if (item != null)
        {
            item.Active = 0;
            item.LastUpdate = DateTime.Now;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Inventario eliminado/a";
        }
        return RedirectToAction(nameof(Index));
    }
}
