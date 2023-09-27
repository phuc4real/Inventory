using Inventory.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Service.DTO.Export
{
    public class ExportResponse
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public int StatusId { get; set; }
        public string? ExportFor { get; set; }

        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsInactive { get; set; }
        public DateTime InactiveAt { get; set; }
        public string? InactiveBy { get; set; }
    }

    public class ExportPaginationResponse : PaginationResponse<ExportResponse> { }
}
