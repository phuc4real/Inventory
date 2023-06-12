using System.ComponentModel.DataAnnotations;

namespace Inventory.Core.ViewModel
{
    public class CatalogDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    public class CatalogEditDTO
    {
        [Required]
        public string? Name { get; set; }
    }
}
