using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SakilaApp.Models;

namespace SakilaApp.Controllers;

[Authorize]
public class CustomersController : Controller
{
    private readonly SakilaContext _context;
    private const int PageSize = 10;

    public CustomersController(SakilaContext context) => _context = context;

    public async Task<IActionResult> Index(string searchString, int page = 1)
    {
        var query = _context.Customers
            .Where(c => c.Active == 1)
            .AsQueryable();

        if (!string.IsNullOrEmpty(searchString))
        {
            query = query.Where(c => c.FirstName.Contains(searchString)
                                  || c.LastName.Contains(searchString)
                                  || c.Email.Contains(searchString));
            ViewData["SearchString"] = searchString;
        }

        query = query.OrderBy(c => c.LastName).ThenBy(c => c.FirstName);
        var paginatedList = await PaginatedList<Customer>.CreateAsync(query, page, PageSize);
        return View(paginatedList);
    }

    public async Task<IActionResult> Details(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null || customer.Active != 1) return NotFound();
        return View(customer);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var stores = await _context.Stores.Where(s => s.Active == 1).OrderBy(s => s.StoreId).ToListAsync();
        var defaultStoreId = stores.Count > 0 ? stores[0].StoreId : 0;
        ViewBag.Stores = new SelectList(stores.Select(s => new { s.StoreId, s.DisplayName }), "StoreId", "DisplayName", defaultStoreId);
        var model = new Customer { StoreId = defaultStoreId };
        if (defaultStoreId > 0)
        {
            var addr = stores[0].AddressId;
            model.AddressId = addr;
        }
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Customer customer)
    {
        if (!ModelState.IsValid)
        {
            var stores = await _context.Stores.Where(s => s.Active == 1).OrderBy(s => s.StoreId).ToListAsync();
            ViewBag.Stores = new SelectList(stores.Select(s => new { s.StoreId, s.DisplayName }), "StoreId", "DisplayName", customer.StoreId);
            return View(customer);
        }

        var firstStoreId = await _context.Stores.Where(s => s.Active == 1).OrderBy(s => s.StoreId).Select(s => s.StoreId).FirstOrDefaultAsync();
        if (customer.StoreId == 0 && firstStoreId > 0) customer.StoreId = firstStoreId;

        // asignar AddressId según la tienda seleccionada
        if (customer.StoreId > 0)
        {
            var storeAddressId = await _context.Stores.Where(s => s.StoreId == customer.StoreId).Select(s => s.AddressId).FirstOrDefaultAsync();
            if (storeAddressId > 0) customer.AddressId = storeAddressId;
        }

        customer.CreateDate = DateTime.Now;
        customer.LastUpdate = DateTime.Now;
        customer.Active = 1;
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Cliente creado";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null || customer.Active != 1) return NotFound();
        var stores = await _context.Stores.Where(s => s.Active == 1).OrderBy(s => s.StoreId).ToListAsync();
        ViewBag.Stores = new SelectList(stores.Select(s => new { s.StoreId, s.DisplayName }), "StoreId", "DisplayName", customer.StoreId);
        // asegurar AddressId sincronizada con la tienda
        if (customer.StoreId > 0)
        {
            var storeAddressId = stores.FirstOrDefault(s => s.StoreId == customer.StoreId)?.AddressId ?? 0;
            customer.AddressId = storeAddressId;
        }
        return View(customer);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Customer customer)
    {
        if (id != customer.CustomerId) return BadRequest();
        if (!ModelState.IsValid)
        {
            var stores = await _context.Stores.Where(s => s.Active == 1).OrderBy(s => s.StoreId).ToListAsync();
            ViewBag.Stores = new SelectList(stores.Select(s => new { s.StoreId, s.DisplayName }), "StoreId", "DisplayName", customer.StoreId);
            return View(customer);
        }
        // asegurar AddressId según la tienda seleccionada
        if (customer.StoreId > 0)
        {
            var storeAddressId = await _context.Stores.Where(s => s.StoreId == customer.StoreId).Select(s => s.AddressId).FirstOrDefaultAsync();
            if (storeAddressId > 0) customer.AddressId = storeAddressId;
        }

        customer.LastUpdate = DateTime.Now;
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Cliente actualizado";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null || customer.Active != 1) return NotFound();
        return View(customer);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer != null)
        {
            customer.Active = 0;
            customer.LastUpdate = DateTime.Now;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Cliente eliminado";
        }
        return RedirectToAction(nameof(Index));
    }
}
