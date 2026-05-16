using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SakilaApp.Models;

namespace SakilaApp.Controllers;

public class HomeController : Controller
{
    private readonly SakilaContext _context;

    public HomeController(SakilaContext context) => _context = context;

    public async Task<IActionResult> Index()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            ViewData["FilmCount"]     = await _context.Films.CountAsync(f => f.Active == 1);
            ViewData["CustomerCount"] = await _context.Customers.CountAsync(c => c.Active == 1);
            ViewData["RentalCount"]   = await _context.Rentals.CountAsync(r => r.Active == 1);
        }
        return View();
    }

    public IActionResult Privacy() => View();
}
