using System.ComponentModel.DataAnnotations;

namespace Inventory.Core.ViewModel
{
    public class Item
    {
        public Guid Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public Catalog? Catalog { get; set; }
    }

    public class UpdateItem
    {
        [Required]
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        [Required]
        public int CatalogId { get; set; }
    }
}
