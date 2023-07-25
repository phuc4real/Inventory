namespace Inventory.Core.Response
{
    public class PaginationResponse<T> : ResultResponse<IEnumerable<T>> where T : class
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
    }
}
