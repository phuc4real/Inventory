using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Core.ViewModel
{
    public class ItemDTO
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public int InStock { get; set; }
        public int Used { get; set; }
        public CatalogDTO? Catalog { get; set; }
        public DateTime CreatedDate { get; set; }
        public AppUserDTO? CreatedByUser { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public AppUserDTO? ModifiedByUser { get; set; }
    }

    public class ItemEditDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public int CatalogId { get; set; }
    }
}
