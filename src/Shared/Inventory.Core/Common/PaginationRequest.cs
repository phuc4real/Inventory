namespace Inventory.Core.Common
{
    public class PaginationRequest : BaseRequest
    {
        public int? Index { get; set; }
        public int? Size { get; set; }
        public string? Sort { get; set; }
        public string? SortDirection { get; set; }
        public string? SearchKeyword { get; set; }
        public bool? IsInactive { get; set; } = false;
    }
}
