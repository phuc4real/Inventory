using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Core.ViewModel
{
    public class ExportDTO
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public AppUserDTO? CreatedByUser { get; set; }
    }

    public class ExportWithDetailDTO : ExportDTO
    {
        public IList<ExportDetailDTO>? Details { get; set; }
    }

    public class ExportCreateDTO
    {
        public string? Description { get; set; }
        public IList<ExportDetailCreateDTO>? Details { get; set; }
    }
}
