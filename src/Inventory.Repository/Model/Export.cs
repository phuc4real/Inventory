using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.Model
{
    public class Export
    {
        [Key]
        public int Id { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        [ForeignKey(nameof(CreatedBy))]
        public AppUser? CreatedByUser { get; set; }

        public IList<Item>? Items { get; set; }
        public IList<ExportDetail>? Details { get; set; }
    }
}
