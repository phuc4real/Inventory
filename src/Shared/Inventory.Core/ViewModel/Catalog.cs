using System.ComponentModel.DataAnnotations;

namespace Inventory.Core.ViewModel
{
    public class Catalog
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    public class UpdateCatalog
    {
        [Required]
        public string? Name { get; set; }
    }
}
