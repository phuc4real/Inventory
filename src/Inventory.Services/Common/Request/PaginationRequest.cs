namespace Inventory.Service.Common.Request
{
    public class PaginationRequest
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string? SortField { get; set; }
        public string? SortDirection { get; set; }
        public string? SearchKeyword { get; set; }
    }
}
