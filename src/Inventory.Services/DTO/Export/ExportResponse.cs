using Inventory.Service.Common;
using Inventory.Service.DTO.Order;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Service.DTO.Export
{
    public class ExportResponse : IEquatable<ExportResponse>
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public int TicketRecordId { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public string? ExportFor { get; set; }

        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        public bool Equals([AllowNull] ExportResponse other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Id.Equals(other.Id);
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class ExportObjectResponse : ObjectResponse<ExportResponse> { }

    public class ExportPaginationResponse : PaginationResponse<ExportResponse> { }
}
