using System.ComponentModel.DataAnnotations;
namespace SakilaApp.Models;

public class Category
{
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [MaxLength(25, ErrorMessage = "Máximo 25 caracteres")]
    [Display(Name = "Nombre")]
    public string Name { get; set; } = string.Empty;

    public DateTime LastUpdate { get; set; } = DateTime.Now;
    public byte Active { get; set; } = 1;
}
