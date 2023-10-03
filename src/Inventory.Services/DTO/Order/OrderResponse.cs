using Inventory.Service.Common;

namespace Inventory.Service.DTO.Order
{
    public class OrderResponse
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
    }

    public class OrderPageResponse : PaginationResponse<OrderResponse> { }

    public class OrderObjectResponse : ObjectResponse<OrderResponse> { }
}
