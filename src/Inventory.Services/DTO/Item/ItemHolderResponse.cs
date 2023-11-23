using Inventory.Service.Common;

namespace Inventory.Service.DTO.Item
{
    public class ItemHolderResponse
    {
        public int ItemId { get; set; }
        public string? ItemCode { get; set; }
        public string? ItemName { get; set; }
        public string? ItemImageUrl { get; set; }

        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }

        public int ExportId { get; set; }

        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
    }

    public class ItemHolderListResponse : PaginationResponse<ItemHolderResponse> { }
}
