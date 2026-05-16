using System.ComponentModel.DataAnnotations;
namespace SakilaApp.Models;

public class Customer
{
    public int CustomerId { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [MaxLength(45, ErrorMessage = "Máximo 45 caracteres")]
    [Display(Name = "Nombre")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio")]
    [MaxLength(45, ErrorMessage = "Máximo 45 caracteres")]
    [Display(Name = "Apellido")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email es obligatorio")]
    [MaxLength(50, ErrorMessage = "Máximo 50 caracteres")]
    [EmailAddress(ErrorMessage = "Formato de email inválido")]
    [Display(Name = "Correo electrónico")]
    public string Email { get; set; } = string.Empty;

    public byte Active { get; set; } = 1;
    public DateTime CreateDate { get; set; } = DateTime.Now;
    public DateTime LastUpdate { get; set; } = DateTime.Now;
}
