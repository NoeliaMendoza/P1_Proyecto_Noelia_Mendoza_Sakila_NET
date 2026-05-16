using System.ComponentModel.DataAnnotations;
namespace SakilaApp.Models;

public class Inventory
{
    public int InventoryId { get; set; }
    public int FilmId { get; set; }
    public int StoreId { get; set; }
    public DateTime LastUpdate { get; set; }
    public byte Active { get; set; } = 1;
}