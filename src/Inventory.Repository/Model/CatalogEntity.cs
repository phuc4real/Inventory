using System.ComponentModel.DataAnnotations;

namespace Inventory.Repository.Model
{
    public class CatalogEntity
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public bool IsDeleted { get; set; }
    }
}
