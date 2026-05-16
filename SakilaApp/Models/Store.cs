using System.ComponentModel.DataAnnotations;
namespace SakilaApp.Models;

public class Store
{
    public int StoreId { get; set; }

    [Required(ErrorMessage = "El gerente es obligatorio")]
    [Display(Name = "ID Gerente")]
    public int ManagerStaffId { get; set; }

    [Required(ErrorMessage = "La dirección es obligatoria")]
    [Display(Name = "ID Dirección")]
    public int AddressId { get; set; }

    public DateTime LastUpdate { get; set; }
    public byte Active { get; set; } = 1;
}
