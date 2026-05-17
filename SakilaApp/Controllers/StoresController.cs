using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SakilaApp.Models;

namespace SakilaApp.Controllers;

[Authorize]
public class StoresController : Controller
{
    private readonly SakilaContext _context;
    private const int PageSize = 10;

    public StoresController(SakilaContext context) => _context = context;

    public async Task<IActionResult> Index(string searchString, int pageNumber = 1)
    {
        if (pageNumber < 1) pageNumber = 1;

        var stores = _context.Stores
            .Where(s => s.Active == 1)
            .AsQueryable();

        if (!string.IsNullOrEmpty(searchString))
        {
            if (int.TryParse(searchString, out int id))
            {
                stores = stores.Where(s => s.StoreId == id || s.ManagerStaffId == id);
            }
            ViewData["SearchString"] = searchString;
        }

        stores = stores.OrderBy(s => s.StoreId);
        var paginatedStores = await PaginatedList<Store>.CreateAsync(stores, pageNumber, PageSize);
        return View(paginatedStores);
    }

    public async Task<IActionResult> Details(int id)
    {
        var store = await _context.Stores.FindAsync(id);
        if (store == null || store.Active != 1) return NotFound();
        return View(store);
    }

    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Store store)
    {
        if (!ModelState.IsValid) return View(store);

        // comprobar que la dirección no esté ya asignada a otra tienda (índice único en BD)
        if (store.AddressId <= 0)
        {
            ModelState.AddModelError("AddressId", "ID de dirección inválido.");
            return View(store);
        }
        // el índice único en la BD impide duplicados independientemente de la columna Active,
        // por eso comprobamos sin filtrar por Active.
        var exists = await _context.Stores.AnyAsync(s => s.AddressId == store.AddressId);
        if (exists)
        {
            ModelState.AddModelError("AddressId", "La dirección ya está asignada a otra tienda.");
            return View(store);
        }

        store.LastUpdate = DateTime.Now;
        store.Active = 1;
        _context.Stores.Add(store);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            // posible condición de carrera u otro índice único
            ModelState.AddModelError(string.Empty, "No se pudo crear la tienda: duplicado en la dirección.");
            return View(store);
        }
        TempData["Success"] = "Tienda creada";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var store = await _context.Stores.FindAsync(id);
        if (store == null || store.Active != 1) return NotFound();
        return View(store);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Store store)
    {
        if (id != store.StoreId) return BadRequest();
        if (!ModelState.IsValid) return View(store);
        store.LastUpdate = DateTime.Now;
        store.Active = 1;
        _context.Stores.Update(store);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Tienda actualizada";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var store = await _context.Stores.FindAsync(id);
        if (store == null || store.Active != 1) return NotFound();
        return View(store);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var store = await _context.Stores.FindAsync(id);
        if (store == null) return NotFound();

        store.Active = 0;
        store.LastUpdate = DateTime.Now;
        await _context.SaveChangesAsync();
        TempData["Success"] = "Tienda eliminada";
        return RedirectToAction(nameof(Index));
    }
}