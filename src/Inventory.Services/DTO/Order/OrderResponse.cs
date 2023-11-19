using Inventory.Service.Common;
using System.Diagnostics.CodeAnalysis;

namespace Inventory.Service.DTO.Order
{
    public class OrderResponse : IEquatable<OrderResponse>
    {
        public int OrderId { get; set; }
        public int RecordId { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CompletedDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public bool Equals([AllowNull] OrderResponse other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return OrderId.Equals(other.OrderId) && RecordId.Equals(other.RecordId);
        }
        public override int GetHashCode()
        {
            int OrderIdHashCode = OrderId.GetHashCode();
            int RecordIdHashCode = RecordId.GetHashCode();

            return OrderIdHashCode ^ RecordIdHashCode;
        }
    }

    public class OrderPageResponse : PaginationResponse<OrderResponse> { }

    public class OrderObjectResponse : ObjectResponse<OrderResponse>
    {
        public List<RecordHistoryResponse>? History { get; set; }
    }
}
