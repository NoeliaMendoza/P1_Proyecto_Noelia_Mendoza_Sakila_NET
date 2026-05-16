using System.ComponentModel.DataAnnotations;
namespace SakilaApp.Models;

public class Film
{
    public int FilmId { get; set; }

    [Required(ErrorMessage = "El título es obligatorio")]
    [MaxLength(128, ErrorMessage = "Máximo 128 caracteres")]
    [Display(Name = "Título")]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "Máximo 500 caracteres")]
    [Display(Name = "Descripción")]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "Año de estreno")]
    public string ReleaseYear { get; set; } = string.Empty;

    [Display(Name = "Duración del alquiler (días)")]
    public byte RentalDuration { get; set; }

    [Display(Name = "Tarifa de alquiler")]
    public decimal RentalRate { get; set; }

    [Display(Name = "Duración (min)")]
    public short? Length { get; set; }

    [Display(Name = "Costo de reposición")]
    public decimal ReplacementCost { get; set; }

    [MaxLength(10)]
    [Display(Name = "Clasificación")]
    public string Rating { get; set; } = string.Empty;

    public byte LanguageId { get; set; }
    public byte? OriginalLanguageId { get; set; }
    public DateTime LastUpdate { get; set; }
    public byte Active { get; set; } = 1;
    public virtual ICollection<FilmActor> FilmActors { get; set; } = new List<FilmActor>();
}
