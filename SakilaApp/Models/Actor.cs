using System.ComponentModel.DataAnnotations;
namespace SakilaApp.Models;

public class Actor
{
    public int ActorId { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [MaxLength(45, ErrorMessage = "Máximo 45 caracteres")]
    [Display(Name = "Nombre")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio")]
    [MaxLength(45, ErrorMessage = "Máximo 45 caracteres")]
    [Display(Name = "Apellido")]
    public string LastName { get; set; } = string.Empty;

    public DateTime LastUpdate { get; set; }
    public byte Active { get; set; } = 1;
    public virtual ICollection<FilmActor> FilmActors { get; set; } = new List<FilmActor>();
}
