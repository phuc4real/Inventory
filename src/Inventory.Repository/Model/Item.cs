using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.Model
{
    public class Item : BaseModel
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public int InStock { get; set; }
        public int Used { get; set;}

        public Guid? CatalogId { get; set; }
        [ForeignKey(nameof(CatalogId))]
        public Catalog? Catalog { get; set; }
    }
}
