using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SakilaApp.Models;
using SakilaApp.Services;
using static System.Collections.Specialized.BitVector32;
var builder = WebApplication.CreateBuilder(args);
// Agregar servicios MVC
builder.Services.AddControllersWithViews();
// Agregar servicios de Razor Pages (necesario para MapRazorPages)
builder.Services.AddRazorPages();
// Configurar DbContext con SQL Server
builder.Services.AddDbContext<SakilaContext>(options =>

options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Configurar Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<SakilaContext>()
.AddDefaultTokenProviders();
// Configurar cookies de autenticación
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});
// Registrar servicio de email
builder.Services.AddTransient<IEmailSender, ConsoleEmailSender>();
var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
 name: "default",
 pattern: "{controller=Home}/{action=Index}/{id?}");
// Si necesitas páginas Razor (por ejemplo, las de Identity), descomenta la siguiente línea
app.MapRazorPages();
app.Run();