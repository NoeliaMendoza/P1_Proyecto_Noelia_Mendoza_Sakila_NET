# SakilaApp

Aplicación web para gestión de videotienda, desarrollada con ASP.NET Core MVC y Entity Framework Core sobre la base de datos Sakila en SQL Server.

---

## Descripción

SakilaApp implementa el patrón MVC para administrar los recursos de una videotienda: películas, actores, clientes, alquileres, inventarios, categorías y tiendas. Incluye autenticación de usuarios con ASP.NET Identity.

---

## Tecnologías

| Tecnología | Versión |
|---|---|
| ASP.NET Core MVC | .NET 10 |
| Entity Framework Core | Latest |
| SQL Server Express | 2022 |
| ASP.NET Identity | Incluido |
| Bootstrap 5 | CDN |
| Bootstrap Icons | CDN |

---

## Estructura del proyecto

```
SakilaApp/
├── Controllers/
│   ├── HomeController.cs         # Página de inicio + estadísticas
│   ├── AccountController.cs      # Login, Register, ForgotPassword
│   ├── FilmsController.cs        # CRUD de películas
│   ├── ActorsController.cs       # CRUD de actores
│   ├── CustomersController.cs    # CRUD de clientes
│   ├── RentalsController.cs      # CRUD de alquileres
│   ├── InventoriesController.cs  # CRUD de inventarios
│   ├── CategoriesController.cs   # CRUD de categorías
│   └── StoresController.cs       # CRUD de tiendas
│
├── Models/
│   ├── Film.cs
│   ├── Actor.cs
│   ├── Customer.cs
│   ├── Rental.cs
│   ├── Inventory.cs
│   ├── Category.cs
│   ├── Store.cs
│   ├── FilmActor.cs              # Relación muchos-a-muchos Film <-> Actor
│   ├── SakilaContext.cs          # DbContext de Entity Framework
│   ├── PaginatedList.cs          # Helper de paginación genérica
│   ├── LoginViewModel.cs
│   ├── RegisterViewModel.cs
│   ├── ForgotPasswordViewModel.cs
│   └── ResetPasswordViewModel.cs
│
├── Views/
│   ├── Home/Index.cshtml
│   ├── Films/                    # Index, Details, Create, Edit, Delete
│   ├── Actors/
│   ├── Customers/
│   ├── Rentals/
│   ├── Inventories/
│   ├── Categories/
│   ├── Stores/
│   ├── Account/
│   └── Shared/_Layout.cshtml
│
├── Migrations/
├── Services/
│   └── ConsoleEmailSender.cs
├── appsettings.json
└── Program.cs
```

---

## Funcionalidades

### Página de inicio con estadísticas

Al iniciar sesión, la pantalla principal muestra tres tarjetas con conteos en tiempo real consultados a la base de datos:

- Películas activas
- Clientes registrados
- Alquileres activos

### Autenticación con ASP.NET Identity

- Registro con email y contraseña
- Inicio de sesión con opción "Recordarme"
- Recuperación y restablecimiento de contraseña
- Todas las rutas de gestión están protegidas con `[Authorize]`

### CRUD para 7 entidades

| Acción | Descripción |
|---|---|
| Index | Listado paginado con búsqueda |
| Details | Vista detallada del registro |
| Create | Formulario con validaciones del lado servidor |
| Edit | Formulario de edición con validaciones |
| Delete | Borrado lógico (`Active = 0`) |

### Mensajes de éxito

Todas las vistas `Index` muestran una alerta al completar una operación (crear, editar o eliminar), usando `TempData["Success"]` para pasar el mensaje del controlador a la vista.

### Paginación

Todas las listas usan `PaginatedList<T>` con 10 registros por página, navegación numérica con ventana deslizante de 5 páginas y búsqueda que preserva la página actual.

### Validaciones en modelos

Los modelos usan data annotations de `System.ComponentModel.DataAnnotations`:

```csharp
// Customer.cs
[Required(ErrorMessage = "El nombre es obligatorio")]
[MaxLength(45, ErrorMessage = "Máximo 45 caracteres")]
public string FirstName { get; set; }

[Required(ErrorMessage = "El email es obligatorio")]
[EmailAddress(ErrorMessage = "Formato de email inválido")]
[MaxLength(50)]
public string Email { get; set; }
```

| Modelo | Validaciones |
|---|---|
| `Actor` | `[Required]`, `[MaxLength(45)]` en nombre y apellido |
| `Category` | `[Required]`, `[MaxLength(25)]` en nombre |
| `Customer` | `[Required]`, `[MaxLength]`, `[EmailAddress]` |
| `Film` | `[Required]`, `[MaxLength(128)]` título, `[MaxLength(500)]` descripción |
| `Store` | `[Required]` en `ManagerStaffId` y `AddressId` |
| `LoginViewModel` | `[Required]`, `[EmailAddress]` |
| `RegisterViewModel` | `[Required]`, `[EmailAddress]`, `[StringLength(min=6)]`, `[Compare]` |

---

## Modelo de datos

```
Film ────< FilmActor >──── Actor
 |
 └──< Inventory >──── Rental
                          |
                       Customer

Store ──── Inventory
Category (independiente)
```

Todas las entidades usan borrado lógico con el campo `Active` (`byte`): `1` activo, `0` eliminado.

---

## Configuración y ejecución

**Prerrequisitos**

- Visual Studio 2022 o superior
- .NET 10 SDK
- SQL Server Express 2022
- Base de datos Sakila restaurada

**1. Restaurar la base de datos**

Abre SSMS, crea una base de datos llamada `sakila`, abre el archivo `sakila.sql` y ejecútalo (F5).

**2. Cadena de conexión**

Edita `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=sakila;Trusted_Connection=true;Encrypt=false;Min Pool Size=5;Max Pool Size=20;"
  }
}
```

Ajusta el nombre de la instancia si es diferente (ej. `DESKTOP-ABC\\MSSQLSERVER`).

**3. Aplicar migraciones**

En la Consola del Administrador de Paquetes de Visual Studio:

```powershell
Update-Database
```

**4. Ejecutar**

Presiona F5 en Visual Studio o desde terminal:

```bash
dotnet run
```

La app quedará disponible en `https://localhost:{puerto}`.

---

## Primer acceso

Regístrate en `/Account/Register` con cualquier email y una contraseña de mínimo 6 caracteres. Una vez autenticado tendrás acceso a todos los módulos.

---

## Paquetes NuGet

```
Microsoft.EntityFrameworkCore
Microsoft.EntityFrameworkCore.SqlServer
Microsoft.EntityFrameworkCore.Tools
Microsoft.AspNetCore.Identity.EntityFrameworkCore
Microsoft.AspNetCore.Identity.UI
Humanizer
```
