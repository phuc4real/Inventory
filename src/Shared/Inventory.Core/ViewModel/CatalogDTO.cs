using System.ComponentModel.DataAnnotations;

namespace Inventory.Core.ViewModel
{
    public class CatalogDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
    }
}
